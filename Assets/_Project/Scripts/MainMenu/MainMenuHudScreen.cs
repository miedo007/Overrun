using System;
using Mtl.UiFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Project.MainMenu
{
    public class MainMenuHudScreen : UIScreen
    {
        public event Action SettingsButtonClicked;
        
        [SerializeField] private Button settingsButton;

        private void Awake()
        {
            settingsButton.onClick.AddListener(()=>SettingsButtonClicked?.Invoke());
        }
    }
}