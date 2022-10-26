using System;
using UnityEngine;

namespace Project.Game.Projectiles
{
    public class ProjectileController : MonoBehaviour
    {
        public static event Action<ProjectileController> Added;

        private float _initializationTime;
        private Transform _transform;
        
        public ProjectileData Data { get; private set; }
        public bool IsActive { get; private set; }

        private void Awake()
        {
            IsActive = true;
            _transform = transform;
        }

        public void Initialize(ProjectileData data)
        {
            _initializationTime = Time.time;
            IsActive = true;
            Data = data;
            Added?.Invoke(this);
        }
        
        public void Step(float dt, float time)
        {
            if (time >= _initializationTime + Data.Lifespan)
            {
                IsActive = false;
            }
            
            _transform.position += _transform.right * Data.Speed * Time.deltaTime;
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
                if (projectileReactor.ReactToProjectile(this))
                {
                    IsActive = false;
                }
            }
        }
    }
}