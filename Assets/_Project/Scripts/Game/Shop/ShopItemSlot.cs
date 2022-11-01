using Project.Application;
using UnityEngine;

namespace Project.Game.Shop
{
    public class ShopItemSlot : MonoBehaviour
    {
        public ShopInventoryItemView ItemView { get; private set; }
        
        public void AddItem(ShopInventoryItemView itemViewPrefab, BaseData data)
        {
            if (ItemView != null)
            {
                Destroy(ItemView.gameObject);
            }

            ItemView = Instantiate(itemViewPrefab, transform);
            ItemView.Initialize(data);
        }
    }
}