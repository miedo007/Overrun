using System;
using System.Collections.Generic;
using Mtl.Injection;
using Project.Application;
using Project.Extensions;
using Project.Game.Items;
using Project.Game.Levels;
using Project.Game.UI;
using Project.Game.Weapons;
using Project.Heroes;
using Project.Tiers;
using Tromagon.Extensions;
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
        [Inject] private  HeroInfo _heroInfo;
        [Inject] private  GameData _gameData;
        
        private int _waveIndex;

        private List<ShopInventoryItemView> CurrentItems = new();

        private void Awake()
        {
            Parent.RemoveAllChildren();
        }

        public void Populate(int waveIndex)
        {
            _waveIndex = waveIndex;
            
            ClearItems();
            
            var minTier = _waveIndex - 10;
            var maxTier = Mathf.FloorToInt(_waveIndex * 0.45f) + 1;

            var weaponCount = 2;
            var items = new List<BaseData>();
            for (var i = 0; i < weaponCount; i++)
            {
                items.Add(_weaponDatabase.GetRandom().GetRandomTier(_gameData.RarityCurve, minTier, maxTier).Data);
            }

            var itemCount = 4;
            for (var i = 0; i < itemCount; i++)
            {
                items.Add(_itemDatabase.GetRandom().GetRandomTier(_gameData.RarityCurve, minTier, maxTier).Data);
            }

            items.Shuffle();
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
            itemView.Initialize(data, _heroInfo, cost);
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
            if (_heroInfo.GetShopCurrencyIntValue() < cost)
            {
                // can't afford
                return;
            }
            
            var weaponData = data as WeaponData;
            if (weaponData != null)
            {
                if (_heroInfo.CurrentWeapons.Count >= _heroInfo.Data.WeaponSlots)
                {
                    weaponsFullNotification.Display();
                    return;
                }
                
                _heroInfo.AddWeapon(weaponData);
            }
            
            var itemData = data as ItemData;
            if (itemData != null)
            {
                _heroInfo.AddItem(itemData);
            }
            
            _heroInfo.ShopCurrency -= cost;
            itemView.Purchase();
        }
    }
}