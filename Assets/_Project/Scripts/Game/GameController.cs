using System;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Levels;
using Project.Game.Player;
using Project.Game.Shop;
using Project.Game.UI;
using Project.Heroes;
using Project.Tiers;
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
        [Inject] private readonly PlayerInput _playerInput;
        [Inject] private readonly HeroInfo _heroInfo;
        [Inject] private readonly PlayerInfo _playerInfo;
        [Inject] private readonly SessionInfo _sessionInfo;
        [Inject ("weapons")] private TieredGroupDatabase _weaponDatabase;
        [Inject ("items")] private TieredGroupDatabase _itemDatabase;

        private void Start()
        {
            _playerInput.Hide();
            _uiFrame.Open<HudScreen>();
            _uiFrame.Open<DamageOverlayScreen>();
            
            //_uiFrame.Open<WeaponTestScreen>();
            if (_heroInfo.Data.StartingWeapons.Length <= 0)
            {
                var weaponSelector = _uiFrame.Open<RandomItemSelectorScreen>();
                weaponSelector.Initialize(_heroInfo.Data.StartingWeaponDatabase == null
                    ? _weaponDatabase 
                    : _heroInfo.Data.StartingWeaponDatabase );
                weaponSelector.OnCloseEvent += OnWeaponSelectorClosed;
            }
            else
            {
                StartLevel();
            }
        }

        private void OnWeaponSelectorClosed(UIScreen weaponSelector)
        {
            weaponSelector.OnCloseEvent -= OnWeaponSelectorClosed;
            StartLevel();
        }

        private void StartLevel()
        {
            _levelController.WaveCompleted += OnWaveCompleted;
            _levelController.LevelCompleted += OnLevelCompleted;
            
            _playerInput.Show();
            _levelController.BeginNextWave(_sessionInfo.LevelIndex,1f);
        }

        private void OnWaveCompleted()
        {
            _playerController.HandleWaveComplete();
            _playerInput.Hide();
            
            var waveCompleteScreen = _uiFrame.Open<WaveCompleteScreen>();
            waveCompleteScreen.OnCloseEvent += OnWaveCompleteScreenClosed;
        }

        private void OnWaveCompleteScreenClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnWaveCompleteScreenClosed;

            if (_heroInfo.WaveRewards > 0)
            {
                var waveRewardsScreen = _uiFrame.Open<WaveRewardsScreen>();
                waveRewardsScreen.Initialize(_levelController.CurrentWaveIndex);
                waveRewardsScreen.OnCloseEvent += OnWaveRewardsClosed;
            }
            else
            {
                var upgradeSelector = _uiFrame.Open<StatUpgradeSelectorScreen>();
                upgradeSelector.Initialize();
                upgradeSelector.OnCloseEvent += OnUpgradeSelectorClosed;
            }
        }

        private void OnUpgradeSelectorClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnUpgradeSelectorClosed;
            
            var shopScreen = _uiFrame.Open<ShopScreen>();
            shopScreen.Initialize();
            shopScreen.OnCloseEvent += OnShopClosed;
        }

        private void OnWaveRewardsClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnWaveRewardsClosed;
            
            var upgradeSelector = _uiFrame.Open<StatUpgradeSelectorScreen>();
            upgradeSelector.Initialize();
            upgradeSelector.OnCloseEvent += OnUpgradeSelectorClosed;
        }

        private void OnShopClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnShopClosed;
            
            _playerController.transform.position = Vector3.zero;
            _playerController.enabled = true;
            
            _playerInput.Show();
            _levelController.BeginNextWave(0,1f);
        }

        private void OnLevelCompleted()
        {
            if (_levelController.CurrentLevelIndex >= _playerInfo.PlayerSave.TopStageIndex)
            {
                _playerInfo.IncrementTopStage();
            }
            
            _playerController.enabled = false;
            _playerController.HandleWaveComplete();
            _playerInput.Hide();
            
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
            _playerController.gameObject.SetActive(false);
            _playerInput.Hide();

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                OnLevelCompleted();
            }
        }
    }
}