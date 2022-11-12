using UnityEngine;

namespace Project.Game.Items
{
    public class ItemBehaviour : ScriptableObject
    {
        [field: SerializeField] public bool HasRandomChance { get; private set; } = true;
        [field: SerializeField] public float Chance { get; private set; } = 0.01f;

        public bool Perform(Vector3 position)
        {
            if (!WillPerform())
            {
                return false;
            }
            
            return OnPerform(position);
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

        public virtual void Cleanup()
        {
            
        }
    }
}