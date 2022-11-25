using System;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Cameras;
using Project.Game.Enemies;
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
        [Inject] private readonly EnemyManager _enemyManager;
        [Inject] private readonly CameraManager _cameraManager;
        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly PlayerHealthController _playerHealthController;
        [Inject] private readonly SceneLoader _sceneLoader;
        [Inject] private readonly PlayerInput _playerInput;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly PlayerInfo _playerInfo;
        [Inject] private readonly SessionInfo _sessionInfo;
        [Inject] private readonly GameData _gameData;
        [Inject ("weapons")] private TieredGroupDatabase _weaponDatabase;
        [Inject ("items")] private TieredGroupDatabase _itemDatabase;

        private HeroInfo _heroInfo;

        private void Start()
        {
            _heroInfo = _heroRegistry.ActiveHero;

            _playerInput.Hide();
            _uiFrame.Open<HudScreen>();
            _uiFrame.Open<DamageOverlayScreen>();
            
            //_uiFrame.Open<WeaponTestScreen>();
            OpenWeaponSelector();
        }

        private void OpenWeaponSelector()
        {
            var weaponSelector = _uiFrame.Open<RandomItemSelectorScreen>();
            weaponSelector.Initialize(_heroInfo.Data.StartingWeaponDatabase == null
                ? _weaponDatabase 
                : _heroInfo.Data.StartingWeaponDatabase );
            weaponSelector.OnCloseEvent += OnWeaponSelectorClosed;
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
            
            BeginNextWave();
        }

        private void OnWaveCompleted()
        {
            _cameraManager.ZoomIn();
            _enemyManager.EndWave();
            
            _playerController.HandleWaveComplete();
            _playerInput.Hide();

            var currencyReward = ApplySoftCurrencyReward();
            
            var waveCompleteScreen = _uiFrame.Open<WaveCompleteScreen>();
            waveCompleteScreen.Initialize(currencyReward);
            waveCompleteScreen.OnCloseEvent += OnWaveCompleteScreenClosed;
        }

        private int ApplySoftCurrencyReward()
        {
            var currencyReward =
                _gameData.GetCurrencyReward(_levelController.CurrentLevelIndex, _levelController.WaveIndex);
            
            _playerInfo.ChangeCurrency(currencyReward);
            
            return currencyReward;
        }

        private void OnWaveCompleteScreenClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnWaveCompleteScreenClosed;

            if (_heroInfo.WaveRewards > 0)
            {
                var waveRewardsScreen = _uiFrame.Open<WaveRewardsScreen>();
                waveRewardsScreen.Initialize(_levelController.WaveIndex);
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

            BeginNextWave();
        }

        private void BeginNextWave()
        {
            var waveIntroScreen = _uiFrame.Open<WaveIntroScreen>();
            waveIntroScreen.OnCloseEvent += OnWaveIntroCompleted;
            waveIntroScreen.DisplayWithWaveIndex(_levelController.WaveIndex);
            if (_levelController.IsFinalWave)
            {
                _uiFrame.Get<HudScreen>().ShowCurrencyBar();
            }
            _cameraManager.ZoomOut();
        }

        private void OnWaveIntroCompleted(UIScreen waveIntroScreen)
        {
            waveIntroScreen.OnCloseEvent -= OnWaveIntroCompleted;
            
            _playerInput.Show();
            _levelController.BeginNextWave(_sessionInfo.LevelIndex,0.375f);
            _enemyManager.BeginWave(_levelController.CurrentLevel,
                _levelController.CurrentLevelIndex,
                _levelController.WaveIndex);
        }

        private void OnLevelCompleted()
        {
            _cameraManager.ZoomIn();
            _enemyManager.EndWave();
            
            if (_levelController.CurrentLevelIndex >= _playerInfo.PlayerSave.TopStageIndex)
            {
                _playerInfo.IncrementTopStage();
            }
            
            _playerController.enabled = false;
            _playerController.HandleWaveComplete();
            _playerInput.Hide();

            var currencyReward = ApplySoftCurrencyReward();

            var levelCompleteScreen = _uiFrame.Open<WaveCompleteScreen>();
            levelCompleteScreen.Initialize(currencyReward, true);
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

        private void LoadMainMenu()
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