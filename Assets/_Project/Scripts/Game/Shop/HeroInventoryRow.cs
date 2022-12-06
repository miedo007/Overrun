using System.Collections.Generic;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Tutorials;
using UnityEngine;

namespace Project.Game.Shop
{
    public class HeroInventoryRow : MonoBehaviour
    {
        [SerializeField] private HeroInventoryItem itemPrefab;
        [SerializeField] private EmptySlot emptySlotPrefab;
        [SerializeField] private GameObject placeholderPrefab;

        [Inject] private readonly UIFrame _uiFrame;
        
        
        public void AddItem(BaseData item)
        {
            var itemInstance = Instantiate(itemPrefab, transform);
            itemInstance.Initialize(item);
        }

        public void AddEmpty()
        {
            var emptySlot = Instantiate(emptySlotPrefab, transform);
            emptySlot.Clicked += OnEmptySlotClicked;
        }

        private void OnEmptySlotClicked(EmptySlot emptySlot)
        {
            if (!PrefKeys.HasCompletedWeaponSlotTutorial())
            {
                _uiFrame.Open<WeaponSlotTutorialScreen>();
            }
        }

        public void AddPlaceholder()
        {
            Instantiate(placeholderPrefab, transform);
        }
        

        public void Populate(List<BaseData> items, int forceCount = -1)
        {
            var count = forceCount < 0 ? items.Count :
                forceCount > items.Count ? forceCount : items.Count;
            
            for (var index = 0; index < count; index++)
            {
                var itemInstance = Instantiate(itemPrefab, transform);
                
                if (index < items.Count)
                {
                    var item = items[index];
                    itemInstance.Initialize(item);
                }
            }
        }
    }
}