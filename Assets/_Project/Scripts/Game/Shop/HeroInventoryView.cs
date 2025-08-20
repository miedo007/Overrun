using System;
using TMPro;
using UnityEngine;
using Mtl.Injection;
using Project.Heroes;

namespace Project.Game.Shop
{
    public class HeroInventoryView : MonoBehaviour, IInjectionReady
    {
        [Header("Prefabs & Parent")]
        [SerializeField] private RectTransform headerPrefab;
        [SerializeField] private HeroInventoryRow rowPrefab;
        [SerializeField] private RectTransform parent;

        [Header("Layout")]
        [SerializeField, Min(1)] private int columns = 6; // ← set this in Inspector

        [Inject] private readonly HeroRegistry _heroRegistry;

        private HeroInfo _heroInfo;

        public void OnReady()
        {
            _heroRegistry.ActiveHeroChanged += OnActiveHeroChanged;
            _heroInfo = _heroRegistry.ActiveHero;
            OnActiveHeroChanged(_heroInfo);
        }

        private void OnDestroy()
        {
            _heroRegistry.ActiveHeroChanged -= OnActiveHeroChanged;

            if (_heroInfo != null)
            {
                _heroInfo.WeaponsChanged -= OnWeaponsChanged;
                _heroInfo.ItemsChanged   -= OnItemsChanged;
            }
        }

        private void OnActiveHeroChanged(HeroInfo heroInfo)
        {
            // Unsubscribe from previous hero (if any)
            if (_heroInfo != null)
            {
                _heroInfo.WeaponsChanged -= OnWeaponsChanged;
                _heroInfo.ItemsChanged   -= OnItemsChanged;
            }

            // Track & subscribe to new hero
            _heroInfo = heroInfo;
            if (_heroInfo != null)
            {
                _heroInfo.WeaponsChanged += OnWeaponsChanged;
                _heroInfo.ItemsChanged   += OnItemsChanged;
            }

            if (isActiveAndEnabled) Refresh();
        }

        private void OnEnable()
        {
            if (_heroInfo != null) Refresh();
        }

        private void OnItemsChanged()  => Refresh();
        private void OnWeaponsChanged()=> Refresh();

        public void Refresh()
        {
            if (parent == null) return;

            // Clear parent
            for (var i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);

            if (_heroInfo == null)
            {
                _heroInfo = _heroRegistry.ActiveHero;
                if (_heroInfo == null) return;
            }

            // ---- WEAPONS ----
            var weaponHeader = Instantiate(headerPrefab, parent);
            weaponHeader.GetComponentInChildren<TextMeshProUGUI>().text = "WEAPONS";

            var slotCount = _heroInfo.GetWeaponSlotCount();
            var weaponRowCount = Mathf.CeilToInt((float)slotCount / columns);

            var slotIndex = 0;
            for (var i = 0; i < weaponRowCount; i++)
            {
                var row = Instantiate(rowPrefab, parent);

                for (var j = slotIndex; j < slotIndex + columns; j++)
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

                slotIndex += columns;
            }

            // ---- ITEMS ----
            var itemsHeader = Instantiate(headerPrefab, parent);
            itemsHeader.GetComponentInChildren<TextMeshProUGUI>().text = "ITEMS";

            var itemCount = _heroInfo.Items.Count;
            var itemRowCount = Mathf.CeilToInt((float)itemCount / columns);

            var itemIndex = 0;
            for (var i = 0; i < itemRowCount; i++)
            {
                var row = Instantiate(rowPrefab, parent);

                for (var j = itemIndex; j < itemIndex + columns; j++)
                {
                    if (j < itemCount)
                        row.AddItem(_heroInfo.Items[j]);
                    else
                        row.AddPlaceholder();
                }

                itemIndex += columns;
            }
        }
    }
}
