using Lean.Pool;
using Project.Game.Projectiles;
using Project.Game.Targets;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyController : MonoBehaviour, IProjectileReactor
    {
        [SerializeField] private Target target;
        
        public EnemyData Data { get; private set; }
        
        public void Initialize(EnemyData enemyData)
        {
            Data = enemyData;
            target.Enabled = true;
        }

        public bool ReactToProjectile(ProjectileController projectile)
        {
            if (target.Enabled)
            {
                LeanPool.Despawn(this);
                target.Enabled = false;
                return true;
            }

            return false;
        }
    }
}