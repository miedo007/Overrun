using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using Mtl.Injection;
using Project.Feedback;
using Project.Game.Collectibles;
using Project.Game.Items;
using Project.Game.Levels;
using Project.Game.Player;
using Project.Game.Rooms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Game.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public SpawnWarning SpawnWarningPrefab { get; private set; }
        [field: SerializeField] public ItemBehaviourTrigger EnemyDeathTrigger { get; private set; }
        [field: SerializeField] public FeedbackData DefaultDeathFeedback { get; private set; }

        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly RoomManager _roomManager;
        [Inject] private readonly CollectiblesManager _collectiblesManager;
        [Inject] private readonly GameData _gameData;

        private LevelData _currentLevel;
        private int _currentWaveIndex;
        private int _currentLevelIndex;
        
        public List<EnemyController> ActiveEnemies { get; private set; } = new();

        public void BeginWave(LevelData levelData, int levelIndex, int waveIndex)
        {
            _currentLevel = levelData;
            _currentLevelIndex = levelIndex;
            _currentWaveIndex = waveIndex;
            StartCoroutine(SpawnRoutine());
        }

        public void EndWaveAfterDelay(float delay)
        {
            Invoke(nameof(EndWave), delay);
        }

        public void EndWave()
        {
            StopAllCoroutines();

            // kill all active enemies
            
            for (var index = ActiveEnemies.Count - 1; index >= 0; index--)
            {
                var enemy = ActiveEnemies[index];
                enemy.Killed -= OnEnemyKilled;
                enemy.Cleanup();
            }
            
            ActiveEnemies.Clear();
            
            // kill all inactive enemies
            var inactiveEnemies = GetComponentsInChildren<EnemyController>();
            
            foreach (var enemy in inactiveEnemies)
            {
                enemy.Killed -= OnEnemyKilled;
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
            var delayBetweenGroups = _gameData.InitialSpawnDelay * (1 - (_gameData.SpawnDelayReductionPerWave * _currentWaveIndex));
            delayBetweenGroups = Mathf.Max(delayBetweenGroups, _gameData.MinimumSpawnDelay);
            
            var totalEnemyCount = _currentLevel.GetEnemyCountForWave(_currentWaveIndex);
            var waveDuration = _currentLevel.GetWaveDuration(_currentWaveIndex);
            var spawnGroupCount = Mathf.FloorToInt(waveDuration / delayBetweenGroups) + 1;
            var enemyCountPerGroup = Mathf.CeilToInt(totalEnemyCount / (float) spawnGroupCount);
            
            var spawnSequence = new SpawnSequence(_currentLevel, _currentLevel.GetWaveInfo(_currentWaveIndex));
            
            while (enabled)
            {
                var groupCenter = _playerController.Position;
                var groupRadius = Random.Range(_gameData.EnemySpawnRange.x, _gameData.EnemySpawnRange.y);
                for (int i = 0; i < enemyCountPerGroup; i++)
                {
                    var enemy = spawnSequence.GetNext();
                    StartCoroutine(SpawnEnemyRoutine(enemy, groupCenter, groupRadius));
                }

                var time = delayBetweenGroups;
                while (time > 0)
                {
                    yield return null;
                    time -= Time.deltaTime;
                }
            }
        }
        
        private IEnumerator SpawnEnemyRoutine(EnemyData enemyData, Vector3 groupCenter, float groupRadius)
        {
            var spawnPoint = _roomManager.GetValidPositionInRadius(groupCenter, groupRadius);
            var spawnWarning = LeanPool.Spawn(SpawnWarningPrefab, spawnPoint, Quaternion.identity, transform);
            
            yield return spawnWarning.ShowRoutine();
            
            var enemy = LeanPool.Spawn(enemyData.Prefab, spawnPoint, Quaternion.identity, transform);
            enemy.transform.localScale = Vector3.zero;
            enemy.Initialize(enemyData, _playerController.Position, _currentLevelIndex, _currentWaveIndex);
            enemy.Killed += OnEnemyKilled;
            
            yield return enemy.transform.DOScale(new Vector3(enemy.FacingDirection, 1, 1), 0.2f).WaitForCompletion();
            
            spawnWarning.Hide();
            
            enemy.Activate();
            ActiveEnemies.Add(enemy);
        }
        
        private void OnEnemyKilled(EnemyController enemy)
        {
            enemy.Killed -= OnEnemyKilled;

            if (enemy.Data.DeathFeedback != null)
            {
                enemy.Data.DeathFeedback.Play(enemy.Position, Quaternion.identity);
            }
            else
            {
                DefaultDeathFeedback.Play(enemy.Position, Quaternion.identity);
            }
            _collectiblesManager.SpawnCollectibles(enemy.transform.position, enemy.Data.CollectibleData);
            ActiveEnemies.Remove(enemy);
            
            var position = enemy.Position;
            enemy.Cleanup();
            EnemyDeathTrigger.Trigger(position);
        }

        private void FixedUpdate()
        {
            var dt = Time.fixedDeltaTime;
            var time = Time.time;
            for (var i = 0; i < ActiveEnemies.Count; i++)
            {
                var enemy = ActiveEnemies[i];
                enemy.Step(dt, time, _playerController);
            }
        }
    }

    public class SpawnSequence
    {
        public List<EnemyData> SpawnList { get; private set; }
        public int CurrentIndex { get; private set; }
        public WaveInfo WaveInfo { get; private set; }
        
        public SpawnSequence(LevelData levelData, string spawnCode)
        {
            SpawnList = new List<EnemyData>();

            if (string.IsNullOrEmpty(spawnCode))
            {
                return;
            }
            
            var enemyIndices = new int[spawnCode.Length];
            for (var i = 0; i < spawnCode.Length; i++)
            {
                var indexString = spawnCode.Substring(i, 1);
                if (int.TryParse(indexString, out var enemyIndex))
                {
                    enemyIndex = Mathf.Clamp(enemyIndex, 0, levelData.Enemies.Length - 1);
                    if (enemyIndex < levelData.Enemies.Length)
                    {
                        enemyIndices[i] = enemyIndex;
                    }
                    else
                    {
                        enemyIndices[i] = 0;
                    }
                }
                else
                {
                    enemyIndices[i] = 0;
                }
            }

            foreach (var enemyIndex in enemyIndices)
            {
                SpawnList.Add(levelData.Enemies[enemyIndex]);
            }
        }

        public EnemyData GetNext()
        {
            if (SpawnList.Count <= 0)
            {
                return WaveInfo.GetRandomEnemy();
            }

            var enemyData = SpawnList[CurrentIndex];
            CurrentIndex = (CurrentIndex + 1) % SpawnList.Count;
            return enemyData;
        }
    }
}