using UnityEngine;

namespace Project.Game.Projectiles
{
    [CreateAssetMenu(fileName = "data_projectile_", menuName = "Data/ProjectileData", order = 0)]
    public class ProjectileData : ScriptableObject
    {
        [field: SerializeField] public ProjectileController ProjectilePrefab { get; private set; }
        [field: SerializeField] public float Speed { get; private set; } = 10f;
        [field: SerializeField] public float Lifespan { get; private set; } = 3;
    }
}