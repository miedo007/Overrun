using Lean.Pool;
using Mtl.Injection;
using Project.Game.Projectiles;
using Project.Game.Targets;
using Project.PopupText;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyController : MonoBehaviour, IProjectileReactor
    {
        [SerializeField] private Target target;

        [Inject] private readonly PopupTextManager popupTextManager;
        
        
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
                
                popupTextManager.DisplayTextAtPosition($"{damage}", Color.white, target.transform.position);
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