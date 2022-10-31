using Project.Game.Weapons;
using Project.Stats;
using UnityEngine;

namespace Project.Heroes
{
    [CreateAssetMenu(fileName = "data_hero_", menuName = "Data/Heroes/HeroData", order = 0)]
    public class HeroData : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public HeroViewController Prefab { get; private set; }
        [field: SerializeField] public CharacterStats StatOverrides { get; private set; }
        [field: SerializeField] public WeaponData[] StartingWeapons { get; private set; }
        [field: SerializeField] public int WeaponSlots { get; set; } = 6;
    }
}