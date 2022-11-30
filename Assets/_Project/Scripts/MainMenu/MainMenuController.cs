using Mtl.Injection;
using Mtl.Save;
using Mtl.UiFramework;
using Project.Application;
using Project.Game;
using Project.Heroes;
using Project.MainMenu.HeroSelection;
using Project.MainMenu.SagaMap;
using UnityEngine;

namespace Project.MainMenu
{
    public class MainMenuController : MonoBehaviour, IInjectionReady
    {
        [Inject] private readonly UIFrame _uiFrame;
        [Inject] private readonly SceneLoader _sceneLoader;
        [Inject] private readonly SessionInfo _sessionInfo;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly InProgressSessionInfo _inProgressSessionInfo;
        [Inject] private readonly SaveManager _saveManager;
        
        public void OnReady()
        {
            _heroRegistry.LoadSelectedHero();
        }

        private void Start()
        {
            // Set target framerate
            UnityEngine.Application.targetFrameRate = 60;
            
            _uiFrame.Open<CurrencyBarScreen>();
            _uiFrame.Open<SagaMapScreen>();
            
            var navBar = _uiFrame.Open<NavBarScreen>();
            navBar.PlayButtonClicked += OnPlayButtonClicked;    
            navBar.UpgradeButtonClicked += OnUpgradeButtonClicked;

            if (_inProgressSessionInfo.InProgressSave.InProgress)
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

        private void InProgressScreenOnConfirmed(bool didConfirm)
        {
            var screen = _uiFrame.Get<InProgressSessionConfirmationScreen>();
            screen.Confirmed -= InProgressScreenOnConfirmed;
            
            if (didConfirm)
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
            var sagamap = _uiFrame.Get<SagaMapScreen>();
            var levelIndex = sagamap.GetSelectedLevel();
            OnLevelSelected(levelIndex);
        }

        private void OnUpgradeButtonClicked()
        {
            _uiFrame.Open<HeroSelectionScreen>();
        }

        private void OpenHeroSelection()
        {
            _uiFrame.Open<HeroSelectionScreen>();
        }

        private void OnLevelSelected(int levelIndex)
        {
            LoadLevel(levelIndex);
        }

        private void LoadLevel(int levelIndex)
        {
            _heroRegistry.SetActiveHero(_heroRegistry.GetSelectedHero());
            _sessionInfo.LevelIndex = levelIndex;
            _sceneLoader.LoadScene("game", 0.2f, 0.5f);
        }
    }
}