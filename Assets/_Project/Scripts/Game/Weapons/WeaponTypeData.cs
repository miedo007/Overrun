using Project.Stats;
using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "data_weapon_type_", menuName = "Data/Weapons/WeaponTypeData", order = 0)]
    public class WeaponTypeData : ScriptableObject
    {
        [field: SerializeField] public StatData CooldownReductionStat { get; private set; }
        [field: SerializeField] public StatData DamageStat { get; private set; }
        [field: SerializeField] public StatData DamagePercentStat { get; private set; }
        [field: SerializeField] public StatData CriticalChanceStat { get; private set; }
    }
}