using System;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game;
using Project.Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.MainMenu
{
    public class NavBarScreen : UIScreen, IInjectionReady
    {
        public event Action UpgradeButtonClicked;
        public event Action PlayButtonClicked;

        [SerializeField] private Button playButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Notification upgradeNotification;

        [Inject] private readonly PlayerInfo _playerInfo;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly GameData _gameData;
        
        private void Awake()
        {
            playButton.onClick.AddListener(()=>PlayButtonClicked?.Invoke());
            upgradeButton.onClick.AddListener(()=>UpgradeButtonClicked?.Invoke());
        }

        public void OnReady()
        {
            _playerInfo.OnCurrencyChanged += OnCurrencyChanged;
            OnCurrencyChanged();
        }

        private void OnDestroy()
        {
            _playerInfo.OnCurrencyChanged -= OnCurrencyChanged;
        }

        private void OnCurrencyChanged()
        {
            var upgradeCost = _gameData.GetUpgradeCost(_heroRegistry.ActiveHero.Level + 1);
            upgradeNotification.Show(_playerInfo.PlayerSave.Currency >= upgradeCost);
        }
    }
}