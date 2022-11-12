using System;
using Project.Game.Items;
using UnityEngine;

namespace Project.Game.Collectibles
{
    [CreateAssetMenu(fileName = "data_collectible_", menuName = "Data/Collectibles/CollectibleData", order = 0)]
    public class CollectibleData : ScriptableObject
    {
        public event Action<CollectibleData> Collected;
        
        [field: SerializeField] public float Value { get; private set; } = 1;
        [field: SerializeField] public bool AutoCollectOnWaveComplete { get; private set; } = true;
        [field: SerializeField] public Collectible Prefab { get; private set; }
        [field: SerializeField] public ItemBehaviourTrigger BehaviourTrigger { get; private set; }

        public void Collect(Vector3 position)
        {
            if (BehaviourTrigger != null)
            {
                BehaviourTrigger.Trigger(position);
            }
            Collected?.Invoke(this);
        }
    }
}