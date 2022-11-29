using Project.Feedback;
using Project.Game.Collectibles;
using UnityEngine;

namespace Project.Game.Enemies
{
    [CreateAssetMenu(fileName = "data_enemy_", menuName = "Data/Enemies/EnemyData", order = 0)]
    public class EnemyData : ScriptableObject
    {
        [field: SerializeField] public EnemyController Prefab { get; private set; }
        [field: SerializeField] public float BaseHealth { get; private set; } = 3;
        [field: SerializeField] public float MoveSpeed { get; private set; } = 3;
        [field: SerializeField] public float TimedActionCooldown { get; set; }
        [field: SerializeField] public MovementMode MovementMode { get; protected set; } = MovementMode.Chase;
        [field: SerializeField] public float MeleeAttackRate { get; set; } = 0.25f;
        [field: SerializeField] public float MeleeDamage { get; set; } = 1f;
        [field: SerializeField] public CollectibleData CollectibleData { get; set; }
        [field: SerializeField] public FeedbackData DeathFeedback { get; set; }
        [field: SerializeField] public AnimationClip DeathAnimation { get; set; }
        [field: SerializeField] public AnimationClip WalkAnimation { get; set; }
    }
    
    public enum MovementMode
    {
        None,
        Chase,
        Wander,
        Charge
    }
}