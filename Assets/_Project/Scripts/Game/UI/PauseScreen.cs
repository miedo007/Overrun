using Mtl.Injection;
using Project.Game.Player;
using Project.Settings;
using UnityEngine;

namespace Project.Game.UI
{
    public class PauseScreen : SettingsScreen
    {
        [Inject] private readonly PlayerInput _playerInput;

        private bool _wasPlayerInputActiveOnOpen;
        
        protected override void OnOpened()
        {
            base.OnOpened();
            
            Time.timeScale = 0;

            Debug.Log(_playerInput.IsActive);
            _wasPlayerInputActiveOnOpen = _playerInput.IsActive;
            
            if (_wasPlayerInputActiveOnOpen)
            {
                _playerInput.Hide(true);
            }
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            
            Time.timeScale = 1;

            if (_wasPlayerInputActiveOnOpen)
            {
                _playerInput.Show();
            }
        }
    }
}