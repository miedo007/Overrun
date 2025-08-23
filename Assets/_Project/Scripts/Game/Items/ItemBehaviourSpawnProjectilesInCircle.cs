using Lean.Pool;
using Mtl.Injection;
using Project.Game.Projectiles;
using Project.Heroes;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "item_behaviour_projectile_circle_", menuName = "Data/Items/Behaviours/ProjectileCircle", order = 0)]
    public class ItemBehaviourSpawnProjectilesInCircle : ItemBehaviour
    {
        [SerializeField] private ProjectileData projectileData;
        [SerializeField] private StatData rangedStat;
        [SerializeField] private StatData damageStat;
        [SerializeField] private int count;
        [SerializeField] private float damageFactor;

        private StatInfo _rangedStatInfo;
        private StatInfo _damageStatInfo;
        
        public StatInfo RangedStatInfo
        {
            get
            {
                if (_rangedStatInfo == null)
                {
                    var hero = InjectionContainer.Instance?.Injector?.Get<HeroRegistry>()?.ActiveHero;
                    if (hero != null && rangedStat != null)
                        _rangedStatInfo = hero.GetStat(rangedStat);
                }
                return _rangedStatInfo;
            }
        }
        
        public StatInfo DamageStatInfo
        {
            get
            {
                if (_damageStatInfo == null)
                {
                    var hero = InjectionContainer.Instance?.Injector?.Get<HeroRegistry>()?.ActiveHero;
                    if (hero != null && damageStat != null)
                        _damageStatInfo = hero.GetStat(damageStat);
                }
                return _damageStatInfo;
            }
        }

        private bool TryResolveStats(out StatInfo ranged, out StatInfo dmg)
        {
            ranged = RangedStatInfo;
            dmg    = DamageStatInfo;
            return ranged != null && dmg != null;
        }

        private float GetDamage()
        {
            // Only called at runtime during combat; stats should be resolved then.
            return (RangedStatInfo?.GetFloatValue() ?? 1f) *
                   (DamageStatInfo?.GetFloatValue() ?? 1f) *
                   damageFactor;
        }
        
        public override bool OnPerform(Vector3 position)
        {
            var angleBetween = 360f / count;
            var angleOffset = Random.Range(0, 360);
            for (var i = 0; i < count; i++)
            {
                if (projectileData?.ProjectilePrefab == null)
                {
                    Debug.LogError($"{name}: Missing projectileData/ProjectilePrefab");
                    break;
                }

                var projectile = LeanPool.Spawn(
                    projectileData.ProjectilePrefab,
                    position,
                    Quaternion.Euler(Vector3.forward * (angleBetween * i + angleOffset)));

                projectile.Initialize(projectileData, GetDamage(), -1, -1, 0);
            }
            return true;
        }

        public override string GetDescription()
        {
            // Safe preview for shop/menus where hero/injector might not exist yet.
            if (!TryResolveStats(out var r, out var d))
            {
                var rangedIcon = rangedStat != null ? rangedStat.name : "Ranged";
                var damageIcon = damageStat != null ? damageStat.name : "Damage";
                return $"<b>{GetChanceDisplay()}</b> to fire <b>{count}</b> bullets. "
                     + $"Damage scales with <b>{rangedIcon}</b> × <b>{damageIcon}</b> × <b>{damageFactor:0.##}</b>.";
            }

            var dmg = GetDamage();
            var rangedIconName = r?.Data?.Icon ? r.Data.Icon.name : "stat";
            return $"<b>{GetChanceDisplay()}</b> to fire <b>{count}</b> bullets, "
                 + $"dealing <b>{dmg:0.#}</b> damage.";
        }

        public override void Cleanup()
        {
            _rangedStatInfo = null;
            _damageStatInfo = null;
        }
    }
}
