using Project.Feedback;
using UnityEngine;

namespace Project.Game.Items
{
    public class ItemBehaviour : ScriptableObject
    {
        [field: SerializeField] public bool HasRandomChance { get; private set; } = true;
        [field: SerializeField] public float Chance { get; private set; } = 0.01f;
        [field: SerializeField] public FeedbackData activationFeedback { get; private set; }

        public bool Perform(Vector3 position)
        {
            if (!WillPerform())
            {
                return false;
            }
            
            if (OnPerform(position))
            {
                if (activationFeedback != null)
                {
                    activationFeedback.Play(GetFeedbackPosition(position), Quaternion.identity);
                }
                return true;
            }

            return false;
        }
        
        public virtual bool OnPerform(Vector3 position)
        {
            return false;
        }

        public bool WillPerform()
        {
            if (!HasRandomChance)
            {return true;}

            return Random.value < Chance;
        }

        public virtual string GetDescription()
        {
            return name;
        }

        public virtual Vector3 GetFeedbackPosition(Vector3 defaultPosition)
        {
            return defaultPosition;
        }

        public virtual void Cleanup()
        {
            
        }
        
        protected string GetChanceDisplay()
        {
            return $"{Chance * 100:0.0}% chance";
        }
    }
}