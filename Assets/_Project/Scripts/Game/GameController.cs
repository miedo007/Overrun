using Mtl.Injection;
using Mtl.Save;
using Mtl.UiFramework;
using MTLSimpleAudio;
using Project.Application;
using Project.Game.Cameras;
using Project.Game.Enemies;
using Project.Game.Items;
using Project.Game.Levels;
using Project.Game.Player;
using Project.Game.Shop;
using Project.Game.UI;
using Project.Game.Weapons;
using Project.Heroes;
using Project.Tiers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Game
{
    public class GameController : MonoBehaviour, IInjectionReady
    {
        [FormerlySerializedAs("gameMusic")] [SerializeField] private MusicData waveMusic;
        [SerializeField] private MusicData shopMusic;
        [SerializeField] private MusicData menuMusic;

        [Inject] private readonly UIFrame _uiFrame;
        [Inject] private readonly InProgressSessionInfo _inProgressSession;
        [Inject] private readonly SaveManager _saveManager;
        //[Inject] private readonly IAnalyticsManager _analyticsManager;
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
        [Inject ("stat_upgrades")] private TieredGroupDatabase _statUpgradeDatabase;

        private HeroInfo _heroInfo;
        
        public void OnReady()
        {
            _playerHealthController.Depleted += OnPlayerHealthDepleted;
            _heroInfo = _heroRegistry.ActiveHero;

        }
        
        private void Start()
        {
            _playerHealthController.Initialize();
            
            var hasInProgressSession = _inProgressSession.InProgressSave.InProgress;
            
            var levelIndex = _sessionInfo.LevelIndex;
            var waveIndex = 0;
            
            if (hasInProgressSession)
            {
                LoadInProgressSession(out levelIndex, out waveIndex);
            }
            
            _levelController.Initialize(levelIndex, waveIndex);
            
            _playerInput.Hide();
            _uiFrame.Open<HudScreen>();
            _uiFrame.Open<DamageOverlayScreen>();
            
            //_uiFrame.Open<WeaponTestScreen>();
            if (!hasInProgressSession)
            {
                OpenWeaponSelector();
            }
            else
            {
                StartLevel();
            }
        }

        private void OnSettingsScreenOpened(UIScreen obj)
        {
            throw new System.NotImplementedException();
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
            menuMusic.Play();
            
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
            shopMusic.Play();
            
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
            
            if (_levelController.WaveIndex > 0)
            {
                _inProgressSession.SaveProgress(_sessionInfo.LevelIndex,
                    _levelController.WaveIndex,
                    _playerHealthController.CurrentHealth,
                    _heroInfo);
                _saveManager.Save();
            }
            
            _playerController.transform.position = Vector3.zero;
            _playerController.enabled = true;
            
            _playerInput.Show();

            BeginNextWave();
        }

        private void BeginNextWave()
        {
            waveMusic.Play();
            
            var waveIntroScreen = _uiFrame.Open<WaveIntroScreen>();
            waveIntroScreen.OnCloseEvent += OnWaveIntroCompleted;
            waveIntroScreen.DisplayWithWaveIndex(_levelController.WaveIndex, _levelController.WaveCount, _levelController.IsFinalWave);
            
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
            _levelController.BeginNextWave(0.375f);
            _enemyManager.BeginWave(_levelController.CurrentLevel,
                _levelController.CurrentLevelIndex,
                _levelController.WaveIndex);
        }

        private void OnLevelCompleted()
        {
            _inProgressSession.ClearProgress();
            _saveManager.Save();
            
            SendLevelSummaryEvent(true);
            
            menuMusic.Play();
            
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

        private void SendLevelSummaryEvent(bool completed)
        {
            /*_analyticsManager.SendLevelSummaryEvent(new LevelSummaryEvent()
            {
                Completed = completed,
                LevelId = _sessionInfo.LevelIndex
            });*/
        }
        
        private void OnLevelCompleteClosed(UIScreen screen)
        {
           LoadMainMenu();
        }
        private void OnPlayerHealthDepleted()
        {
            _inProgressSession.ClearProgress();
            _saveManager.Save();
            
            SendLevelSummaryEvent(false);
            
            menuMusic.Play();
            
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
        
        private void LoadInProgressSession(out int levelIndex, out int waveIndex)
        {
            var save = _inProgressSession.InProgressSave;
            levelIndex = save.LevelIndex;
            waveIndex = save.WaveIndex;

            _playerHealthController.CurrentHealth = _playerHealthController.MaxHealth;
            
            // Load items & stat upgrades first
            foreach (var itemId in save.Items)
            {
                var itemData = _itemDatabase.GetItemWithId(itemId);
                if (itemData != null)
                {
                    _heroInfo.AddItem(itemData as ItemData);
                }
                else
                {
                    var statUpgradeData = _statUpgradeDatabase.GetItemWithId(itemId);
                    if (statUpgradeData != null)
                    {
                        _heroInfo.AddItem(statUpgradeData as ItemData);
                    }
                }
            }

            foreach (var weaponId in _inProgressSession.InProgressSave.Weapons)
            {
                var weaponData = _weaponDatabase.GetItemWithId(weaponId);
                _heroInfo.AddWeapon(weaponData as WeaponData);
            }

            _playerHealthController.CurrentHealth = _inProgressSession.InProgressSave.Health;
            _heroInfo.ShopCurrency = _inProgressSession.InProgressSave.ShopCurrency;
        }

        #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                OnLevelCompleted();
            }
        }
        #endif
    }
}