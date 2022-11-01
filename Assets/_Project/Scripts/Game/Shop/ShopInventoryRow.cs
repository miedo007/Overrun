using System.Collections.Generic;
using Project.Application;
using UnityEngine;

namespace Project.Game.Shop
{
    public class ShopInventoryRow : MonoBehaviour
    {
        [field: SerializeField] public ShopItemSlot[] Slots { get; private set; }
        [field: SerializeField] public ShopInventoryItemView ItemViewPrefab { get; private set; }

        public void Populate(List<BaseData> dataList)
        {
            for (var i = 0; i < Slots.Length; i++)
            {
                var slot = Slots[i];
                slot.AddItem(ItemViewPrefab, dataList[i]);
            }
        }
    }
}