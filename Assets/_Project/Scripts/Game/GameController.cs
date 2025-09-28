using System.Collections;
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
using Project.Game.Tutorials;   // ★ for IdleHint
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
        [Inject("weapons")] private TieredGroupDatabase _weaponDatabase;
        [Inject("items")] private TieredGroupDatabase _itemDatabase;
        [Inject("stat_upgrades")] private TieredGroupDatabase _statUpgradeDatabase;

        private HeroInfo _heroInfo;

        // ★ cache the hint once (even if inactive)
        private IdleHint _idleHint;

        public void OnReady()
        {
            _playerHealthController.Depleted += OnPlayerHealthDepleted;
            _heroInfo = _heroRegistry.ActiveHero;

            // ★ find the IdleHint in scene (true = include inactive)
            _idleHint = FindObjectOfType<IdleHint>(true);
            
#if UNITY_EDITOR
            // Add midgame ads debugger for easy testing
            var debuggerGO = new GameObject("MidgameAdsDebugger");
            debuggerGO.AddComponent<Project.Game.Levels.MidgameAdsDebugger>();
            Debug.Log("[GameController] MidgameAdsDebugger added to scene. Press F2 to force ads!");
#endif
        }

        private IEnumerator Start()
        {
            _playerHealthController.Initialize();

            var hasInProgressSession =
                _inProgressSession != null &&
                _inProgressSession.InProgressSave != null &&
                _inProgressSession.InProgressSave.InProgress;

            var levelIndex = Mathf.Max(0, _sessionInfo != null ? _sessionInfo.LevelIndex : 0);
            var waveIndex = 0;

            if (hasInProgressSession)
            {
                LoadInProgressSession(out levelIndex, out waveIndex);
            }

            _levelController.Initialize(levelIndex, waveIndex);

            _uiFrame.Open<HudScreen>();
            _uiFrame.Open<DamageOverlayScreen>();

            yield return null;

            _playerInput.Hide(true);

            // No Poki signal here. Start is fired on PLAY (MainMenu).

            if (!hasInProgressSession)
            {
                OpenWeaponSelector();
            }
            else
            {
                StartLevel();
            }
        }

        private void OpenWeaponSelector()
        {
            var weaponSelector = _uiFrame.Open<RandomItemSelectorScreen>();
            weaponSelector.Initialize(_heroInfo.Data.StartingWeaponDatabase == null
                ? _weaponDatabase
                : _heroInfo.Data.StartingWeaponDatabase);
            weaponSelector.OnCloseEvent += OnWeaponSelectorClosed;
            // Do NOT stop here — you want the run to remain "playing" after PLAY.
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
            // ★ Hide/stop the hint while out of combat (optional but recommended)
            if (_idleHint != null) _idleHint.End();

            menuMusic.Play();

            _cameraManager.ZoomIn();
            _enemyManager.EndWave();

            _playerController.HandleWaveComplete();
            _playerInput.Hide();

            // No stop between waves.

            var currencyReward = ApplySoftCurrencyReward();

            var waveCompleteScreen = _uiFrame.Open<WaveCompleteScreen>();
            waveCompleteScreen.Initialize(currencyReward, false, _levelController.WaveIndex);
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
            // No stop here.
        }

        private void OnWaveRewardsClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnWaveRewardsClosed;

            var upgradeSelector = _uiFrame.Open<StatUpgradeSelectorScreen>();
            upgradeSelector.Initialize();
            upgradeSelector.OnCloseEvent += OnUpgradeSelectorClosed;
            // No stop here.
        }

        private void OnShopClosed(UIScreen screen)
        {
            screen.OnCloseEvent -= OnShopClosed;

            if (_levelController.WaveIndex > 0 && _inProgressSession != null)
            {
                _inProgressSession.SaveProgress(
                    _sessionInfo != null ? _sessionInfo.LevelIndex : 0,
                    _levelController.WaveIndex,
                    _playerHealthController.CurrentHealth,
                    _heroInfo);
                _saveManager.Save();
            }

            _playerController.transform.position = Vector3.zero;
            _playerController.enabled = true;

            BeginNextWave();
        }

        private void BeginNextWave()
        {
            _playerInput.Show();

            waveMusic.Play();

            var waveIntroScreen = _uiFrame.Open<WaveIntroScreen>();
            waveIntroScreen.OnCloseEvent += OnWaveIntroCompleted;
            waveIntroScreen.DisplayWithWaveIndex(
                _levelController.WaveIndex,
                _levelController.WaveCount,
                _levelController.IsFinalWave);

            if (_levelController.IsFinalWave)
            {
                _uiFrame.Get<HudScreen>().ShowCurrencyBar();
            }

            _cameraManager.ZoomOut();
            // No stop during intros.
        }

        private void OnWaveIntroCompleted(UIScreen waveIntroScreen)
{
    waveIntroScreen.OnCloseEvent -= OnWaveIntroCompleted;

    // Show movement hint immediately when wave intro closes
    _idleHint?.Begin();

    // Instead of starting the wave right away, wait until the player moves
    StartCoroutine(WaitForMoveThenStartWave());
}

