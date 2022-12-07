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
        
        private int _waveIndex;

        private List<ShopInventoryItemView> CurrentItems = new();

        private void Awake()
        {
            Parent.RemoveAllChildren();
        }
        
        public void Populate(int waveIndex)
        {
            ClearItems();
            
            _waveIndex = waveIndex;
            
            var weaponCount = 2;
            var items = new List<BaseData>();
            
            var tierRange = _gameData.GetItemTierRangeForWave(waveIndex);
            var chanceIncrease = _gameData.GetChanceIncreaseForWave(waveIndex);
            for (var i = 0; i < weaponCount; i++)
            {
                items.Add(_weaponDatabase.GetRandom().GetRandomTier(_gameData.RarityCurve, tierRange, chanceIncrease).Data);
            }

            var itemCount = 4;
            for (var i = 0; i < itemCount; i++)
            {
                items.Add(_itemDatabase.GetRandom().GetRandomTier(_gameData.RarityCurve, tierRange, chanceIncrease).Data);
            }

            items.Shuffle();

            foreach (var item in items)
            {
                AddItem(item);
            }

            Scroller.horizontalNormalizedPosition = 0;
        }
        
        public void AddItem(BaseData data)
        {
            var itemView = Instantiate(ItemViewPrefab, Parent);
            var cost = _gameData.GetScaledCost(data, _waveIndex);
            itemView.Initialize(data, _heroRegistry.ActiveHero, cost);
            itemView.BuyButtonClicked += OnBuyButtonClicked;
            CurrentItems.Add(itemView);
        }
        
        public void ClearItems()
        {
            foreach (var item in CurrentItems)
            {
                item.BuyButtonClicked -= OnBuyButtonClicked;
                Destroy(item.gameObject);
            }
            
            CurrentItems.Clear();
        }

        private void OnBuyButtonClicked(ShopInventoryItemView itemView)
        {
            var data = itemView.Data;
            
            var cost = _gameData.GetScaledCost(data, _waveIndex);
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
    }
}