using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Levels;
using Project.Game.Player;
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
            _heroInfo.AddWeapon(_weaponDatabase.GetRandomWeapon());
            _levelController.BeginNextWave(0,2f);
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