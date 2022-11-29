using System;
using System.Collections;
using Project.Game.Player;
using Project.Game.Rooms;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyActionBase : ScriptableObject
    {
        
        [field: SerializeField] public float Cooldown { get; protected set; }
        [field: SerializeField] public bool CanInterrupt { get; protected set; }

        public IEnumerator ActionRoutine(EnemyController enemy,
            PlayerController player,
            RoomManager roomManager,
            float time,
            Action onCompleteCallback)
        {
            yield return OnPerformActionRoutine(enemy, player, roomManager, time);
            onCompleteCallback?.Invoke();
        }

        protected virtual IEnumerator OnPerformActionRoutine(EnemyController enemy, PlayerController player,
            RoomManager roomManager, float time)
        {
            yield return null;
        }

        public void Interrupt()
        {
            OnInterrupt();
        }

        protected virtual void OnInterrupt()
        {
            
        }
    }
}