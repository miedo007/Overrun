using System;
using Project.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Game.Projectiles
{
    public class ProjectileController : MonoBehaviour
    {
        public static event Action<ProjectileController> Added;

        private float _initializationTime;
        private Transform _transform;
        private float _speed;
        private float _lifespan;
        private float _criticalChance;
        private float _criticalMultiplier;
        private int _pierceCount;
        private float _knockbackForce;
        private float _currentLife;

        public ProjectileData Data { get; private set; }
        public bool IsActive { get; private set; }

        public float Damage { get; private set; }

        private void Awake()
        {
            IsActive = true;
            _transform = transform;
        }

        public void Initialize(ProjectileData data, float damage, float criticalChance = 0f, float criticalMultiplier = 0f, float knockbackForce = 0f)
        {
            _currentLife = 0;
            _pierceCount = data.BasePierceCount;
            _criticalChance = criticalChance;
            _criticalMultiplier = criticalMultiplier;
            _knockbackForce = knockbackForce;
            
            _initializationTime = Time.time;
            Damage = damage;
            
            IsActive = true;
            Data = data;
            
            _speed = Data.RandomSpeed ? Random.Range(data.SpeedRange.x, data.SpeedRange.y) : data.Speed;
            _lifespan = Data.RandomLifespan ? Random.Range(data.LifespanRange.x, data.LifespanRange.y) : data.Lifespan;
            
            Added?.Invoke(this);
        }
        
        public void Step(float dt, float time)
        {
            if (_currentLife >= _lifespan)
            {
                IsActive = false;
                return;
            }

            if (Data.ScaleDownOverLife)
            {
                _transform.localScale = Vector3.one * Data.ScaleCurve.Evaluate((_currentLife / _lifespan));
            }
            
            _transform.position += _transform.right * (_speed * Time.deltaTime);
            _currentLife += dt;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsActive)
            {
                return;
            }

            var projectileReactor = other.GetComponent<IProjectileReactor>();
            if (projectileReactor != null)
            {
                var isCritical = Random.value <= _criticalChance;
                var damage = isCritical ? Damage * _criticalMultiplier : Damage;
                if (projectileReactor.ReactToProjectile(this, damage, isCritical, (Vector2)_transform.right, _knockbackForce))
                {
                    if (Data.HitFeedback != null)
                    {
                        Data.HitFeedback.Play(transform.position, Quaternion.identity);
                    }
                    
                    _pierceCount--;
                    if (_pierceCount <= 0)
                    {
                        Kill();
                    }
                }
            }
        }

        public void Kill()
        {
            IsActive = false;
        }
    }
}