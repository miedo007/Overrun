using System.Collections.Generic;
using Mtl.Injection;
using Project.Application;
using Project.Extensions;
using Project.Game.Items;
using Project.Game.Weapons;
using Project.Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopInventoryView : MonoBehaviour
    {
        [field: SerializeField] public ShopInventoryItemView ItemViewPrefab { get; private set; }
        [field: SerializeField] public RectTransform Parent { get; private set; }
        [field: SerializeField] public ScrollRect Scroller { get; private set; }
        
        [Inject] private  WeaponDatabase _weaponDatabase;
        [Inject] private  ItemDatabase _itemDatabase;
        [Inject] private  HeroInfo _heroInfo;
        [Inject] private  GameData _gameData;

        private int _waveIndex;

        private List<ShopInventoryItemView> CurrentItems = new();


        public void Populate(int waveIndex)
        {
            _waveIndex = waveIndex;
            
            ClearItems();
            
            var weaponCount = 2;
            var items = new List<BaseData>();
            for (var i = 0; i < weaponCount; i++)
            {
                items.Add(_weaponDatabase.GetRandom());
            }

            var itemCount = 4;
            for (var i = 0; i < itemCount; i++)
            {
                items.Add(_itemDatabase.GetRandom());
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
            var cost = GetScaledCost(data);
            itemView.Initialize(data, _heroInfo, cost);
            itemView.BuyButtonClicked += OnBuyButtonClicked;
            CurrentItems.Add(itemView);
        }

        private int GetScaledCost(BaseData data)
        {
            return  Mathf.CeilToInt((data.BasePrice * _gameData.ShopBasePriceMultiplier) * Mathf.Pow(_gameData.ShopPriceIncreaseCoeffecient, _waveIndex));
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
            
            var cost = GetScaledCost(data);
            if (_heroInfo.ShopCurrency < cost)
            {
                // can't afford
                return;
            }
            
            var weaponData = data as WeaponData;
            if (weaponData != null)
            {
                if (_heroInfo.CurrentWeapons.Count >= _heroInfo.Data.WeaponSlots)
                {
                    //TODO :: Indicate to player that their weapon slots are full
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