private void StartWaveGameplay()
{
    _levelController.BeginNextWave(0.375f);
    _enemyManager.BeginWave(
        _levelController.CurrentLevel,
        _levelController.CurrentLevelIndex,
        _levelController.WaveIndex);

            CrazySdkManager.GameplayStart();
}

private System.Collections.IEnumerator WaitForMoveThenStartWave()
{
    Vector3 lastPos = _playerController.transform.position;

    while (true)
    {
        yield return null;

        if (HasMovementInput() || HasMovedSince(ref lastPos))
            break;
    }

    StartWaveGameplay();
}

private bool HasMovedSince(ref Vector3 lastPos)
{
    Vector3 pos = _playerController.transform.position;
    float distSqr = (pos - lastPos).sqrMagnitude;
    lastPos = pos;
    return distSqr > 0.0004f; // tweak threshold to match your scale
}

private bool HasMovementInput()
{
    if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f) return true;
    if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f) return true;

    if (Input.anyKeyDown) return true;
    if (Input.GetMouseButton(0)) return true;
    if (Input.touchCount > 0) return true;

    return false;
}

        private void OnLevelCompleted()
        {
            // Stop only when the level/run actually ends (win)
            CrazySdkManager.GameplayStop();

            if (_inProgressSession != null)
            {
                _inProgressSession.ClearProgress();
            }

            if (_levelController.CurrentLevelIndex >= _playerInfo.PlayerSave.TopStageIndex)
            {
                _playerInfo.IncrementTopStage();
            }
            _saveManager.Save();

            SendLevelSummaryEvent(true);

            menuMusic.Play();

            _cameraManager.ZoomIn();
            _enemyManager.EndWave();

            _playerController.enabled = false;
            _playerController.HandleWaveComplete();
            _playerInput.Hide();

            // ★ ensure hint is off on win screen
            if (_idleHint != null) _idleHint.End();

            var currencyReward = ApplySoftCurrencyReward();

            var levelCompleteScreen = _uiFrame.Open<WaveCompleteScreen>();
            levelCompleteScreen.Initialize(currencyReward, true);
            levelCompleteScreen.OnCloseEvent += OnLevelCompleteClosed;
        }

        // No-op now that analytics are removed
        private void SendLevelSummaryEvent(bool completed) { }

        private void OnLevelCompleteClosed(UIScreen screen)
        {
            LoadMainMenu();
        }

        private void OnPlayerHealthDepleted()
        {
            // Stop only when the level/run actually ends (fail)
            CrazySdkManager.GameplayStop();

            if (_inProgressSession != null)
            {
                _inProgressSession.ClearProgress();
            }
            _saveManager.Save();

            SendLevelSummaryEvent(false);

            _enemyManager.EndWaveAfterDelay(0.5f);

            menuMusic.Play();

            _playerHealthController.Depleted -= OnPlayerHealthDepleted;
            _levelController.WaveCompleted -= OnWaveCompleted;
            _levelController.LevelCompleted -= OnLevelCompleted;

            _playerController.enabled = false;
            _playerController.gameObject.SetActive(false);
            _playerInput.Hide();

            // ★ ensure hint is off on fail screen
            if (_idleHint != null) _idleHint.End();

            var levelFailedScreen = _uiFrame.Open<LevelFailedScreen>();
            levelFailedScreen.ConfirmButtonClicked += OnLevelFailConfirmed;
            levelFailedScreen.ReviveButtonClicked += OnReviveButtonClicked;
        }

        private void OnLevelFailConfirmed()
        {
            LoadMainMenu();
        }

        private void OnReviveButtonClicked()
        {
            Debug.Log("[GameController] Player used revive - continuing current wave");
            
            // Close the failed screen
            _uiFrame.Close<LevelFailedScreen>();
            
            // Reactivate and re-enable the player controller
            _playerController.gameObject.SetActive(true);
            _playerController.enabled = true;
            
            // Fix weapon animation states after GameObject reactivation
            ResetWeaponAnimationStates();
            
            // Restore player health to full
            _playerHealthController.CurrentHealth = _playerHealthController.MaxHealth;
            Debug.Log($"[GameController] Player health restored to {_playerHealthController.CurrentHealth}/{_playerHealthController.MaxHealth}");
            
            // Re-subscribe to health events for the new attempt
            _playerHealthController.Depleted += OnPlayerHealthDepleted;
            _levelController.WaveCompleted += OnWaveCompleted;
            _levelController.LevelCompleted += OnLevelCompleted;
            
            // Clean up any remaining enemies from the failed attempt
            _enemyManager.EndWave();
            
            // Continue the current wave (timer keeps running)
            _levelController.ContinueCurrentWave();
            
            // Restart enemy spawning for the current wave
            _enemyManager.BeginWave(
                _levelController.CurrentLevel,
                _levelController.CurrentLevelIndex,
                _levelController.WaveIndex);
            
            // Re-enable player input
            _playerInput.Show();
            
            Debug.Log("[GameController] Player revived successfully! Timer continues, enemies respawned, weapons reset!");
        }

        private void ResetWeaponAnimationStates()
        {
            // Find all weapon animation components and reset their states
            var weaponAnimations = _playerController.GetComponentsInChildren<Animation>();
            foreach (var animation in weaponAnimations)
            {
                if (animation != null)
                {
                    // Stop any ongoing animations and reset to default state
                    animation.Stop();
                    animation.Rewind();
                    Debug.Log($"[GameController] Reset animation state for weapon: {animation.gameObject.name}");
                }
            }
        }

        private void LoadMainMenu()
        {
            // Safety net before leaving the scene
            CrazySdkManager.GameplayStop();
            _sceneLoader.LoadScene("main_menu", 0.2f, 0.5f);
        }

        private void LoadInProgressSession(out int levelIndex, out int waveIndex)
        {
            levelIndex = 0;
            waveIndex = 0;

            if (_inProgressSession == null || _inProgressSession.InProgressSave == null)
            {
                Debug.LogWarning("[GameController] No in-progress save; starting fresh.");
                _playerHealthController.CurrentHealth = _playerHealthController.MaxHealth;
                return;
            }

            var save = _inProgressSession.InProgressSave;

            levelIndex = Mathf.Max(0, save.LevelIndex);
            waveIndex = Mathf.Max(0, save.WaveIndex);

            _playerHealthController.CurrentHealth = _playerHealthController.MaxHealth;

            // Load items & stat upgrades
            if (save.Items != null)
            {
                foreach (var itemId in save.Items)
                {
                    var itemData = _itemDatabase.GetItemWithId(itemId);
                    if (itemData != null)
                    {
                        _heroInfo.AddItem(itemData as ItemData);
                        continue;
                    }

                    var statUpgradeData = _statUpgradeDatabase.GetItemWithId(itemId);
                    if (statUpgradeData != null)
                    {
                        _heroInfo.AddItem(statUpgradeData as ItemData);
                    }
                }
            }

            // Load weapons
            if (save.Weapons != null)
            {
                foreach (var weaponId in save.Weapons)
                {
                    var weaponData = _weaponDatabase.GetItemWithId(weaponId);
                    if (weaponData != null)
                    {
                        _heroInfo.AddWeapon(weaponData as WeaponData);
                    }
                }
            }

            _playerHealthController.CurrentHealth = Mathf.Max(1, save.Health);
            _heroInfo.ShopCurrency = Mathf.Max(0, save.ShopCurrency);
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

        // Pause/focus hygiene for WebGL
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) CrazySdkManager.GameplayStop();
        }
        private void OnApplicationPause(bool paused)
        {
            if (paused) CrazySdkManager.GameplayStop();
        }
    }
}
