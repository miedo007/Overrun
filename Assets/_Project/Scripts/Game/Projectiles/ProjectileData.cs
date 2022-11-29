using NaughtyAttributes;
using Project.Feedback;
using UnityEngine;

namespace Project.Game.Projectiles
{
    [CreateAssetMenu(fileName = "data_projectile_", menuName = "Data/ProjectileData", order = 0)]
    public class ProjectileData : ScriptableObject
    {
        [field: SerializeField] public ProjectileController ProjectilePrefab { get; private set; }
        [field: SerializeField] public bool RandomSpeed { get; private set; } = false;
        [field: SerializeField, HideIf("RandomSpeed")] public float Speed { get; private set; } = 10f;
        [field: SerializeField, ShowIf("RandomSpeed")] public Vector2 SpeedRange { get; private set; } = new Vector2(10f, 12f);
        [field: SerializeField] public bool RandomLifespan { get; private set; } = false;
        [field: SerializeField, HideIf("RandomLifespan")] public float Lifespan { get; private set; } = 3;
        [field: SerializeField, ShowIf("RandomLifespan")] public Vector2 LifespanRange { get; private set; } = new Vector2(0.5f, 1f);
        [field: SerializeField] public int BasePierceCount { get; private set; } = 1;
        [field: SerializeField] public FeedbackData HitFeetback { get; private set; }
    }
}