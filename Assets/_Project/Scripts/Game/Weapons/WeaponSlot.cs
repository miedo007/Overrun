using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponSlot : MonoBehaviour
    {
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public int SortingOrder { get; private set; } = 1;
        [field: SerializeField] public int SortingOrderOnFlip { get; private set; } = 1;

        public WeaponViewController WeaponView { get; private set; }
        public WeaponData WeaponData { private set; get; }

        public void SetWeapon(WeaponData weaponData)
        {
            WeaponData = weaponData;
            WeaponView = Instantiate(WeaponData.ViewPrefab);
            WeaponView.SetSlot(this);
        }

        public void SetFlipped(bool flipX)
        {
            WeaponView.SetSortingOrder(flipX ? SortingOrderOnFlip : SortingOrder);
            WeaponView.SetFlipped(flipX);
        }
    }
}