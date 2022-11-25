using Project.Application;
using Project.Heroes;
using Project.Feedback;
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
        [field: SerializeField] public float KnockbackMultiplier { get; set; } = 1f;
        [field: SerializeField] public FeedbackData ActivationFeedback { get; set; }
        [field: SerializeField] public FeedbackData MeleeHitFeedback { get; set; }

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

        public string GetRangeDisplayText(HeroInfo heroInfo)
        {
            return GetDisplayStringForValue(Range, false);
        }
        
        public string GetDamageDisplayText(HeroInfo heroInfo)
        {
            var projectileCount = 1;
            var multiProjectileBehavior = BehaviourBase as WeaponBehaviourMultiProjectile;
            if (multiProjectileBehavior != null)
            {
                projectileCount = multiProjectileBehavior.ProjectileCount;
            }

            var projectileCountString = projectileCount > 1 ? $"{projectileCount}x" : "";
            var damageStat = heroInfo.GetStat(Type.DamageStat);
            var damageValue = DamageFactor * damageStat.GetFloatValue();
            var valueString = GetDisplayStringForValue(damageValue, false);
            var modifiedDirection = damageStat.GetModifiedDirection();
            return $"{GetColorStringForModifiedDirection(modifiedDirection)}<b>{projectileCountString}{valueString}</b><alpha=#88>( x<sprite tint=1 name={damageStat.Data.Icon.name}>)";
        }

        private string GetColorStringForModifiedDirection(int modifiedDirection)
        {
            var colorString = "";
            if (modifiedDirection < 0)
            {
                colorString = $"<color={Colors.Negative}>";
            }
            else if (modifiedDirection > 0)
            {
                colorString = $"<color={Colors.Positive}>";
            }

            return colorString;
        }

        public string GetCritDamageDisplayText(HeroInfo heroInfo)
        {
            var damageStat = heroInfo.GetStat(Type.DamageStat);
            var critDamageValue = DamageFactor * damageStat.GetFloatValue() * CriticalDamageMultiplier;
            var valueString = GetDisplayStringForValue(critDamageValue, false);
            return $"<b>{valueString}</b>";
        }
        
        public string GetCooldownDisplayText(HeroInfo heroInfo)
        {
            var cooldownStat = heroInfo.GetStat(Type.CooldownReductionStat);
            var cooldownValue = Cooldown * (1f-cooldownStat.GetFloatValue());
            var valueString = GetDisplayStringForValue(cooldownValue, false);
            var modifiedDirection = cooldownStat.GetModifiedDirection();
            return $"{GetColorStringForModifiedDirection(modifiedDirection)}<b>{valueString}s</b><alpha=#88>( x<sprite tint=1 name={cooldownStat.Data.Icon.name}>)";
        }
        
        public string GetKnockbackDisplayText(HeroInfo heroInfo)
        {
            var knockbackStat = heroInfo.GetStat(Type.KnockbackStat);
            var knockbackValue =  KnockbackMultiplier * knockbackStat.GetFloatValue();
            var valueString = GetDisplayStringForValue(knockbackValue, false);
            var modifiedDirection = knockbackStat.GetModifiedDirection();
            return $"{GetColorStringForModifiedDirection(modifiedDirection)}<b>{valueString}</b><alpha=#88>( x<sprite tint=1 name={knockbackStat.Data.Icon.name}>)";
        }
    }
}