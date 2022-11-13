using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game;
using Project.Scripts.MainMenu.SagaMap;
using UnityEngine;

namespace Project.Scripts.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [Inject] private readonly UIFrame _uiFrame;
        [Inject] private readonly SceneLoader _sceneLoader;
        [Inject] private readonly SessionInfo _sessionInfo;
        
        private void Start()
        {
            var sagamapScreen = _uiFrame.Open<SagaMapScreen>();
            sagamapScreen.LevelSelected += OnLevelSelected;
        }

        private void OnLevelSelected(int levelIndex)
        {
            var sagamapScreen = _uiFrame.Get<SagaMapScreen>();
            sagamapScreen.LevelSelected -= OnLevelSelected;
            
            LoadLevel(levelIndex);
        }

        private void LoadLevel(int levelIndex)
        {
            _sessionInfo.LevelIndex = levelIndex;
            _sceneLoader.LoadScene("game", 0.2f, 0.5f);
        }
    }
}