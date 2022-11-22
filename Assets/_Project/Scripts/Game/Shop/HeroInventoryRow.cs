using System.Collections.Generic;
using Project.Application;
using UnityEngine;

namespace Project.Game.Shop
{
    public class HeroInventoryRow : MonoBehaviour
    {
        [SerializeField] private HeroInventoryItem itemPrefab;
        [SerializeField] private GameObject emptySlotPrefab;
        [SerializeField] private GameObject placeholderPrefab;

        public void AddItem(BaseData item)
        {
            var itemInstance = Instantiate(itemPrefab, transform);
            itemInstance.Initialize(item);
        }

        public void AddEmpty()
        {
            Instantiate(emptySlotPrefab, transform);
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