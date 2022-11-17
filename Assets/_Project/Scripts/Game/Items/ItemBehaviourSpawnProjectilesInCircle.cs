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
        [SerializeField] private int count;

        private StatInfo _rangedStatInfo;
        
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
        
        public override bool OnPerform(Vector3 position)
        {
            var angleBetween = 360f / count;
            for (var i = 0; i < count; i++)
            {
                var projectile = LeanPool.Spawn(projectileData.ProjectilePrefab, position,Quaternion.Euler( Vector3.forward * angleBetween * i));
                projectile.Initialize(
                    projectileData,
                    RangedStatInfo.GetFloatValue(), 
                    -1, 
                    -1,
                    0
                );
            }

            return true;
        }

        public override string GetDescription()
        {
            return $"{GetChanceDisplay()} to firs {count} bullets in a ring";
        }

        public override void Cleanup()
        {
            _rangedStatInfo = null;
        }
    }
}