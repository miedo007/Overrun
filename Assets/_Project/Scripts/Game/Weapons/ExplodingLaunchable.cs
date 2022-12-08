using System;
using Project.Feedback;
using Project.Game.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Game.Weapons
{
    public class ExplodingLaunchable : Launchable
    {
        public event Action<ExplodingLaunchable> Exploded;
        
        [SerializeField] private FeedbackData explosionFeedback;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private ExplosionWarning explosionWarning;
        

        private static readonly Collider2D[] Results = new Collider2D[32];

        protected override void OnInitialize()
        {
            if (explosionWarning != null)
            {
                explosionWarning.Initialize(radius, Lifespan);
            }
        }

        protected override void OnEndOfLife()
        {
            var center = transform.position;
            explosionFeedback.Play(center, Quaternion.identity);
            var resultCount = Physics2D.OverlapCircleNonAlloc(center, radius, Results, layerMask);
            for (int i = 0; i < resultCount; i++)
            {
                var other = Results[i];
                var damageReceiver = other.GetComponent<IDamageReceiver>();
                if (damageReceiver != null)
                {
                    var isCritical = Random.value <= CritChance;
                    var damage = isCritical ? Damage * CritMultiplier : Damage;
                    damageReceiver.ReceiveDamage(damage, false,
                        (other.transform.position - center).normalized, KnockbackForce, gameObject);
                }
            }
            
            base.OnEndOfLife();
        }
    }
}