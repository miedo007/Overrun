using System;
using Lean.Pool;
using Project.Game.Player;
using Project.Game.Projectiles;
using Project.Game.Rooms;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyActionRangedAttack : EnemyActionBase
    {
        [field: SerializeField] public ProjectileData ProjectileData { get; private set; }
        [field: SerializeField] public Vector3 SpawnOffset { get; private set; }
        [field: SerializeField] public float DamageFactor { get; private set; } = 0.5f;
        

        protected override void OnPerform(EnemyController enemy, PlayerController player, RoomManager roomManager, float time,
            Action onCompleteCallback)
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
                enemy.CurrentMeleeDamage * DamageFactor, 
                -1, 
                -1,
                0
            );
            
            onCompleteCallback?.Invoke();
        }
    }
}