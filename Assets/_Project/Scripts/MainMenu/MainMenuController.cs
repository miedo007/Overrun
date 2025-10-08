using System.Collections;
using Mtl.Injection;
using Mtl.Save;
using Mtl.UiFramework;
using MTLSimpleAudio;
using Project.Application;
using Project.Game;
using Project.Heroes;
using Project.MainMenu.HeroSelection;
using Project.MainMenu.SagaMap;
using Project.Settings;
using UnityEngine;

namespace Project.MainMenu
{
    public class MainMenuController : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private MusicData musicData;

        [Inject] private readonly UIFrame _uiFrame;
        [Inject] private readonly SceneLoader _sceneLoader;
        [Inject] private readonly SessionInfo _sessionInfo;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly InProgressSessionInfo _inProgressSessionInfo;
        [Inject] private readonly PlayerInfo _playerInfo;
        [Inject] private readonly SaveManager _saveManager;

        private NavBarScreen _navBar;
        private bool _hasCheckedInProgressSession = false;

        public void OnReady()
        {
            _heroRegistry.LoadSelectedHero();
            
            // Subscribe to login status changes to reload save data
            SaveSystemIntegration.OnLoginStatusChanged += OnLoginStatusChanged;
        }

        private void OnLoginStatusChanged(bool isLoggedIn)
        {
            Debug.Log($"MainMenuController: Login status changed to {isLoggedIn}, hasChecked: {_hasCheckedInProgressSession}");
            
            if (isLoggedIn)
            {
                Debug.Log("User logged in - resetting hasChecked flag and reloading save data");
                _hasCheckedInProgressSession = false; // Reset the flag to allow checking again
                StartCoroutine(ReloadSaveDataAndCheckInProgress());
            }
        }

