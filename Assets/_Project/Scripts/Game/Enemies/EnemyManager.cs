using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using Mtl.Injection;
using Project.Game.Levels;
using Project.Game.Player;
using Project.Game.Rooms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Game.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public EnemyDatabase Enemies { get; private set; }
        [field: SerializeField] public SpawnWarning SpawnWarningPrefab { get; private set; }

        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly RoomManager _roomManager;

        private SpawnSequence _spawnSequence;
        private WaveInfo _currentWave;
        
        public List<EnemyController> ActiveEnemies { get; private set; } = new();

        public void BeginWave(WaveInfo currentWaveInfo)
        {
            _currentWave = currentWaveInfo;
            _spawnSequence = new SpawnSequence(Enemies, _currentWave.SpawnCode);
            
            StartCoroutine(SpawnRoutine());
        }

        public void EndWave()
        {
            StopAllCoroutines();

            // kill all active enemies
            
            for (var index = ActiveEnemies.Count - 1; index >= 0; index--)
            {
                var enemy = ActiveEnemies[index];
                enemy.Cleanup();
            }
            
            ActiveEnemies.Clear();
            
            // kill all inactive enemies
            var inactiveEnemies = GetComponentsInChildren<EnemyController>();
            
            foreach (var enemy in inactiveEnemies)
            {
                enemy.Cleanup();
            }
            
            // clean up spawn warnings
            var spawnWarnings = GetComponentsInChildren<SpawnWarning>();
            
            foreach (var warning in spawnWarnings)
            {
                warning.Hide();
            }
        }

        private IEnumerator SpawnRoutine()
        {
            var currentIntervalDelay = 2f;
            
            while (enabled)
            {
                float delay;
                foreach (var spawnItem in _spawnSequence.SpawnItems)
                {
                    if (spawnItem.IsDelay)
                    {
                        delay = currentIntervalDelay * spawnItem.Quantity;
                        while (delay > 0)
                        {
                            yield return null;
                            delay -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        StartCoroutine(SpawnGroup(spawnItem as SpawnEnemyGroup));
                    }
                }
                
                // Wait before next spawn loop
                
                delay = currentIntervalDelay;
                while (delay > 0)
                {
                    yield return null;
                    delay -= Time.deltaTime;
                }
                
                // Scale the delay between intervals for each loop through the spawn sequence
                
                currentIntervalDelay *= 0.95f;
            }
        }

        private IEnumerator SpawnGroup(SpawnEnemyGroup spawnEnemyGroup)
        {
            var enemyData = spawnEnemyGroup.EnemyData;
            var groupCenter = _roomManager.GetValidPositionInRadius(_playerController.Position, 5);
            
            for (var i = 0; i < spawnEnemyGroup.Quantity; i++)
            {
                StartCoroutine(SpawnEnemyRoutine(enemyData, groupCenter));

                var delay = Random.Range(0.1f, 0.2f);
                while (delay > 0)
                {
                    yield return null;
                    delay -= Time.deltaTime;
                }
            }
        }

        private IEnumerator SpawnEnemyRoutine(EnemyData enemyData, Vector3 groupCenter)
        {
            var spawnPoint = _roomManager.GetValidPositionInRadius(groupCenter, 2f);
            var spawnWarning = LeanPool.Spawn(SpawnWarningPrefab, spawnPoint, Quaternion.identity, transform);
            
            yield return spawnWarning.ShowRoutine();
            
            var enemy = LeanPool.Spawn(enemyData.Prefab, spawnPoint, Quaternion.identity, transform);
            enemy.transform.localScale = Vector3.zero;
            enemy.Initialize(enemyData, _playerController.Position);
            enemy.Killed += OnEnemyKilled;
            
            yield return enemy.transform.DOScale(1, 0.2f).WaitForCompletion();
            
            spawnWarning.Hide();
            
            enemy.Activate();
            ActiveEnemies.Add(enemy);
        }


        private void OnEnemyKilled(EnemyController enemy)
        {
            enemy.Killed -= OnEnemyKilled;
            ActiveEnemies.Remove(enemy);
            enemy.Cleanup();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            var time = Time.time;
            for (var i = 0; i < ActiveEnemies.Count; i++)
            {
                var enemy = ActiveEnemies[i];
                enemy.Step(dt, time, _playerController.Position);
            }
        }
    }

    public class SpawnSequence
    {
        public List<SpawnItem> SpawnItems { get; private set; }
        
        public SpawnSequence(EnemyDatabase enemies, string spawnCode)
        {
            SpawnItems = new List<SpawnItem>();
            var intervals = spawnCode.Split(',');
            foreach (var interval in intervals)
            {
                // is this a delay interval or enemy group
                if (interval[0] == '_')
                {
                    SpawnItems.Add(new SpawnSequenceDelay
                    {
                        Quantity = interval.Length
                    });
                }
                else
                {
                    var enemyCode = interval[0];
                    var enemyCount = int.Parse(interval.Substring(1,interval.Length -1));

                    var spawnItem = new SpawnEnemyGroup
                    {
                        EnemyData = enemies.GetEnemyWithCode(enemyCode),
                        Quantity = enemyCount
                    };

                    SpawnItems.Add(spawnItem);
                }
            }
        }
    }
    
    public class SpawnItem
    {
        public bool IsDelay { get; protected set; }
        public int Quantity { get; set; }
    }
    
    public class SpawnEnemyGroup : SpawnItem
    {
        public EnemyData EnemyData;
    }
    
    public class SpawnSequenceDelay : SpawnItem
    {
        public SpawnSequenceDelay()
        {
            IsDelay = true;
        }
    }
}