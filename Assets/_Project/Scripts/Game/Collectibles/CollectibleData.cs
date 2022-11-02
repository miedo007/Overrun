using System;
using UnityEngine;

namespace Project.Game.Collectibles
{
    [CreateAssetMenu(fileName = "data_collectible_", menuName = "Data/Collectibles/CollectibleData", order = 0)]
    public class CollectibleData : ScriptableObject
    {
        public event Action<CollectibleData> Collected;
        
        [field: SerializeField] public float Value { get; private set; } = 1;
        [field: SerializeField] public Collectible Prefab { get; private set; }

        public void Collect()
        {
            Collected?.Invoke(this);
        }
    }
}