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
        [Inject] private readonly PlayerHealthController _playerHealthController;
        [Inject] private readonly SceneLoader _sceneLoader;
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
            var waveCompleteScreen = _uiFrame.Open<WaveCompleteScreen>();
            waveCompleteScreen.OnCloseEvent += OnWaveCompleteScreenClosed;
        }

        private void OnWaveCompleteScreenClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnWaveCompleteScreenClosed;
            
            var shopScreen = _uiFrame.Open<ShopScreen>();
            shopScreen.OnCloseEvent += OnShopClosed;
        }

        private void OnShopClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnShopClosed;
            
            _heroInfo.AddWeapon(_weaponDatabase.GetRandom());
            _levelController.BeginNextWave(0,1f);
        }

        private void OnLevelCompleted()
        {
            _sceneLoader.LoadScene(gameObject.scene.name, 0, 0.5f);
        }

        public void OnReady()
        {
            _playerHealthController.Depleted += OnPlayerHealthDepleted;
        }

        private void OnPlayerHealthDepleted()
        {
            _playerHealthController.Depleted -= OnPlayerHealthDepleted;
            _sceneLoader.LoadScene(gameObject.scene.name, 0, 0.5f);
        }
    }
}