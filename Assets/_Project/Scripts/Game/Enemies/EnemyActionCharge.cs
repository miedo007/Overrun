using System;
using System.Collections;
using Project.Game.Player;
using Project.Game.Rooms;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyActionCharge : EnemyActionBase
    {
        [SerializeField] private float speed;
        [SerializeField] private GameObject chargeFx;
        [SerializeField] private float maxChargeDistance = 16;

        private Vector2 _startPosition;
        private float _maxChargeDistanceSqr;
        
        protected override IEnumerator OnPerformActionRoutine(EnemyController enemy, PlayerController player,
            RoomManager roomManager, float time, Action onCompleteCallback)
        {
            enemy.Collider.isTrigger = true;
            var position = enemy.transform.position;
            var targetPosition = player.transform.position;

            var vectorToTarget = (targetPosition- position).normalized;
            enemy.FacePlayer(vectorToTarget);
            
            var previousMass = enemy.Rigidbody.mass;
            enemy.Rigidbody.mass = float.MaxValue;
            enemy.Rigidbody.velocity = vectorToTarget * speed;
            chargeFx.SetActive(true);
            _startPosition = enemy.Rigidbody.position;
            _maxChargeDistanceSqr = maxChargeDistance * maxChargeDistance;

            yield return new WaitUntil(() =>
                IsVelocityZero(enemy.Rigidbody) || 
                HasReachedMaxChargeDistance(enemy.Rigidbody.position));
            
            chargeFx.SetActive(false);
            enemy.Rigidbody.mass = previousMass;
            enemy.Collider.isTrigger = false;
            
            onCompleteCallback?.Invoke();
        }

        private bool IsVelocityZero(Rigidbody2D rb)
        {
            return rb.velocity.sqrMagnitude.Equals(0);
        }

        private bool HasReachedMaxChargeDistance(Vector2 position)
        {
            return (position - _startPosition).sqrMagnitude >= _maxChargeDistanceSqr;
        }

        protected override void OnPerform(EnemyController enemy, PlayerController player, RoomManager roomManager, float time,
            Action onCompleteCallback)
        {
            StartCoroutine(OnPerformActionRoutine(enemy, player, roomManager, time, onCompleteCallback));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            chargeFx.SetActive(false);
        }
    }
}