        private System.Collections.IEnumerator ReloadSaveDataAndCheckInProgress()
        {
            Debug.Log("Starting reload coroutine...");
            
            // Reload both in-progress session data AND hero data from cloud storage
            Debug.Log("Attempting to reload in-progress session data from cloud storage");
            
            // Force reload the in-progress session from cloud storage directly into the existing instance
            var inProgressReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("in-progress");
            
            _saveManager.TryLoad(_inProgressSessionInfo, success =>
            {
                Debug.Log($"Reloaded in-progress session data: success={success}");
                if (success)
                {
                    Debug.Log($"Loaded data - InProgress: {_inProgressSessionInfo.InProgressSave?.InProgress}, Level: {_inProgressSessionInfo.InProgressSave?.LevelIndex}, Wave: {_inProgressSessionInfo.InProgressSave?.WaveIndex}");
                    CheckForInProgressSession();
                }
                else
                {
                    Debug.Log("Failed to reload in-progress session data");
                    CheckForInProgressSession(); // Still check in case there's local data
                }
            }, inProgressReadWriter);
            
            // CRITICAL FIX: Also reload hero data from cloud storage
            Debug.Log("Attempting to reload hero data from cloud storage");
            var heroReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("heroes");
            
            // First, try to load the raw JSON data directly
            if (heroReadWriter.TryLoad(out var rawHeroData))
            {
                Debug.Log($"Raw hero data from cloud: {rawHeroData}");
                
                // Parse the JSON to verify what we're getting
                try 
                {
                    var heroSaveData = Newtonsoft.Json.JsonConvert.DeserializeObject<Project.Heroes.HeroSave>(rawHeroData);
                    if (heroSaveData != null)
                    {
                        var heroId = "data_hero_0"; // The hero we're tracking
                        var cloudLevel = heroSaveData.GetHeroLevel(heroId);
                        Debug.Log($"Parsed cloud data - Hero: {heroId}, Level: {cloudLevel}");
                        
                        // Force update the HeroRegistry's save data directly
                        Debug.Log("Force updating HeroRegistry save data");
                        var registrySave = (Project.Heroes.HeroSave)_heroRegistry.Save;
                        registrySave.SelectedHero = heroSaveData.SelectedHero;
                        registrySave.HeroLevels = heroSaveData.HeroLevels;
                        
                        // Verify the update worked
                        var updatedLevel = registrySave.GetHeroLevel(heroId);
                        Debug.Log($"After force update - Save Level: {updatedLevel}");
                        
                        // Refresh the active hero
                        var selectedHeroData = _heroRegistry.GetSelectedHero();
                        _heroRegistry.SetActiveHero(selectedHeroData);
                        
                        var activeHero = _heroRegistry.ActiveHero;
                        Debug.Log($"Hero reloaded - ID: {heroId}, Level: {activeHero.Level}");
                    }
                    else 
                    {
                        Debug.LogError("Failed to parse hero save data from cloud");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Exception parsing hero data: {ex.Message}");
                }
            }
            else 
            {
                Debug.Log("Failed to load raw hero data from cloud");
            }
            
            yield break; // Exit immediately, callbacks handle the rest
        }

        private void Start()
        {
            musicData.Play();

            // Set target framerate
            UnityEngine.Application.targetFrameRate = 60;

            _uiFrame.Open<MainMenuHudScreen>();
            _uiFrame.Open<SagaMapScreen>();

            _navBar = _uiFrame.Open<NavBarScreen>();
            _navBar.PlayButtonClicked += OnPlayButtonClicked;
            _navBar.UpgradeButtonClicked += OnUpgradeButtonClicked;
            _navBar.SettingsButtonClicked += OnSettingsButtonCLicked;

#if UNITY_WEBGL && !UNITY_EDITOR
            // Menu is not active gameplay
            CrazySdkManager.GameplayStop();
#endif

            // Check for in-progress session initially - with smart cloud detection
            if (SaveSystemIntegration.IsUserLoggedIn && SaveSystemIntegration.IsCrazySDKReady())
            {
                Debug.Log("User is already logged in, checking cloud data immediately");
                StartCoroutine(LoadCloudDataAndCheck());
            }
            else
            {
                Debug.Log("CrazySDK not ready yet or user not logged in - waiting for SDK initialization");
                StartCoroutine(WaitForSDKAndCheckInProgress());
            }
        }

        private System.Collections.IEnumerator LoadCloudDataAndCheck()
        {
            Debug.Log("Loading cloud data immediately for logged-in user");
            
            // No artificial delay - load cloud data right away
            var inProgressReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("in-progress");
            
            _saveManager.TryLoad(_inProgressSessionInfo, success =>
            {
                Debug.Log($"Loaded in-progress session from cloud: success={success}");
                if (success)
                {
                    Debug.Log($"Cloud data - InProgress: {_inProgressSessionInfo.InProgressSave?.InProgress}, Level: {_inProgressSessionInfo.InProgressSave?.LevelIndex}, Wave: {_inProgressSessionInfo.InProgressSave?.WaveIndex}");
                }
                CheckForInProgressSession();
            }, inProgressReadWriter);
            
            yield break; // Exit immediately, callback handles the rest
        }

        private System.Collections.IEnumerator WaitForSDKAndCheckInProgress()
        {
            Debug.Log("Waiting for CrazySDK to be ready before checking in-progress session...");
            
#if UNITY_EDITOR
            // In Editor, CrazySDK will never be ready, so check immediately with local storage
            Debug.Log("Running in Unity Editor - checking in-progress session immediately with local storage");
            CheckForInProgressSession();
            yield break;
#endif
            
            // Wait for CrazySDK to be ready (important for guest users who use localStorage)
            while (!SaveSystemIntegration.IsCrazySDKReady())
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.Log("CrazySDK is now ready - checking for in-progress session");
            
            // Now that SDK is ready, reload data using the proper storage backend
            var inProgressReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("in-progress");
            
            _saveManager.TryLoad(_inProgressSessionInfo, success =>
            {
                Debug.Log($"Loaded in-progress session after SDK ready: success={success}");
                if (success)
                {
                    Debug.Log($"SDK-ready data - InProgress: {_inProgressSessionInfo.InProgressSave?.InProgress}, Level: {_inProgressSessionInfo.InProgressSave?.LevelIndex}, Wave: {_inProgressSessionInfo.InProgressSave?.WaveIndex}");
                }
                CheckForInProgressSession();
            }, inProgressReadWriter);
        }

        private void CheckForInProgressSession()
        {
            Debug.Log($"CheckForInProgressSession called - hasChecked: {_hasCheckedInProgressSession}");
            
            // Check if we've already handled this session OR if the screen is already open
            var existingScreen = _uiFrame.Get<InProgressSessionConfirmationScreen>();
            if (_hasCheckedInProgressSession || (existingScreen != null && existingScreen.IsOpened))
            {
                Debug.Log($"Skipping in-progress check - hasChecked: {_hasCheckedInProgressSession}, screenAlreadyOpen: {existingScreen != null && existingScreen.IsOpened}");
                return;
            }
            
            Debug.Log($"InProgressSessionInfo: null={_inProgressSessionInfo == null}");
            if (_inProgressSessionInfo != null)
            {
                Debug.Log($"InProgressSave: null={_inProgressSessionInfo.InProgressSave == null}");
                if (_inProgressSessionInfo.InProgressSave != null)
                {
                    Debug.Log($"InProgress flag: {_inProgressSessionInfo.InProgressSave.InProgress}");
                    Debug.Log($"Level: {_inProgressSessionInfo.InProgressSave.LevelIndex}, Wave: {_inProgressSessionInfo.InProgressSave.WaveIndex}");
                }
            }
            
            // Check for valid in-progress session
            if (_inProgressSessionInfo != null &&
                _inProgressSessionInfo.InProgressSave != null &&
                _inProgressSessionInfo.InProgressSave.InProgress)
            {
                Debug.Log($"Found in-progress session: Level {_inProgressSessionInfo.InProgressSave.LevelIndex}, Wave {_inProgressSessionInfo.InProgressSave.WaveIndex}");
                
                if (_inProgressSessionInfo.IsValid())
                {
                    Debug.Log("In-progress session is valid, checking if screen already exists");
                    
                    // Double-check to prevent opening duplicate screens
                    existingScreen = _uiFrame.Get<InProgressSessionConfirmationScreen>();
                    if (existingScreen == null || !existingScreen.IsOpened)
                    {
                        Debug.Log("Opening confirmation screen");
                        var inProgressScreen = _uiFrame.Open<InProgressSessionConfirmationScreen>();
                        inProgressScreen.Confirmed += InProgressScreenOnConfirmed;
                        _hasCheckedInProgressSession = true;
                    }
                    else
                    {
                        Debug.Log("In-progress confirmation screen already opened, skipping");
                        _hasCheckedInProgressSession = true;
                    }
                }
                else
                {
                    Debug.LogError("Invalid in progress save. Clear it and ignore it");
                    _inProgressSessionInfo.ClearProgress();
                    _saveManager.Save();
                    _hasCheckedInProgressSession = true;
                }
            }
            else
            {
                Debug.Log("No in-progress session found");
                _hasCheckedInProgressSession = true;
            }
        }

        private void OnDestroy()
        {
            if (_navBar != null)
            {
                _navBar.PlayButtonClicked -= OnPlayButtonClicked;
                _navBar.UpgradeButtonClicked -= OnUpgradeButtonClicked;
                _navBar.SettingsButtonClicked -= OnSettingsButtonCLicked;
            }
            
            SaveSystemIntegration.OnLoginStatusChanged -= OnLoginStatusChanged;
        }

        private void OnSettingsButtonCLicked()
        {
            _uiFrame.Open<SettingsScreen>();
#if UNITY_WEBGL && !UNITY_EDITOR
            // Still non-play
            CrazySdkManager.GameplayStop();
#endif
        }

        private void InProgressScreenOnConfirmed(bool didConfirm)
        {
            var screen = _uiFrame.Get<InProgressSessionConfirmationScreen>();
            screen.Confirmed -= InProgressScreenOnConfirmed;

            if (didConfirm && _inProgressSessionInfo.InProgressSave != null)
            {
                _heroRegistry.SetSelectedHero(_inProgressSessionInfo.InProgressSave.HeroId);
                LoadLevel(_inProgressSessionInfo.InProgressSave.LevelIndex);
            }
            else
            {
                _inProgressSessionInfo.ClearProgress();
                _saveManager.Save();
                screen.Close();
            }
        }

        private void OnPlayButtonClicked()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Start of the run: fire gameplayStart as soon as the player hits PLAY
            CrazySdkManager.GameplayStop();
#endif
            var levelIndex = _playerInfo.PlayerSave.TopStageIndex;
            OnLevelSelected(levelIndex);
        }

        private void OnUpgradeButtonClicked()
        {
            _uiFrame.Open<HeroSelectionScreen>();
#if UNITY_WEBGL && !UNITY_EDITOR
            CrazySdkManager.GameplayStop();
#endif
        }

        private void OpenHeroSelection()
        {
            _uiFrame.Open<HeroSelectionScreen>();
#if UNITY_WEBGL && !UNITY_EDITOR
            CrazySdkManager.GameplayStop();
#endif
        }

        private void OnLevelSelected(int levelIndex)
        {
            LoadLevel(levelIndex);
        }

        private void LoadLevel(int levelIndex)
        {
            _heroRegistry.SetActiveHero(_heroRegistry.GetSelectedHero());
            _sessionInfo.LevelIndex = levelIndex;

            // Do NOT start gameplay here; GameController handles it,
            // and we already fired start on PLAY.
            _sceneLoader.LoadScene("game", 0.2f, 0.5f);
        }
    }
}
