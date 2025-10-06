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
            
            // Remove artificial delay - load immediately when login detected
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
            
            yield break; // Exit immediately, callback handles the rest
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
                Debug.Log("User not logged in yet, checking local data only");
                CheckForInProgressSession();
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

        private void CheckForInProgressSession()
        {
            Debug.Log($"CheckForInProgressSession called - hasChecked: {_hasCheckedInProgressSession}");
            
            if (_hasCheckedInProgressSession) 
            {
                Debug.Log("Already checked in-progress session, skipping");
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
            
            // Correct in-progress check
            if (_inProgressSessionInfo != null &&
                _inProgressSessionInfo.InProgressSave != null &&
                _inProgressSessionInfo.InProgressSave.InProgress)
            {
                Debug.Log($"Found in-progress session: Level {_inProgressSessionInfo.InProgressSave.LevelIndex}, Wave {_inProgressSessionInfo.InProgressSave.WaveIndex}");
                
                if (_inProgressSessionInfo.IsValid())
                {
                    Debug.Log("In-progress session is valid, opening confirmation screen");
                    var inProgressScreen = _uiFrame.Open<InProgressSessionConfirmationScreen>();
                    inProgressScreen.Confirmed += InProgressScreenOnConfirmed;
                    _hasCheckedInProgressSession = true;
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
