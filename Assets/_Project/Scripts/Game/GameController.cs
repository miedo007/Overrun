using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Levels;
using Project.Game.Player;
using Project.Game.Shop;
using Project.Game.UI;
using Project.Game.Weapons;
using Project.Heroes;
using UnityEngine;

namespace Project.Game
{
    public class GameController : MonoBehaviour, IInjectionReady
    {
        [Inject] private readonly UIFrame _uiFrame;
        [Inject] private readonly LevelController _levelController;
        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly PlayerHealthController _playerHealthController;
        [Inject] private readonly SceneLoader _sceneLoader;
        [Inject] private readonly UltimateJoystick _joystick;
        [Inject] private readonly HeroInfo _heroInfo;
        [Inject] private readonly WeaponDatabase _weaponDatabase;

        private void Start()
        {
            _uiFrame.Open<HudScreen>();
            //_uiFrame.Open<WeaponTestScreen>();

            _levelController.WaveCompleted += OnWaveCompleted;
            _levelController.LevelCompleted += OnLevelCompleted;
            
            _levelController.BeginNextWave(0,1f);
        }
        
        private void OnWaveCompleted()
        {
            _playerController.HandleWaveComplete();
            
            var waveCompleteScreen = _uiFrame.Open<WaveCompleteScreen>();
            waveCompleteScreen.OnCloseEvent += OnWaveCompleteScreenClosed;
        }

        private void OnWaveCompleteScreenClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnWaveCompleteScreenClosed;
            
            var shopScreen = _uiFrame.Open<ShopScreen>();
            shopScreen.Initialize(_levelController.CurrentWaveIndex);
            shopScreen.OnCloseEvent += OnShopClosed;
        }

        private void OnShopClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnShopClosed;
            _playerController.enabled = true;
            _levelController.BeginNextWave(0,1f);
        }

        private void OnLevelCompleted()
        {
            _playerController.enabled = false;
            _playerController.HandleWaveComplete();
            
            _joystick.gameObject.SetActive(false);
            var levelCompleteScreen = _uiFrame.Open<LevelCompleteScreen>();
            levelCompleteScreen.OnCloseEvent += OnLevelCompleteClosed;
        }

        private void OnLevelCompleteClosed(UIScreen screen)
        {
           LoadMainMenu();
        }

        public void OnReady()
        {
            _playerHealthController.Depleted += OnPlayerHealthDepleted;
        }

        private void OnPlayerHealthDepleted()
        {
            _playerHealthController.Depleted -= OnPlayerHealthDepleted;
            _levelController.WaveCompleted -= OnWaveCompleted;
            _levelController.LevelCompleted -= OnLevelCompleted;
            
            _playerController.enabled = false;
            _joystick.gameObject.SetActive(false);
            _playerController.gameObject.SetActive(false);
            
            var levelFailedScreen = _uiFrame.Open<LevelFailedScreen>();
            levelFailedScreen.ConfirmButtonClicked += OnLevelFailConfirmed;
        }

        private void OnLevelFailConfirmed()
        {
            LoadMainMenu();
        }

        public void LoadMainMenu()
        {
            _sceneLoader.LoadScene("main_menu", 0.2f, 0.5f);
        }
        
    }
}