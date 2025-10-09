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

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
            currencyUpgradeButton.onClick.AddListener(OnCurrencyUpgradeButtonClicked);
            adUpgradeButton.onClick.AddListener(OnAdUpgradeButtonClicked);
        }

        private void Update()
        {
            // No timer updates needed - ad upgrades are always available
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
            // Always allow ad upgrade - no cooldown check needed
            PerformRewardedAdUpgrade();
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
            
            // Disable button during ad
            adUpgradeButton.interactable = false;
            
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    Debug.Log("[HeroView] Rewarded ad completed - upgrading hero for free!");
                    
                    try 
                    {
                        heroLevelUpFeedback.Play(heroImage.transform.position, Quaternion.identity);
                        _heroRegistry.UpgradeActiveHero();
                        _saveManager.Save();
                        
                        // Update UI after upgrade
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
            _heroRegistry.ActiveHeroChanged += OnActiveHeroChanged;
            OnActiveHeroChanged(_heroRegistry.ActiveHero);
            
            _playerInfo.OnCurrencyChanged += OnCurrencyChanged;
            OnCurrencyChanged();
        }
        

        private void OnCurrencyChanged()
        {
            UpdateUpgradeButtons();
        }

        private void UpdateUpgradeButtons()
        {
            var nextUpgradeCost = _gameData.GetUpgradeCost(_heroRegistry.ActiveHero.Level);
            var hasEnoughCurrency = _playerInfo.PlayerSave.Currency >= nextUpgradeCost;

            // Update currency upgrade button
            currencyUpgradeButton.interactable = hasEnoughCurrency;
            currencyUpgradeCostText.text = $"{nextUpgradeCost}";
            currencyUpgradeCostText.color = hasEnoughCurrency ? Color.white : Color.red;

            // Ad upgrade button is always available
            adUpgradeButton.interactable = true;
            
            // Hide ad button text - icon only
            if (adUpgradeText != null)
            {
                adUpgradeText.text = "";
                adUpgradeText.enabled = false;
            }
            
            // Ad icon is always enabled
            if (adUpgradeIcon != null)
            {
                adUpgradeIcon.color = Color.white;
                adUpgradeIcon.enabled = true;
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