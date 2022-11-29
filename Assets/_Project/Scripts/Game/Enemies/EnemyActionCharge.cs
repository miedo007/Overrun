using System.Collections;
using Project.Game.Player;
using Project.Game.Rooms;
using UnityEngine;

namespace Project.Game.Enemies
{
    [CreateAssetMenu(fileName = "enemy_behaviour_charge_", menuName = "Data/Enemies/Charge", order = 0)]
    public class EnemyActionCharge : EnemyActionBase
    {
        [SerializeField] private float speed;
        
        protected override IEnumerator OnPerformActionRoutine(EnemyController enemy, PlayerController player,
            RoomManager roomManager, float time)
        {
            var position = enemy.transform.position;
            var targetPosition = player.transform.position;

            var vectorToTarget = (targetPosition- position).normalized;
            enemy.FacePlayer(vectorToTarget);
            
            var previousMass = enemy.Rigidbody.mass;
            enemy.Rigidbody.mass = float.MaxValue;
            enemy.Rigidbody.velocity = vectorToTarget * speed;
            
            yield return new WaitUntil(()=>enemy.Rigidbody.velocity.sqrMagnitude.Equals(0));
            
            enemy.Rigidbody.mass = previousMass;
        }
    }
}