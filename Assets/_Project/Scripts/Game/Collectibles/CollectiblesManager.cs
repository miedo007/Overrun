using System;
using System.Collections.Generic;
using Lean.Pool;
using Mtl.Injection;
using Project.Game.Levels;
using Project.Heroes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Game.Collectibles
{
    public class CollectiblesManager : MonoBehaviour
    {
        [field: SerializeField] public CollectibleData DefaultCollectible { get; private set; }
        [field: SerializeField] public CollectibleData DefaultCollectibleFinalWave { get; private set; }
        [field: SerializeField] public CollectibleData CollectibleContainer { get; private set; }
        
        [Inject] private readonly LevelController _levelController;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly HeroInfo _heroInfo;

        private int _containersDropped;
        
        private void Start()
        {
            _levelController.WaveStarted += OnWaveStarted;
            CollectibleContainer.Collected += OnContainerCollected;
        }

        private void OnContainerCollected(CollectibleData data)
        {
            _heroInfo.WaveRewards++;
        }

        private void OnWaveStarted()
        {
            _containersDropped = 0;
        }

        public void SpawnCollectibles(Vector3 position, CollectibleData collectibleData)
        {
            if (collectibleData == null)
            {
                if (_levelController.IsFinalWave)
                {
                    collectibleData = DefaultCollectibleFinalWave;
                }
                else if (_containersDropped < _gameData.MaxContainersPerWave && Random.Range(0f, 200f) < 2f)
                {
                    collectibleData = CollectibleContainer;
                    _containersDropped++;
                }
                else
                {
                    collectibleData = DefaultCollectible;
                }
            }

            var rotation = collectibleData.Prefab.IsRotatable
                ? Quaternion.Euler(0, 0, Random.value * 360f)
                : Quaternion.identity;

            var collectible = LeanPool.Spawn(collectibleData.Prefab, position, rotation);
            collectible.Initialize(collectibleData);
        }
    }
}