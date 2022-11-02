using System.Collections;
using System.Collections.Generic;
using Mtl.Injection;
using Project.Application;
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

        private List<ShopInventoryItemView> CurrentItems = new();


        public void Populate()
        {
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

            foreach (var item in items)
            {
                AddItem(item);
            }

            Scroller.horizontalNormalizedPosition = 0;
        }
        
        public void AddItem(BaseData data)
        {
            var itemView = Instantiate(ItemViewPrefab, Parent);
            itemView.Initialize(data, _heroInfo);
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
            _heroInfo.ShopCurrency -= data.BasePrice;

            var weaponData = data as WeaponData;
            if (weaponData != null)
            {
                _heroInfo.AddWeapon(weaponData);
                return;
            }
            
            var itemData = data as ItemData;
            if (itemData != null)
            {
                _heroInfo.AddItem(itemData);
            }
        }
    }
}