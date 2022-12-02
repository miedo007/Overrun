using System;
using System.Collections;
using Project.Feedback;
using Project.Game.Player;
using Project.Game.Rooms;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyActionBase : MonoBehaviour
    {
        [field: SerializeField] public bool CanInterrupt { get; protected set; }
        [field: SerializeField] public FeedbackData ActivateFeedback { get; protected set; }

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
            if (ActivateFeedback != null)
            {
                ActivateFeedback.Play(transform.position, Quaternion.identity);
            }
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