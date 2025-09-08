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

        public void OnReady()
        {
            _heroRegistry.LoadSelectedHero();
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

            // Correct in-progress check
            if (_inProgressSessionInfo != null &&
                _inProgressSessionInfo.InProgressSave != null &&
                _inProgressSessionInfo.InProgressSave.InProgress)
            {
                if (_inProgressSessionInfo.IsValid())
                {
                    var inProgressScreen = _uiFrame.Open<InProgressSessionConfirmationScreen>();
                    inProgressScreen.Confirmed += InProgressScreenOnConfirmed;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("Invalid in progress save. Clear it and ignore it");
#endif
                    _inProgressSessionInfo.ClearProgress();
                    _saveManager.Save();
                }
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
