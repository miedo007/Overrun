using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace Project.Game.Projectiles
{
    public class ProjectileManager : MonoBehaviour
    {
        private readonly List<ProjectileController> _activeProjectiles = new List<ProjectileController>();

        private void Awake()
        {
            ProjectileController.Added += OnProjectileAdded;
        }

        private void OnDestroy()
        {
            ProjectileController.Added -= OnProjectileAdded;
        }

        private void OnProjectileAdded(ProjectileController projectile)
        {
            _activeProjectiles.Add(projectile);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            var time = Time.time;

            for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
            {
                var projectile = _activeProjectiles[i];
                projectile.Step(dt, time);

                if (!projectile.IsActive)
                {
                    _activeProjectiles.RemoveAt(i);
                    LeanPool.Despawn(projectile);
                }
            }
        }
    }
}