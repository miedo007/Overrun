using System;
using Lean.Pool;
using Project.Game.Player;
using Project.Game.Rooms;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyActionExplode : EnemyActionBase
    {
        [SerializeField] private ExplodingLaunchable explosionPrefab;

        private Action _completeCallback;
        
        protected override void OnPerform(EnemyController enemy, PlayerController player, RoomManager roomManager, float time,
            Action onCompleteCallback)
        {
            var explosion = LeanPool.Spawn(explosionPrefab, transform.position, Quaternion.identity);
            explosion.Initialize(enemy.CurrentMeleeDamage, 0, 0, 0, 1.5f, Vector2.zero);
            explosion.Exploded += OnExplosionComplete;
            _completeCallback = onCompleteCallback;
        }

        private void OnExplosionComplete(ExplodingLaunchable explosion)
        {
            explosion.Exploded -= OnExplosionComplete;
            _completeCallback.Invoke();
        }
    }
}