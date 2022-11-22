using Mtl.Injection;
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

        public void OnReady()
        {
            _heroRegistry.LoadSelectedHero();
        }

        private void Start()
        {
            _uiFrame.Open<CurrencyBarScreen>();
            _uiFrame.Open<SagaMapScreen>();
            
            var navBar = _uiFrame.Open<NavBarScreen>();
            navBar.PlayButtonClicked += OnPlayButtonClicked;    
            navBar.UpgradeButtonClicked += OnUpgradeButtonClicked;
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