using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponSlot : MonoBehaviour
    {
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public int SortingOrder { get; private set; } = 1;

        public WeaponController Weapon { get; private set; }
        public WeaponData WeaponData { private set; get; }

        public void SetWeapon(WeaponData weaponData)
        {
            WeaponData = weaponData;
            Weapon = Instantiate(WeaponData.Prefab);
            Weapon.Initialize(weaponData, this);
        }
    }
}