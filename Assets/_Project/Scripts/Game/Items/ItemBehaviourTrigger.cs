using System;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "item_behaviour_trigger_", menuName = "Data/Items/BehaviourTrigger", order = 0)]
    public class ItemBehaviourTrigger : ScriptableObject
    {
        public event Action<ItemBehaviourTrigger, Vector3> Triggered;

        [field: SerializeField, TextArea] public string Description { get; private set; }
        
        public void Trigger(Vector3 position)
        {
            Triggered?.Invoke(this, position);
        }
    }
}