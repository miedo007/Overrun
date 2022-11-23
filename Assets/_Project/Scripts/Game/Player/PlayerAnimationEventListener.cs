using JetBrains.Annotations;
using Project.Feedback;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerAnimationEventListener : MonoBehaviour
    {
        [SerializeField] private FeedbackData stepFeedback;
        
        [PublicAPI]
        public void Step()
        {
            stepFeedback.Play(transform.position, Quaternion.identity);
        }
    }
}