using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game;
using Project.Heroes;
using Project.MainMenu.HeroSelection;
using Project.Scripts.MainMenu.SagaMap;
using UnityEngine;

namespace Project.Scripts.MainMenu
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
            
            var sagamapScreen = _uiFrame.Open<SagaMapScreen>();
            sagamapScreen.LevelSelected += OnLevelSelected;
            
            OpenHeroSelection();
        }

        private void OpenHeroSelection()
        {
            _uiFrame.Open<HeroSelectionScreen>();
        }

        private void OnLevelSelected(int levelIndex)
        {
            var sagamapScreen = _uiFrame.Get<SagaMapScreen>();
            sagamapScreen.LevelSelected -= OnLevelSelected;
            
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