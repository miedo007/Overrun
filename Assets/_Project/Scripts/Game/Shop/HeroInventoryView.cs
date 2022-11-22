using System;
using System.Collections.Generic;
using Mtl.Injection;
using Project.Application;
using Project.Heroes;
using Project.Stats;
using TMPro;
using UnityEngine;

namespace Project.Game.Shop
{
    public class HeroInventoryView : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private RectTransform headerPrefab;
        [SerializeField] private HeroInventoryRow rowPrefab;
        [SerializeField] private RectTransform parent;

        [Inject] private readonly HeroRegistry _heroRegistry;

        private HeroInfo _heroInfo;

        public void OnReady()
        {
            _heroRegistry.ActiveHeroChanged += OnActiveHeroChanged;
            _heroInfo = _heroRegistry.ActiveHero;
            OnActiveHeroChanged(_heroRegistry.ActiveHero);
        }

        private void OnDestroy()
        {
            _heroRegistry.ActiveHeroChanged -= OnActiveHeroChanged;
        }

        private void OnActiveHeroChanged(HeroInfo heroInfo)
        {
            if (_heroInfo != null)
            {
                _heroRegistry.ActiveHero.WeaponsChanged -= OnWeaponsChanged;
                _heroRegistry.ActiveHero.ItemsChanged -= OnItemsChanged;
            }

            _heroInfo = heroInfo;
            _heroInfo.WeaponsChanged += OnWeaponsChanged;
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

        private void OnWeaponsChanged()
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

            if (_heroInfo == null)
            {
                _heroInfo = _heroRegistry.ActiveHero;
            }
            
            // add enough rows to support heroes max weapon count 
            var slotCount = _heroInfo.GetWeaponSlotCount();
            var rowCount = slotCount / 3;
            rowCount += slotCount % 3 != 0 ? 1 : 0;

            var slotIndex = 0;
            for (var i = 0; i < rowCount; i++)
            {
                var row = Instantiate(rowPrefab, parent);
                for (var j = slotIndex; j < slotIndex + 3; j++)
                {
                    if (j < _heroInfo.CurrentWeapons.Count)
                    {
                        row.AddItem(_heroInfo.CurrentWeapons[j]);
                    }
                    else if (j < slotCount)
                    {
                        row.AddEmpty();
                    }
                    else
                    {
                        row.AddPlaceholder();
                    }
                }
                
                slotIndex += 3;
            }
            
            var itemsHeader = Instantiate(headerPrefab, parent);
            itemsHeader.GetComponentInChildren<TextMeshProUGUI>().text = "ITEMS";
            
            rowCount = _heroInfo.Items.Count / 3;
            rowCount += _heroInfo.Items.Count % 3 != 0 ? 1 : 0;

            var itemIndex = 0;
            for (var i = 0; i < rowCount; i++)
            {
                var row = Instantiate(rowPrefab, parent);
                
                for (var j = itemIndex; j < itemIndex + 3; j++)
                {
                    if (j < _heroInfo.Items.Count)
                    {
                        row.AddItem(_heroInfo.Items[j]);
                    }
                    else
                    {
                        row.AddPlaceholder();
                    }
                }
                itemIndex += 3;
            }
        }
    }
}