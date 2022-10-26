namespace Project.Game.Projectiles
{
    public interface IProjectileReactor
    {
        public bool ReactToProjectile(ProjectileController projectile);
    }
}