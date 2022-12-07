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
                    var heroInfo = InjectionContainer.Instance.Injector.Get<HeroRegistry>().ActiveHero;
                    _rangedStatInfo = heroInfo.GetStat(rangedStat);
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
                    var heroInfo = InjectionContainer.Instance.Injector.Get<HeroRegistry>().ActiveHero;
                    _damageStatInfo = heroInfo.GetStat(damageStat);
                }

                return _damageStatInfo;
            }
        }

        private float GetDamage()
        {
            return RangedStatInfo.GetFloatValue() * DamageStatInfo.GetFloatValue() * damageFactor;
        }
        
        public override bool OnPerform(Vector3 position)
        {
            var angleBetween = 360f / count;
            var angleOffset = Random.Range(0, 360);
            for (var i = 0; i < count; i++)
            {
                var projectile = LeanPool.Spawn(projectileData.ProjectilePrefab,
                    position,
                    Quaternion.Euler(Vector3.forward * (angleBetween * i + angleOffset)));
                
                projectile.Initialize(
                    projectileData,
                    GetDamage(), 
                    -1, 
                    -1,
                    0
                );
            }

            return true;
        }

        public override string GetDescription()
        {
            return $"<b>{GetChanceDisplay()}</b> to fire <b>{count}</b> bullets in a ring, dealing <b>{GetDamage()}</b> damage (x<sprite tint=1 name={RangedStatInfo.Data.Icon.name}>)";
        }

        public override void Cleanup()
        {
            _rangedStatInfo = null;
        }
    }
}