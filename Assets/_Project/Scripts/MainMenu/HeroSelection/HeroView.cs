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
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image heroImage;
        [SerializeField] private FeedbackData heroLevelUpFeedback;

        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly SaveManager _saveManager;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly PlayerInfo _playerInfo;

        private HeroInfo _heroInfo;

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
            if (HeroUpgradeAdsTracker.CanUseAdUpgrade)
            {
                PerformRewardedAdUpgrade();
            }
            else
            {
                Debug.Log("[HeroView] Cannot use ad upgrade - already used this session");
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
            
            // Check if player can use ad upgrade
            if (!HeroUpgradeAdsTracker.CanUseAdUpgrade)
            {
                Debug.Log("[HeroView] Cannot use ad upgrade - already used this session");
                return;
            }
            
            // Disable button during ad
            adUpgradeButton.interactable = false;
            
            // Show loading state on ad button
            if (adUpgradeText != null)
            {
                adUpgradeText.text = "LOADING...";
            }
            
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    Debug.Log("=== [HeroView] AD CALLBACK TRIGGERED - Starting hero upgrade process ===");
                    Debug.Log("[HeroView] Rewarded ad completed - upgrading hero for free!");
                    
                    try 
                    {
                        var oldLevel = _heroRegistry.ActiveHero.Level;
                        Debug.Log($"[HeroView] Hero level before upgrade: {oldLevel}");
                        Debug.Log($"[HeroView] Active hero: {_heroRegistry.ActiveHero.Data.Id}");
                        Debug.Log($"[HeroView] Hero registry state before upgrade: {_heroRegistry.ActiveHero.Data.name}");
                        
                        heroLevelUpFeedback.Play(heroImage.transform.position, Quaternion.identity);
                        
                        Debug.Log("[HeroView] About to call _heroRegistry.UpgradeActiveHero()");
                        _heroRegistry.UpgradeActiveHero();
                        Debug.Log("[HeroView] _heroRegistry.UpgradeActiveHero() completed");
                    
                    var newLevel = _heroRegistry.ActiveHero.Level;
                    Debug.Log($"[HeroView] Hero level after upgrade: {newLevel}");
                    
                    // Check what's actually in the save data
                    var heroId = _heroRegistry.ActiveHero.Data.Id;
                    var savedLevel = ((HeroSave)_heroRegistry.Save).GetHeroLevel(heroId);
                    Debug.Log($"[HeroView] Hero level in save data: {savedLevel} for hero ID: {heroId}");
                    
                    Debug.Log($"[HeroView] Save system login status: {SaveSystemIntegration.IsUserLoggedIn}, SDK ready: {SaveSystemIntegration.IsCrazySDKReady()}");
                    
                    // Force the OnChanged event to ensure HeroRegistry is marked as dirty
                    Debug.Log("[HeroView] Forcing HeroRegistry to trigger OnChanged event");
                    _heroRegistry.ForceSaveUpdate();
                    
                    // Save immediately and also after a short delay to ensure persistence
                    _saveManager.Save();
                    Debug.Log("[HeroView] Immediate hero upgrade save completed!");
                    
                    // Also save after a short delay to ensure cloud sync
                    StartCoroutine(DelayedSave());
                    
                    // Mark ad upgrade as used for this session
                    HeroUpgradeAdsTracker.MarkAdUpgradeUsed();
                    
                    // Update UI after upgrade
                    UpdateUpgradeButtons();
                    
                    Debug.Log("=== [HeroView] AD UPGRADE PROCESS COMPLETED SUCCESSFULLY ===");
                }
                catch (System.Exception ex) 
                {
                    Debug.LogError($"[HeroView] Exception during hero upgrade: {ex.Message}");
                    Debug.LogError($"[HeroView] Stack trace: {ex.StackTrace}");
                    // Restore button state on error
                    UpdateUpgradeButtons();
                }
            },
                onAdFailed: () => {
                    Debug.Log("[HeroView] Rewarded ad failed - restoring button state");
                    // Restore button state
                    UpdateUpgradeButtons();
                }
            );
        }

        private System.Collections.IEnumerator DelayedSave()
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("[HeroView] Performing delayed save to ensure cloud persistence");
            
            // Force the OnChanged event to ensure HeroRegistry is marked as dirty  
            Debug.Log("[HeroView] Forcing HeroRegistry to trigger OnChanged event (delayed)");
            _heroRegistry.ForceSaveUpdate();
            
            _saveManager.Save();
            Debug.Log("[HeroView] Delayed hero upgrade save completed!");
        }

        public void OnReady()
        {
            // Initialize the ads tracker with required dependencies
            HeroUpgradeAdsTracker.Initialize(_playerInfo, _saveManager);
            
            _heroRegistry.ActiveHeroChanged += OnActiveHeroChanged;
            OnActiveHeroChanged(_heroRegistry.ActiveHero);
            
            _playerInfo.OnCurrencyChanged += OnCurrencyChanged;
            _playerInfo.OnHeroAdUpgradeStateChanged += OnHeroAdUpgradeStateChanged;
            OnCurrencyChanged();
        }
        

        private void OnCurrencyChanged()
        {
            UpdateUpgradeButtons();
        }
        
        private void OnHeroAdUpgradeStateChanged()
        {
            UpdateUpgradeButtons();
        }

        private void UpdateUpgradeButtons()
        {
            var nextUpgradeCost = _gameData.GetUpgradeCost(_heroRegistry.ActiveHero.Level);
            var hasEnoughCurrency = _playerInfo.PlayerSave.Currency >= nextUpgradeCost;
            var canUseAdUpgrade = HeroUpgradeAdsTracker.CanUseAdUpgrade;

            // Update currency upgrade button
            currencyUpgradeButton.interactable = hasEnoughCurrency;
            currencyUpgradeCostText.text = $"{nextUpgradeCost}";
            currencyUpgradeCostText.color = hasEnoughCurrency ? Color.white : Color.red;

            // Update ad upgrade button
            adUpgradeButton.interactable = canUseAdUpgrade;
            
            // Update ad button text based on availability
            if (adUpgradeText != null)
            {
                adUpgradeText.text = canUseAdUpgrade ? "UPGRADE WITH AD" : "AD USED";
                adUpgradeText.color = canUseAdUpgrade ? Color.white : Color.gray;
            }
        }

        private void OnSelectButtonClicked()
        {
            _heroRegistry.SetSelectedHero(_heroInfo.Data.name);
            Selected?.Invoke(_heroInfo.Data);
        }

        /// <summary>
        /// Test method to verify hero upgrade logic works (temporary debugging)
        /// </summary>
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

        private void OnDestroy()
        {
            _heroRegistry.ActiveHeroChanged -= OnActiveHeroChanged;
            _playerInfo.OnCurrencyChanged -= OnCurrencyChanged;
            _playerInfo.OnHeroAdUpgradeStateChanged -= OnHeroAdUpgradeStateChanged;
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