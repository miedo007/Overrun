using System.Collections;
using Lean.Pool;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class Launchable : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        
        protected float CritChance;
        protected float Damage;
        protected float CritMultiplier;
        protected float KnockbackForce;
        
        public void Initialize(float damage, float criticalChance, float criticalMultiplier,
            float knockbackForce, float lifespan, Vector2 force)
        {
            body.velocity = Vector2.zero;
            body.AddForce(force, ForceMode2D.Impulse);
            
            CritChance = criticalChance;
            Damage = damage;
            CritMultiplier = criticalMultiplier;
            KnockbackForce = knockbackForce;

            StartCoroutine(EndLifeRoutine(lifespan));
        }

        private IEnumerator EndLifeRoutine(float lifespan)
        {
            var timer = 0f;
            while (timer < lifespan)
            {
                yield return null;
                timer += Time.deltaTime;
            }

            OnEndOfLife();
        }

        protected virtual void OnEndOfLife()
        {
            LeanPool.Despawn(this);
        }
    }
}