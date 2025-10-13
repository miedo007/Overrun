using System;
using System.Collections;
using Mtl.Injection;
using Mtl.Save;
using Project.Application;
using Project.Feedback;
using Project.Game;
using Project.Game.Levels;
using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.MainMenu.HeroSelection
{
    public class HeroView : MonoBehaviour, IInjectionReady
    {
        public event Action<HeroData> Selected;
        
        [SerializeField] private Button selectButton;
        [SerializeField] private Button currencyUpgradeButton;
        [SerializeField] private Button adUpgradeButton;
        [SerializeField] private TextMeshProUGUI currencyUpgradeCostText;
        [SerializeField] private TextMeshProUGUI adUpgradeText;
        [SerializeField] private Image adUpgradeIcon;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image heroImage;
        [SerializeField] private FeedbackData heroLevelUpFeedback;

        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly SaveManager _saveManager;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly PlayerInfo _playerInfo;

        private HeroInfo _heroInfo;
        private Coroutine _cooldownCheckCoroutine;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
            currencyUpgradeButton.onClick.AddListener(OnCurrencyUpgradeButtonClicked);
            adUpgradeButton.onClick.AddListener(OnAdUpgradeButtonClicked);
        }

        private void OnCurrencyUpgradeButtonClicked()
        {
            var nextUpgradeCost = _gameData.GetUpgradeCost(_heroRegistry.ActiveHero.Level);
            var hasEnoughCurrency = _playerInfo.PlayerSave.Currency >= nextUpgradeCost;

            if (hasEnoughCurrency)
            {
                PerformCurrencyUpgrade(nextUpgradeCost);
            }
            else
            {
                Debug.Log("[HeroView] Not enough currency for upgrade");
            }
        }

        private void OnAdUpgradeButtonClicked()
        {
            if (HeroAdUpgradeTimer.CanUseAdUpgrade)
            {
                PerformRewardedAdUpgrade();
            }
            else
            {
                var remainingTime = HeroAdUpgradeTimer.GetRemainingCooldownTime();
                Debug.Log($"[HeroView] Cannot use ad upgrade - cooldown active for {remainingTime:F0} more seconds");
            }
        }

        private void PerformCurrencyUpgrade(int upgradeCost)
        {
            Debug.Log($"[HeroView] Upgrading hero with currency: {upgradeCost}");
            heroLevelUpFeedback.Play(heroImage.transform.position, Quaternion.identity);
            _heroRegistry.UpgradeActiveHero();
            _playerInfo.ChangeCurrency(-upgradeCost);
            _saveManager.Save();
        }

        private void PerformRewardedAdUpgrade()
        {
            Debug.Log("[HeroView] Attempting rewarded ad upgrade");
            
            adUpgradeButton.interactable = false;
            
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    Debug.Log("[HeroView] Rewarded ad completed - upgrading hero for free!");
                    
                    try 
                    {
                        heroLevelUpFeedback.Play(heroImage.transform.position, Quaternion.identity);
                        _heroRegistry.UpgradeActiveHero();
                        _saveManager.Save();
                        
                        HeroAdUpgradeTimer.StartCooldown();
                        StartCooldownCheckIfNeeded();
                        UpdateUpgradeButtons();
                        
                        Debug.Log("[HeroView] Ad upgrade completed successfully");
                    }
                    catch (System.Exception ex) 
                    {
                        Debug.LogError($"[HeroView] Exception during hero upgrade: {ex.Message}");
                        UpdateUpgradeButtons();
                    }
                },
                onAdFailed: () => {
                    Debug.Log("[HeroView] Rewarded ad failed - restoring button state");
                    UpdateUpgradeButtons();
                }
            );
        }

        public void OnReady()
        {
            HeroAdUpgradeTimer.Initialize(_playerInfo, _saveManager);
            HeroAdUpgradeTimer.OnDurationChanged += OnTimerDurationChanged;
            
            var currentDuration = HeroAdUpgradeTimer.CooldownDuration;
            Debug.Log($"[HeroView] Timer initialized with duration: {currentDuration} seconds ({currentDuration/60f:F1} minutes)");
            
            _heroRegistry.ActiveHeroChanged += OnActiveHeroChanged;
            OnActiveHeroChanged(_heroRegistry.ActiveHero);
            
            _playerInfo.OnCurrencyChanged += OnCurrencyChanged;
            OnCurrencyChanged();
            
            // Start checking for cooldown expiry if needed
            StartCooldownCheckIfNeeded();
        }
        

        private void OnTimerDurationChanged()
        {
            Debug.Log($"[HeroView] Timer duration changed to {HeroAdUpgradeTimer.CooldownDuration}s - refreshing UI");
            StartCooldownCheckIfNeeded();
            UpdateUpgradeButtons();
        }

        private void OnCurrencyChanged()
        {
            UpdateUpgradeButtons();
        }

        private void UpdateUpgradeButtons()
        {
            var nextUpgradeCost = _gameData.GetUpgradeCost(_heroRegistry.ActiveHero.Level);
            var hasEnoughCurrency = _playerInfo.PlayerSave.Currency >= nextUpgradeCost;
            var canUseAdUpgrade = HeroAdUpgradeTimer.CanUseAdUpgrade;

            // Update currency upgrade button
            currencyUpgradeButton.interactable = hasEnoughCurrency;
            currencyUpgradeCostText.text = $"{nextUpgradeCost}";
            currencyUpgradeCostText.color = hasEnoughCurrency ? Color.white : Color.red;

            // Hide entire ad button during cooldown, show when available
            adUpgradeButton.gameObject.SetActive(canUseAdUpgrade);
            
            if (canUseAdUpgrade)
            {
                adUpgradeButton.interactable = true;
                
                // Ensure icon and text are visible when button is available
                if (adUpgradeIcon != null)
                {
                    adUpgradeIcon.enabled = true;
                    adUpgradeIcon.color = Color.white;
                }
                
                if (adUpgradeText != null)
                {
                    adUpgradeText.enabled = true;
                    adUpgradeText.color = Color.white;
                }
            }
            
            Debug.Log($"[HeroView] Ad button visibility: {canUseAdUpgrade}");
        }

        private void StartCooldownCheckIfNeeded()
        {
            // Stop any existing coroutine
            if (_cooldownCheckCoroutine != null)
            {
                StopCoroutine(_cooldownCheckCoroutine);
                _cooldownCheckCoroutine = null;
            }
            
            // Only start checking if there's an active cooldown
            var canUseAdUpgrade = HeroAdUpgradeTimer.CanUseAdUpgrade;
            var remainingTime = HeroAdUpgradeTimer.GetRemainingCooldownTime();
            
            Debug.Log($"[HeroView] Cooldown check - CanUse: {canUseAdUpgrade}, Remaining: {remainingTime:F1}s");
            
            if (!canUseAdUpgrade && remainingTime > 0)
            {
                Debug.Log($"[HeroView] Starting cooldown check - {remainingTime:F0} seconds remaining");
                _cooldownCheckCoroutine = StartCoroutine(CooldownCheckCoroutine());
            }
            else
            {
                Debug.Log("[HeroView] No cooldown check needed - button should be available");
            }
        }
        
        private IEnumerator CooldownCheckCoroutine()
        {
            Debug.Log("[HeroView] Cooldown check coroutine started");
            
            while (true)
            {
                var canUseAdUpgrade = HeroAdUpgradeTimer.CanUseAdUpgrade;
                var remainingTime = HeroAdUpgradeTimer.GetRemainingCooldownTime();
                
                Debug.Log($"[HeroView] Cooldown check tick - CanUse: {canUseAdUpgrade}, Remaining: {remainingTime:F1}s");
                
                if (canUseAdUpgrade)
                {
                    Debug.Log("[HeroView] Cooldown expired - showing ad button and stopping coroutine");
                    UpdateUpgradeButtons();
                    _cooldownCheckCoroutine = null;
                    yield break;
                }
                
                yield return new WaitForSeconds(1f);
            }
        }

        private void OnSelectButtonClicked()
        {
            _heroRegistry.SetSelectedHero(_heroInfo.Data.name);
            Selected?.Invoke(_heroInfo.Data);
        }

        [UnityEngine.ContextMenu("Test Hero Upgrade")]
        public void TestHeroUpgrade()
        {
            Debug.Log("=== [HeroView] TESTING HERO UPGRADE (NON-AD) ===");
            
            var oldLevel = _heroRegistry.ActiveHero.Level;
            var heroId = _heroRegistry.ActiveHero.Data.Id;
            var oldSaveLevel = ((HeroSave)_heroRegistry.Save).GetHeroLevel(heroId);
            
            Debug.Log($"[HeroView] Before upgrade - UI Level: {oldLevel}, Save Level: {oldSaveLevel}, Hero ID: {heroId}");
            
            _heroRegistry.UpgradeActiveHero();
            
            var newLevel = _heroRegistry.ActiveHero.Level;
            var newSaveLevel = ((HeroSave)_heroRegistry.Save).GetHeroLevel(heroId);
            
            Debug.Log($"[HeroView] After upgrade - UI Level: {newLevel}, Save Level: {newSaveLevel}");
            
            _saveManager.Save();
            Debug.Log("[HeroView] Test upgrade saved!");
        }

        [UnityEngine.ContextMenu("Test Ad Cooldown")]
        public void TestAdCooldown()
        {
            Debug.Log("[HeroView] Starting test ad cooldown");
            HeroAdUpgradeTimer.StartCooldown();
            StartCooldownCheckIfNeeded();
            UpdateUpgradeButtons();
        }

        [UnityEngine.ContextMenu("Reset Ad Cooldown")]
        public void ResetAdCooldown()
        {
            Debug.Log("[HeroView] Resetting ad cooldown");
            _playerInfo.ChangeHeroAdUpgradeTime(0);
            _saveManager.Save();
            
            // Stop cooldown checking since cooldown is reset
            if (_cooldownCheckCoroutine != null)
            {
                StopCoroutine(_cooldownCheckCoroutine);
                _cooldownCheckCoroutine = null;
            }
            
            UpdateUpgradeButtons();
        }

        [UnityEngine.ContextMenu("Check Timer Status")]
        public void CheckTimerStatus()
        {
            var canUse = HeroAdUpgradeTimer.CanUseAdUpgrade;
            var remaining = HeroAdUpgradeTimer.GetRemainingCooldownTime();
            var lastAdTime = _playerInfo.PlayerSave.LastHeroAdUpgradeTime;
            var duration = HeroAdUpgradeTimer.CooldownDuration;
            
            Debug.Log($"[HeroView] === TIMER STATUS ===");
            Debug.Log($"[HeroView] Can Use Ad: {canUse}");
            Debug.Log($"[HeroView] Remaining Time: {remaining:F1}s");
            Debug.Log($"[HeroView] Last Ad Time: {lastAdTime}");
            Debug.Log($"[HeroView] Duration: {duration}s");
            Debug.Log($"[HeroView] Button Active: {adUpgradeButton.gameObject.activeSelf}");
            Debug.Log($"[HeroView] Coroutine Running: {_cooldownCheckCoroutine != null}");
        }

        [UnityEngine.ContextMenu("Force Refresh UI")]
        public void ForceRefreshUI()
        {
            Debug.Log("[HeroView] Force refreshing UI");
            StartCooldownCheckIfNeeded();
            UpdateUpgradeButtons();
        }

        private void OnDestroy()
        {
            _heroRegistry.ActiveHeroChanged -= OnActiveHeroChanged;
            _playerInfo.OnCurrencyChanged -= OnCurrencyChanged;
            HeroAdUpgradeTimer.OnDurationChanged -= OnTimerDurationChanged;
            
            // Clean up cooldown check coroutine
            if (_cooldownCheckCoroutine != null)
            {
                StopCoroutine(_cooldownCheckCoroutine);
                _cooldownCheckCoroutine = null;
            }
        }

        private void OnActiveHeroChanged(HeroInfo heroInfo)
        {
            _heroInfo = heroInfo;
            levelText.text = $"LEVEL {heroInfo.Level + 1}"; 
            heroImage.sprite = _heroInfo.Data.Sprite;
            UpdateUpgradeButtons();
        }
    }
}