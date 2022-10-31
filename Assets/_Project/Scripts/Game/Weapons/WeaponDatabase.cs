using System.Collections.Generic;
using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "database_weapons_", menuName = "Data/Weapons/WeaponDatabase", order = 0)]
    public class WeaponDatabase : ScriptableObject
    {
        [field: SerializeField] public List<WeaponData> Weapons { get; private set; }

        public WeaponData GetRandomWeapon()
        {
            return Weapons[Random.Range(0, Weapons.Count)];
        }
        
    }
}