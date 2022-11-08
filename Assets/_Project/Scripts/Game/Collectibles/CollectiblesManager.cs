using Lean.Pool;
using Mtl.Injection;
using Project.Game.Levels;
using UnityEngine;

namespace Project.Game.Collectibles
{
    public class CollectiblesManager : MonoBehaviour
    {
        [field: SerializeField] public CollectibleData DefaultCollectible { get; private set; }
        [field: SerializeField] public CollectibleData DefaultCollectibleFinalWave { get; private set; }

        [Inject] private readonly LevelController _levelController;

        public void SpawnCollectibles(Vector3 position, CollectibleData collectibleData)
        {
            if (collectibleData == null)
            {
                collectibleData = _levelController.IsFinalWave ? DefaultCollectibleFinalWave : DefaultCollectible;
            }

            var collectible = LeanPool.Spawn(collectibleData.Prefab, position,
                Quaternion.Euler(0, 0, Random.value * 360f));
            
            collectible.Initialize(collectibleData);
        }
    }
}