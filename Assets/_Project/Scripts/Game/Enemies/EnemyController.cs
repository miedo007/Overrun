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

        public float CurrentHealth { get; private set; }
        
        public void Initialize(EnemyData enemyData)
        {
            CurrentHealth = enemyData.BaseHealth;
            Data = enemyData;
            target.Enabled = true;
        }

        public bool ReactToProjectile(ProjectileController projectile, float damage)
        {
            if (target.Enabled)
            {
                CurrentHealth -= damage;
                if (CurrentHealth <= 0)
                {
                    Kill();
                }
                return true;
            }

            return false;
        }

        public void Kill()
        {
            LeanPool.Despawn(this);
            target.Enabled = false;
        }
    }
}