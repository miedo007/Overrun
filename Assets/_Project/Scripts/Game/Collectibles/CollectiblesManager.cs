using Lean.Pool;
using UnityEngine;

namespace Project.Game.Collectibles
{
    public class CollectiblesManager : MonoBehaviour
    {
        [field: SerializeField] public CollectibleData DefaultCollectible { get; private set; }
        
        public void SpawnCollectibles(Vector3 position, CollectibleData collectibleData)
        {
            if (collectibleData == null)
            {
                collectibleData = DefaultCollectible;
            }

            var collectible = LeanPool.Spawn(collectibleData.Prefab, position,
                Quaternion.Euler(0, 0, Random.value * 360f));
            
            collectible.Initialize(collectibleData);
        }
    }
}