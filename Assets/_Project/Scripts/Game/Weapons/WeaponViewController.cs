using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponViewController : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        
        public void SetSlot(WeaponSlot weaponSlot)
        {
            transform.SetParent(weaponSlot.Transform, false);
            SetSortingOrder(weaponSlot.SortingOrder);
        }

        public void SetSortingOrder(int sortingOrder)
        {
            SpriteRenderer.sortingOrder = sortingOrder;
        }

        public void SetFlipped(bool flipX)
        {
            SpriteRenderer.flipX = flipX;
        }
    }
}