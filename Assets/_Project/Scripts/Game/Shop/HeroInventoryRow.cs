using System.Collections.Generic;
using Project.Application;
using UnityEngine;

namespace Project.Game.Shop
{
    public class HeroInventoryRow : MonoBehaviour
    {
        [SerializeField] private HeroInventoryItem itemPrefab;

        public void Populate(List<BaseData> items, int forceIncludeCount)
        {
            var count = forceIncludeCount < 0 ? items.Count :
                forceIncludeCount > items.Count ? forceIncludeCount : items.Count;
            for (var index = 0; index < count; index++)
            {
                var itemInstance = Instantiate(itemPrefab, transform);
                
                if (index < items.Count)
                {
                    var item = items[index];
                    itemInstance.Icon.sprite = item.Sprite;
                    itemInstance.Icon.enabled = true;
                }
            }
        }
    }
}