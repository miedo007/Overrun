using Project.Application;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "data_weapon_", menuName = "Data/WeaponData", order = 0)]
    public class WeaponData : BaseData
    {
        [field: SerializeField] public WeaponController Prefab { get; private set; }
        [field: SerializeField] public WeaponTypeData Type { get; private set; }
        [field: SerializeField] public WeaponBehaviourBase BehaviourBase { get; set; }
        [field: SerializeField, Tooltip("Duration between attacks")] public float Cooldown { get; private set; } = 1f;
        [field: SerializeField, Tooltip("Multiplied by WeaponType Damage Stat")] public float DamageFactor { get; private set; } = 1f;
        [field: SerializeField] public float Range { get; set; } = 2.5f;
        [field: SerializeField] public float CriticalDamageMultiplier { get; set; } = 2f;

        public override string GetDescriptionForHero(HeroInfo heroInfo)
        {
            var baseDamage = DamageFactor * heroInfo.GetStat(Type.DamageStat).GetFloatValue();
            var cooldown = Cooldown * (1f - heroInfo.GetStat(Type.CooldownReductionStat).GetFloatValue());

            var description = 
                $"DMG:  <b>{baseDamage:0.0}</b>\n" +
                $"Crit DMG: <b>{CriticalDamageMultiplier:0.0}</b>\n" +
                $"Cooldown: <b>{cooldown:0.0}</b>\n" +
                $"Range: <b>{Range:0.0}</b>";

            return description;
        }
    }
}