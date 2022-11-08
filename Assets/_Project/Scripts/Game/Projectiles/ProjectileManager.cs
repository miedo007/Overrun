using System.Collections.Generic;
using Lean.Pool;
using Mtl.Injection;
using Project.Game.Levels;
using UnityEngine;

namespace Project.Game.Projectiles
{
    public class ProjectileManager : MonoBehaviour, IInjectionReady
    {
        private readonly List<ProjectileController> _activeProjectiles = new List<ProjectileController>();

        [Inject] private readonly LevelController _levelController;
        
        public void OnReady()
        {
            ProjectileController.Added += OnProjectileAdded;
            _levelController.WaveCompleted += OnWaveOrLevelComplete;
            _levelController.LevelCompleted += OnWaveOrLevelComplete;
        }

        private void OnDestroy()
        {
            ProjectileController.Added -= OnProjectileAdded;
            _levelController.WaveCompleted -= OnWaveOrLevelComplete;
            _levelController.LevelCompleted -= OnWaveOrLevelComplete;
        }

        private void OnWaveOrLevelComplete()
        {
            foreach (var projectile in _activeProjectiles)
            {
                projectile.Kill();
                LeanPool.Despawn(projectile);
            }
            
            _activeProjectiles.Clear();
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