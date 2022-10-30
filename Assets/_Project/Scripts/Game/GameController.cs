using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Player;
using Project.Game.UI;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Game
{
    public class GameController : MonoBehaviour, IInjectionReady
    {
        [Inject] private readonly UIFrame _uiFrame;
        [Inject] private readonly PlayerHealthController _playerHealthController;
        [Inject] private readonly SceneLoader _sceneLoader;
        

        private void Start()
        {
            _uiFrame.Open<HudScreen>();
            _uiFrame.Open<WeaponTestScreen>();
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