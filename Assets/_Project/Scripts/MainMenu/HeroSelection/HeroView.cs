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
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image heroImage;
        [SerializeField] private FeedbackData heroLevelUpFeedback;

        [Header("Currency/Ad UI Elements")]
        [SerializeField] private GameObject coinImageObject;  // GameObject containing coin icon
        [SerializeField] private GameObject adIconObject;     // GameObject containing ad icon

        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly SaveManager _saveManager;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly PlayerInfo _playerInfo;

        private HeroInfo _heroInfo;
        private bool _isShowingRewardedAdOption = false;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }

        private void OnUpgradeButtonClicked()
        {
            var nextUpgradeCost = _gameData.GetUpgradeCost(_heroRegistry.ActiveHero.Level);
            var hasEnoughCurrency = _playerInfo.PlayerSave.Currency >= nextUpgradeCost;

            if (hasEnoughCurrency)
            {
                // Normal currency upgrade
                PerformCurrencyUpgrade(nextUpgradeCost);
            }
            else if (_isShowingRewardedAdOption)
            {
                // Rewarded ad upgrade
                PerformRewardedAdUpgrade();
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
            upgradeButton.interactable = false;
            
            // Show loading state - hide both icons, show loading text
            if (coinImageObject != null) coinImageObject.SetActive(false);
            if (adIconObject != null) adIconObject.SetActive(false);
            upgradeCostText.text = "LOADING...";
            upgradeCostText.gameObject.SetActive(true);
            
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
                    UpdateUpgradeButton();
                    
                    Debug.Log("=== [HeroView] AD UPGRADE PROCESS COMPLETED SUCCESSFULLY ===");
                }
                catch (System.Exception ex) 
                {
                    Debug.LogError($"[HeroView] Exception during hero upgrade: {ex.Message}");
                    Debug.LogError($"[HeroView] Stack trace: {ex.StackTrace}");
                    // Restore button state on error
                    UpdateUpgradeButton();
                }
            },
                onAdFailed: () => {
                    Debug.Log("[HeroView] Rewarded ad failed - restoring button state");
                    // Restore button state
                    UpdateUpgradeButton();
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
            UpdateUpgradeButton();
        }
        
        private void OnHeroAdUpgradeStateChanged()
        {
            UpdateUpgradeButton();
        }

        private void UpdateUpgradeButton()
        {
            var nextUpgradeCost = _gameData.GetUpgradeCost(_heroRegistry.ActiveHero.Level);
            var hasEnoughCurrency = _playerInfo.PlayerSave.Currency >= nextUpgradeCost;
            var canUseAdUpgrade = HeroUpgradeAdsTracker.CanUseAdUpgrade;

            if (hasEnoughCurrency)
            {
                // Show normal upgrade with currency cost
                _isShowingRewardedAdOption = false;
                upgradeButton.interactable = true;
                
                // Show coin icon and cost text, hide ad icon
                if (coinImageObject != null) coinImageObject.SetActive(true);
                if (adIconObject != null) adIconObject.SetActive(false);
                upgradeCostText.text = $"{nextUpgradeCost}";
                upgradeCostText.color = Color.white;
                upgradeCostText.gameObject.SetActive(true);
            }
            else if (canUseAdUpgrade)
            {
                // Show rewarded ad upgrade option
                _isShowingRewardedAdOption = true;
                upgradeButton.interactable = true;
                
                // Show ad icon, hide coin icon and cost text
                if (coinImageObject != null) coinImageObject.SetActive(false);
                if (adIconObject != null) adIconObject.SetActive(true);
                upgradeCostText.gameObject.SetActive(false); // Hide text when showing ad icon
            }
            else
            {
                // Player has already used ad upgrade this session and doesn't have currency
                _isShowingRewardedAdOption = false;
                upgradeButton.interactable = false;
                
                // Show coin icon and cost in red to indicate insufficient funds
                if (coinImageObject != null) coinImageObject.SetActive(true);
                if (adIconObject != null) adIconObject.SetActive(false);
                upgradeCostText.text = $"{nextUpgradeCost}";
                upgradeCostText.color = Color.red;
                upgradeCostText.gameObject.SetActive(true);
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
            UpdateUpgradeButton();
        }
    }
}