using System.Collections;
using Lean.Pool;
using Project.Game.Player;
using Project.Game.Projectiles;
using UnityEngine;

namespace Project.Game.Enemies
{
    [CreateAssetMenu(fileName = "enemy_behaviour_ranged_attack_", menuName = "Data/Enemies/RangedAttack", order = 0)]
    public class EnemyActionRangedAttack : EnemyActionBase
    {
        [field: SerializeField] public ProjectileData ProjectileData { get; private set; }
        [field: SerializeField] public Vector3 SpawnOffset { get; private set; }

        protected override IEnumerator OnPerformActionRoutine(EnemyController enemy, PlayerController player, float time)
        {
            var position = enemy.transform.position;
            var targetPosition = player.transform.position;
            // vector from this object towards the target location
            var vectorToTarget = (targetPosition- position).normalized;
            // rotate that vector by 90 degrees around the Z axis
            var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
            
            var rotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
            var projectile = LeanPool.Spawn(ProjectileData.ProjectilePrefab, position + SpawnOffset, rotation);
            
            projectile.Initialize(
                ProjectileData,
                enemy.CurrentMeleeDamage, 
                -1, 
                -1,
                0
            );
            
            yield break;
        }
    }
}