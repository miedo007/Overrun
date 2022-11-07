using UnityEngine;

namespace Project.Game.Projectiles
{
    public interface IProjectileReactor
    {
        public bool ReactToProjectile(ProjectileController projectile, float damage, bool isCritical, Vector2 force);
    }
}