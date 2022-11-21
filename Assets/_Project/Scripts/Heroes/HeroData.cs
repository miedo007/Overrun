using Project.Game.Items;
using Project.Game.Weapons;
using Project.Stats;
using Project.Tiers;
using UnityEngine;

namespace Project.Heroes
{
    [CreateAssetMenu(fileName = "data_hero_", menuName = "Data/Heroes/HeroData", order = 0)]
    public class HeroData : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; set; }
        [field: SerializeField] public HeroViewController Prefab { get; private set; }
        [field: SerializeField] public CharacterStats StatOverrides { get; private set; }
        [field: SerializeField] public WeaponData[] StartingWeapons { get; private set; }
        [field: SerializeField] public TieredGroupDatabase StartingWeaponDatabase { get; set; }
        [field: SerializeField] public ItemData[] StartingItems { get; private set; }
        [field: SerializeField] public int StartingShopCurrency { get; set; } = 0;

        public string Id => name;
    }
}