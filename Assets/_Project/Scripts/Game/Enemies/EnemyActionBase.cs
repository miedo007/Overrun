using System;
using System.Collections;
using Project.Game.Player;
using Project.Game.Rooms;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyActionBase : MonoBehaviour
    {
        [field: SerializeField] public float Cooldown { get; protected set; }
        [field: SerializeField] public bool CanInterrupt { get; protected set; }

        public void Interrupt()
        {
            OnInterrupt();
        }

        protected virtual void OnInterrupt()
        {
            
        }

        public virtual void Cleanup()
        {
            
        }

        public void Perform(EnemyController enemy,
            PlayerController player,
            RoomManager roomManager,
            float time,
            Action onCompleteCallback)
        {
            OnPerform(enemy, player, roomManager, time, onCompleteCallback);
        }
        
        protected virtual void OnPerform(EnemyController enemy,
            PlayerController player,
            RoomManager roomManager,
            float time,
            Action onCompleteCallback)
        {
        }

        protected virtual IEnumerator OnPerformActionRoutine(EnemyController enemy, PlayerController player,
            RoomManager roomManager, float time, Action onCompleteCallback)
        {
            yield return null;
        }

        
    }
}