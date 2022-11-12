using UnityEngine;

namespace Project.Game.Items
{
    public class ItemBehaviour : ScriptableObject
    {
        [field: SerializeField] public string Description { get; private set; }

        public virtual void Perform()
        {
            
        }

        public virtual string GetDescription()
        {
            return Description;
        }
    }
}