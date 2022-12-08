using System.Collections;
using Lean.Pool;
using Mtl.Injection;
using Project.Game.Levels;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class Launchable : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private Rigidbody2D body;
        
        protected float CritChance;
        protected float Damage;
        protected float CritMultiplier;
        protected float KnockbackForce;
        protected float Lifespan;

        [Inject] private readonly LevelController _levelController;

        public void OnReady()
        {
        }

        private void OnLevelCompleted()
        {
            Cleanup();
        }

        private void OnWaveCompleted()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _levelController.WaveCompleted -= OnWaveCompleted;
            _levelController.LevelCompleted -= OnLevelCompleted;
            StopAllCoroutines();
            LeanPool.Despawn(this);
        }

        public void Initialize(float damage, float criticalChance, float criticalMultiplier,
            float knockbackForce, float lifespan, Vector2 force)
        {
            
            _levelController.WaveCompleted += OnWaveCompleted;
            _levelController.LevelCompleted += OnLevelCompleted;
            body.velocity = Vector2.zero;
            body.AddForce(force, ForceMode2D.Impulse);
            
            CritChance = criticalChance;
            Damage = damage;
            CritMultiplier = criticalMultiplier;
            KnockbackForce = knockbackForce;
            Lifespan = lifespan;

            OnInitialize();
            
            StartCoroutine(EndLifeRoutine(lifespan));
        }

        protected virtual void OnInitialize()
        {
            
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
            Cleanup();
        }
    }
}