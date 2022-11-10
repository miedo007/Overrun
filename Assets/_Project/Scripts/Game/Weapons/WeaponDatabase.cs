using System.Collections.Generic;
using Project.Tiers;
using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "database_weapons_", menuName = "Data/Weapons/WeaponDatabase", order = 0)]
    public class WeaponDatabase : ScriptableObject
    {
        [field: SerializeField] public List<WeaponData> Weapons { get; private set; }
        [field: SerializeField] public List<TieredDataGroup> WeaponGroups { get; private set; }

        public TieredDataGroup GetRandomGroup()
        {
            return WeaponGroups[Random.Range(0, WeaponGroups.Count)];
        }
        
        public WeaponData GetRandom()
        {
            return Weapons[Random.Range(0, Weapons.Count)];
        }
        
    }
}