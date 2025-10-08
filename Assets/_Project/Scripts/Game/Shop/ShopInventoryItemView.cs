using System;
using DG.Tweening;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Levels;
using Project.Game.Tutorials;
using Project.Game.Weapons;
using Project.Heroes;
using Project.Tiers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopInventoryItemView : MonoBehaviour
    {
        public event Action<ShopInventoryItemView> BuyButtonClicked;
        public event Action<ShopInventoryItemView> RewardedAdBuyClicked;
        public event Action<ShopInventoryItemView, bool> LockedStateChanged;
        
        [field: SerializeField] public  CanvasGroup CanvasGroup { get; private set; }
        [field: SerializeField] public  Image Icon { get; private set; }
        [field: SerializeField] public  Image IconBacker { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI NameField { get; private set; }
        [field: SerializeField] public InfoViewWeapon WeaponInfoView;
        [field: SerializeField] public InfoViewItem ItemInfoView;
        [field: SerializeField] public  Button BuyButton { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI CostText { get; private set; }
        [field: SerializeField] public  LayoutElement LayoutElement { get; private set; }
        [field: SerializeField] public  MergeableNotification MergeableNotification { get; private set; }
        [field: SerializeField] public  LockToggle LockToggle { get; private set; }

        [Header("Currency/Ad UI Elements")]
        [SerializeField] private GameObject coinImageObject;  // GameObject containing coin icon
        [SerializeField] private GameObject adIconObject;     // GameObject containing ad icon

        [Inject] private readonly TierDatabase _tierDatabase;
        [Inject] private readonly UIFrame _uiFrame;

        private bool _affordable;
        private bool _purchased;
        private int _cost;
        private string _customButtonText; // Store custom button text like "CHOOSE"
        private bool _isShowingRewardedAdOption = false;
        private bool _canUseRewardedAd = true; // Will be set by ShopScreen
        private bool _isAdExclusive = false; // Track if this item is ad-exclusive
        
        public BaseData Data { get; private set; }
        public int Cost => _cost;

        private HeroInfo _heroInfo;

        private void Awake()
        {
            if (BuyButton != null)
            {
                BuyButton.onClick.AddListener(OnBuyButtonClicked);
            }
        }

        public void Initialize(BaseData data, HeroInfo hero, int cost, string buyButtonString = "")
        {
            if (MergeableNotification != null)
            {
                MergeableNotification.Deactivate();
            }
            
            _cost = cost;
            _heroInfo = hero;
            _customButtonText = buyButtonString; // Store the custom button text
            Data = data;
            Icon.sprite = data.Sprite;
            IconBacker.color = data.Tier.Color;
            NameField.text = $"{data.DisplayName} {data.Tier.NamePostfix}";

            var isWeapon = data as WeaponData != null;
            
            WeaponInfoView.gameObject.SetActive(isWeapon);
            ItemInfoView.gameObject.SetActive(!isWeapon);
            
            if (isWeapon)
            {
                WeaponInfoView.Initialize(data);
            }
            else
            {
                ItemInfoView.Initialize(data);
            }

            if (BuyButton != null)
            {
                hero.ShopCurrencyChanged += OnShopCurrencyChanged;
                hero.WeaponsChanged += OnWeaponsChanged;
                OnShopCurrencyChanged();
                
                if (cost < 0)
                {
                    BuyButton.gameObject.SetActive(false);
                }
                else if (!string.IsNullOrEmpty(buyButtonString))
                {
                    // Custom button text provided - will be handled in UpdateBuyButtonDisplay
                    Debug.Log($"[ShopInventoryItemView] Custom button text set: '{buyButtonString}' for {data.DisplayName}");
                }
                else
                {
                    // Will be updated in UpdateBuyButtonDisplay()
                }
            }
            
            RefreshMergeableNotification();
        }

        public void SetRewardedAdAvailability(bool canUseRewardedAd)
        {
            _canUseRewardedAd = canUseRewardedAd;
            OnShopCurrencyChanged(); // Refresh button state
        }

        public void SetAdExclusive(bool isAdExclusive)
        {
            _isAdExclusive = isAdExclusive;
            OnShopCurrencyChanged(); // Refresh button state
        }
        
        private void OnDestroy()
        {
            if (_heroInfo != null)
            {
                _heroInfo.ShopCurrencyChanged -= OnShopCurrencyChanged;
                _heroInfo.WeaponsChanged -= OnWeaponsChanged;
            }
        }

        private void OnShopCurrencyChanged()
        {
            _affordable = _cost <= _heroInfo.GetShopCurrencyIntValue();
            UpdateBuyButtonDisplay();
            RefreshMergeableNotification();
        }

        private void UpdateBuyButtonDisplay()
        {
            if (_purchased) return;

            // If custom button text is provided (like "CHOOSE"), always use it
            if (!string.IsNullOrEmpty(_customButtonText))
            {
                _isShowingRewardedAdOption = false;
                BuyButton.interactable = true;
                
                // Hide both currency and ad icons for custom text
                if (coinImageObject != null) coinImageObject.SetActive(false);
                if (adIconObject != null) adIconObject.SetActive(false);
                CostText.text = _customButtonText;
                CostText.gameObject.SetActive(true);
                
                Debug.Log($"[ShopInventoryItemView] Using custom button text: '{_customButtonText}'");
                return;
            }

            // Check if this item is ad-exclusive
            if (_isAdExclusive)
            {
                // Ad-exclusive items can only be bought with ads
                if (_canUseRewardedAd)
                {
                    _isShowingRewardedAdOption = true;
                    BuyButton.interactable = true;
                    
                    // Show ad icon, hide coin icon and cost text
                    if (coinImageObject != null) coinImageObject.SetActive(false);
                    if (adIconObject != null) adIconObject.SetActive(true);
                    CostText.gameObject.SetActive(false);
                }
                else
                {
                    // Ad not available, disable button
                    _isShowingRewardedAdOption = false;
                    BuyButton.interactable = false;
                    
                    // Show ad icon grayed out
                    if (coinImageObject != null) coinImageObject.SetActive(false);
                    if (adIconObject != null) adIconObject.SetActive(true);
                    CostText.gameObject.SetActive(false);
                }
                return;
            }

            // Regular items - currency only (no ad option)
            var hasEnoughCurrency = _affordable;
            
            if (hasEnoughCurrency)
            {
                // Show normal purchase with currency
                _isShowingRewardedAdOption = false;
                BuyButton.interactable = true;
                
                // Show coin icon and cost text, hide ad icon
                if (coinImageObject != null) coinImageObject.SetActive(true);
                if (adIconObject != null) adIconObject.SetActive(false);
                CostText.text = $"<sprite name=currency_energy> {_cost}";
                CostText.gameObject.SetActive(true);
            }
            else
            {
                // Can't afford and no ad option available for regular items
                _isShowingRewardedAdOption = false;
                BuyButton.interactable = false;
                
                // Show grayed out currency display
                if (coinImageObject != null) coinImageObject.SetActive(true);
                if (adIconObject != null) adIconObject.SetActive(false);
                CostText.text = $"<sprite name=currency_energy> {_cost}";
                CostText.gameObject.SetActive(true);
            }
        }

        private bool CanBePurchasedWithAd()
        {
            // Only ad-exclusive items can be purchased with ads
            if (!_isAdExclusive) return false;
            
            var weaponData = Data as WeaponData;
            if (weaponData != null)
            {
                // Can buy weapon with ad if: has free slot OR can merge
                return _heroInfo.HasFreeWeaponSlot() || _heroInfo.CanMergeWeapon(weaponData, 1);
            }
            else
            {
                // Items can always be purchased (they don't have inventory limits)
                return true;
            }
        }

        private void OnWeaponsChanged()
        {
            RefreshMergeableNotification();
            OnShopCurrencyChanged(); // Refresh ad availability based on weapon slots
        }

        private void RefreshMergeableNotification()
        {
            if (MergeableNotification == null)
            {
                return;
            }

            if (_purchased)
            {
                MergeableNotification.Deactivate();
                return;
            }

            if (!_affordable && !_isShowingRewardedAdOption)
            {
                MergeableNotification.Deactivate();
                return;
            }

            var weaponData = Data as WeaponData;
            if (weaponData == null)
            {
                MergeableNotification.Deactivate();
                return;
            }
            
            if (!_heroInfo.HasFreeWeaponSlot() && _heroInfo.CanMergeWeapon(weaponData, 1))
            {
                MergeableNotification.Activate(_tierDatabase.GetNextTier(weaponData.Tier).Color);
                if (!PrefKeys.HasCompletedAutoMergeTutorial())
                {
                    if (!_uiFrame.Get<AutoMergeTutorialScreen>().IsOpened)
                    {
                        _uiFrame.Open<AutoMergeTutorialScreen>();
                    }
                }
            }
            else
            {
                MergeableNotification.Deactivate();
            }
        }

        private void OnBuyButtonClicked()
        {
            if (_isShowingRewardedAdOption)
            {
                // Handle rewarded ad purchase
                PerformRewardedAdPurchase();
            }
            else if (_heroInfo.GetShopCurrencyIntValue() >= _cost)
            {
                // Handle normal currency purchase
                LockToggle.SetState(false);
                BuyButtonClicked?.Invoke(this);
            }
        }

        private void PerformRewardedAdPurchase()
        {
            Debug.Log($"[ShopInventoryItemView] Attempting rewarded ad purchase for {Data.DisplayName}");
            
            // Disable button during ad
            BuyButton.interactable = false;
            
            // Show loading state - hide both icons, show loading text
            if (coinImageObject != null) coinImageObject.SetActive(false);
            if (adIconObject != null) adIconObject.SetActive(false);
            CostText.text = "LOADING...";
            CostText.gameObject.SetActive(true);
            
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    Debug.Log($"[ShopInventoryItemView] Rewarded ad completed - purchasing {Data.DisplayName} for free!");
                    LockToggle.SetState(false);
                    RewardedAdBuyClicked?.Invoke(this);
                },
                onAdFailed: () => {
                    Debug.Log("[ShopInventoryItemView] Rewarded ad failed - restoring button state");
                    // Restore button state
                    UpdateBuyButtonDisplay();
                }
            );
        }

        public void Purchase()
        {
            _purchased = true;
            BuyButton.gameObject.SetActive(false);
            CanvasGroup.interactable = false;
            CanvasGroup.DOFade(0, 0.125f)
                .OnComplete(ScaleDown);
        }

        private void ScaleDown()
        {
            LayoutElement.DOPreferredSize(Vector2.zero, 0.1f)
                .OnComplete(() => gameObject.SetActive(false));
        }

        public void EnableLockToggle(bool state)
        {
            LockToggle.gameObject.SetActive(true);
            LockToggle.SetState(state);
            LockToggle.StateChanged += OnLockToggleStateChanged;
        }

        private void OnLockToggleStateChanged(bool isLocked)
        {
            LockedStateChanged?.Invoke(this, isLocked);
        }
    }
}