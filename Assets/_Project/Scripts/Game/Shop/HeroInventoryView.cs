using System;
using System.Collections.Generic;
using System.Linq;
using Mtl.Injection;
using Project.Application;
using Project.Heroes;
using TMPro;
using UnityEngine;

namespace Project.Game.Shop
{
    public class HeroInventoryView : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private RectTransform headerPrefab;
        [SerializeField] private HeroInventoryRow rowPrefab;
        [SerializeField] private RectTransform parent;
        

        [Inject] private readonly HeroInfo _heroInfo;

        public void OnReady()
        {
            _heroInfo.CurrentWeaponsChanged += OnCurrentWeaponsChanged;
            _heroInfo.ItemsChanged += OnItemsChanged;
        }

        private void OnEnable()
        {
            if (_heroInfo == null)
            {
                return;
            }
            
            Refresh();
        }

        private void OnItemsChanged()
        {
            Refresh();
        }

        private void OnCurrentWeaponsChanged()
        {
            Refresh();
        }

        public void Refresh()
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                Destroy(child.gameObject);
            }

            var weaponHeader = Instantiate(headerPrefab, parent);
            weaponHeader.GetComponentInChildren<TextMeshProUGUI>().text = "WEAPONS";
            
            // add enough rows to support heroes max weapon count 
            var rowCount = _heroInfo.Data.WeaponSlots / 3;
            rowCount += _heroInfo.Data.WeaponSlots % 3 != 0 ? 1 : 0;

            var weaponIndex = 0;
            for (var i = 0; i < rowCount; i++)
            {
                var weapons = new List<BaseData>();
                for (var j = weaponIndex; j < weaponIndex + 3; j++)
                {
                    if (j < _heroInfo.CurrentWeapons.Count)
                    {
                        weapons.Add(_heroInfo.CurrentWeapons[j]);
                    }
                }
                
                var row = Instantiate(rowPrefab, parent);
                row.Populate(weapons, 3);
                
                weaponIndex += 3;
            }
            
            var itemsHeader = Instantiate(headerPrefab, parent);
            itemsHeader.GetComponentInChildren<TextMeshProUGUI>().text = "ITEMS";
            
            // add enough rows to support heroes max weapon count 
            rowCount = _heroInfo.Items.Count / 3;
            rowCount += _heroInfo.Items.Count % 3 != 0 ? 1 : 0;

            var itemIndex = 0;
            for (var i = 0; i < rowCount; i++)
            {
                var items = new List<BaseData>();
                for (var j = itemIndex; j < itemIndex + 3; j++)
                {
                    if (j < _heroInfo.Items.Count)
                    {
                        items.Add(_heroInfo.Items[j]);
                    }
                }
                
                var row = Instantiate(rowPrefab, parent);
                row.Populate(items, -1);
                
                itemIndex += 3;
            }
        }
    }
}