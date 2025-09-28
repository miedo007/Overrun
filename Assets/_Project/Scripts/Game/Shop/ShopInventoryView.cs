using System.Collections.Generic;
using Mtl.Injection;
using Mtl.Toolbox;
using Mtl.UiFramework;
using Project.Application;
using Project.Extensions;
using Project.Game.Items;
using Project.Game.Tutorials;
using Project.Game.UI;
using Project.Game.Weapons;
using Project.Heroes;
using Project.Tiers;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopInventoryView : MonoBehaviour
    {
        [field: SerializeField] public ShopInventoryItemView ItemViewPrefab { get; private set; }
        [field: SerializeField] public RectTransform Parent { get; private set; }
        [field: SerializeField] public ScrollRect Scroller { get; private set; }

        [SerializeField] private DropDownNotification weaponsFullNotification;

        [Inject("weapons")] private  TieredGroupDatabase _weaponDatabase;
        [Inject("items")] private  TieredGroupDatabase _itemDatabase;
        [Inject] private  HeroRegistry _heroRegistry;
        [Inject] private  GameData _gameData;
        [Inject] private  UIFrame _uiFrame;

        private BaseData _lockedItem;
        private int _waveIndex;
        private bool _hasUsedRewardedAdThisShop = false; // Track once-per-shop limitation

        private List<ShopInventoryItemView> CurrentItems = new();

        private void Awake()
        {
            Parent.RemoveAllChildren();
        }
        
        public void Populate(int waveIndex)
        {
            ClearItems();
            
            var weaponCount = 2;
            var itemCount = 4;
            
            var items = new List<BaseData>();
            
            if (_lockedItem != null)
            {
                if (_lockedItem as WeaponData != null)
                {
                    weaponCount--;
                }
                else
                {
                    itemCount--;
                }
            }
            
            _waveIndex = waveIndex;
            
            var tierRange = _gameData.GetItemTierRangeForWave(waveIndex);
            var chanceIncrease = _gameData.GetChanceIncreaseForWave(waveIndex);
            for (var i = 0; i < weaponCount; i++)
            {
                items.Add(_weaponDatabase.GetRandom().GetRandomTier(_gameData.RarityCurve, tierRange, chanceIncrease).Data);
            }

            for (var i = 0; i < itemCount; i++)
            {
                items.Add(_itemDatabase.GetRandom().GetRandomTier(_gameData.RarityCurve, tierRange, chanceIncrease).Data);
            }

            items.Shuffle();

            if (_lockedItem != null)
            {
                items.Insert(0, _lockedItem);
            }

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                AddItem(item, _lockedItem != null && index == 0);
            }

            Scroller.horizontalNormalizedPosition = 0;
        }

        public void ResetRewardedAdUsage()
        {
            _hasUsedRewardedAdThisShop = false;
            Debug.Log("[ShopInventoryView] Rewarded ad usage reset for new shop session");
            
            // Update all items to show ad availability
            UpdateAllItemsAdAvailability();
        }
        
        public void AddItem(BaseData data, bool lockedItem)
        {
            var itemView = Instantiate(ItemViewPrefab, Parent);
            var cost = _gameData.GetScaledCost(data, _waveIndex);
            itemView.Initialize(data, _heroRegistry.ActiveHero, cost);
            itemView.BuyButtonClicked += OnBuyButtonClicked;
            itemView.RewardedAdBuyClicked += OnRewardedAdBuyClicked;
            itemView.EnableLockToggle(lockedItem);
            itemView.LockedStateChanged += OnItemViewLockStateChanged;
            
            // Set initial rewarded ad availability
            itemView.SetRewardedAdAvailability(!_hasUsedRewardedAdThisShop);
            
            CurrentItems.Add(itemView);
        }

        private void UpdateAllItemsAdAvailability()
        {
            foreach (var item in CurrentItems)
            {
                item.SetRewardedAdAvailability(!_hasUsedRewardedAdThisShop);
            }
        }

        private void OnItemViewLockStateChanged(ShopInventoryItemView updatedItem, bool state)
        {
            if (!PrefKeys.HasCompletedItemLockingTutorial())
            {
                _uiFrame.Open<ItemLockingTutorialScreen>();
            }
            
            foreach (var item in CurrentItems)
            {
                if (item != updatedItem)
                {
                    item.LockToggle.SetState(false);
                }
            }
        }

        public void ClearItems()
        {
            _lockedItem = null;
            
            for (var index = CurrentItems.Count - 1; index >= 0; index--)
            {
                var itemView = CurrentItems[index];
                if (itemView.LockToggle.IsOn)
                {
                    _lockedItem = itemView.Data;
                }

                CurrentItems.RemoveAt(index);
                itemView.BuyButtonClicked -= OnBuyButtonClicked;
                itemView.RewardedAdBuyClicked -= OnRewardedAdBuyClicked;
                Destroy(itemView.gameObject);
            }
        }

        private void OnBuyButtonClicked(ShopInventoryItemView itemView)
        {
            var data = itemView.Data;
            
            var cost = itemView.Cost;
            if (_heroRegistry.ActiveHero.GetShopCurrencyIntValue() < cost)
            {
                // can't afford
                return;
            }
            
            var weaponData = data as WeaponData;
            if (weaponData != null)
            {
                PurchaseWeapon(itemView, weaponData, cost);
            }
            else
            {
                var itemData = data as ItemData;
                if (itemData != null)
                {
                    PurchaseItem(itemView, itemData, cost);
                }
            }
        }

        private void OnRewardedAdBuyClicked(ShopInventoryItemView itemView)
        {
            Debug.Log($"[ShopInventoryView] Processing rewarded ad purchase for {itemView.Data.DisplayName}");
            
            // Mark rewarded ad as used for this shop session
            _hasUsedRewardedAdThisShop = true;
            UpdateAllItemsAdAvailability();
            
            var data = itemView.Data;
            var weaponData = data as WeaponData;
            if (weaponData != null)
            {
                PurchaseWeaponWithAd(itemView, weaponData);
            }
            else
            {
                var itemData = data as ItemData;
                if (itemData != null)
                {
                    PurchaseItemWithAd(itemView, itemData);
                }
            }
        }

        private void PurchaseItem(ShopInventoryItemView itemView, ItemData itemData, int cost)
        {
            itemView.Purchase();
            _heroRegistry.ActiveHero.ShopCurrency -= cost;
            _heroRegistry.ActiveHero.AddItem(itemData);
            if (!PrefKeys.HasCompletedItemsTutorial())
            {
                _uiFrame.Open<ItemTutorialScreen>();
            }
        }

        private void PurchaseItemWithAd(ShopInventoryItemView itemView, ItemData itemData)
        {
            Debug.Log($"[ShopInventoryView] Purchased {itemData.DisplayName} with rewarded ad (FREE!)");
            itemView.Purchase();
            // No currency cost for rewarded ad purchases
            _heroRegistry.ActiveHero.AddItem(itemData);
            if (!PrefKeys.HasCompletedItemsTutorial())
            {
                _uiFrame.Open<ItemTutorialScreen>();
            }
        }

        private void PurchaseWeapon(ShopInventoryItemView itemView, WeaponData weaponData, int cost)
        {
            if (!_heroRegistry.ActiveHero.HasFreeWeaponSlot())
            {
                // can this weapon be merged with an item in the players inventory?
                if (_heroRegistry.ActiveHero.CanMergeWeapon(weaponData, 1))
                {
                    itemView.Purchase();
                    _heroRegistry.ActiveHero.ShopCurrency -= cost;
                    _heroRegistry.ActiveHero.MergeWeapon(weaponData, 1);
                    return;
                }

                if (!PrefKeys.HasCompletedWeaponSlotsFullTutorial())
                {
                    _uiFrame.Open<WeaponSlotsFullTutorialScreen>();
                }
                weaponsFullNotification.Display();
                return;
            }

            itemView.Purchase();
            _heroRegistry.ActiveHero.ShopCurrency -= cost;
            _heroRegistry.ActiveHero.AddWeapon(weaponData);
        }

        private void PurchaseWeaponWithAd(ShopInventoryItemView itemView, WeaponData weaponData)
        {
            Debug.Log($"[ShopInventoryView] Purchased {weaponData.DisplayName} with rewarded ad (FREE!)");
            
            if (!_heroRegistry.ActiveHero.HasFreeWeaponSlot())
            {
                // can this weapon be merged with an item in the players inventory?
                if (_heroRegistry.ActiveHero.CanMergeWeapon(weaponData, 1))
                {
                    itemView.Purchase();
                    // No currency cost for rewarded ad purchases
                    _heroRegistry.ActiveHero.MergeWeapon(weaponData, 1);
                    return;
                }

                if (!PrefKeys.HasCompletedWeaponSlotsFullTutorial())
                {
                    _uiFrame.Open<WeaponSlotsFullTutorialScreen>();
                }
                weaponsFullNotification.Display();
                return;
            }

            itemView.Purchase();
            // No currency cost for rewarded ad purchases
            _heroRegistry.ActiveHero.AddWeapon(weaponData);
        }
    }
}