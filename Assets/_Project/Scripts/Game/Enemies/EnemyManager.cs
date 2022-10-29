using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Pool;
using Mtl.Injection;
using Project.Game.Player;
using Project.Game.Rooms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Game.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public EnemyData[] EnemyDatas { get; private set; }
        [field: SerializeField] public SpawnWarning SpawnWarningPrefab { get; private set; }

        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly RoomManager _roomManager;
        
        public List<EnemyController> ActiveEnemies { get; private set; } = new();

        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(Random.Range(0.3f, 1.25f));
                StartCoroutine(SpawnGroup());
            }
        }

        private IEnumerator SpawnGroup()
        {
            var enemyData = EnemyDatas[Random.Range(0, EnemyDatas.Length)];
            var groupCenter = _roomManager.GetValidPositionInRadius(_playerController.Position, 5);
            var groupSize = enemyData.GetRandomGroupSize();
            
            for (var i = 0; i < groupSize; i++)
            {
                StartCoroutine(SpawnEnemyRoutine(enemyData, groupCenter));
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            }
        }

        private IEnumerator SpawnEnemyRoutine(EnemyData enemyData, Vector3 groupCenter)
        {
            var spawnPoint = _roomManager.GetValidPositionInRadius(groupCenter, 2f);
            var spawnWarning = LeanPool.Spawn(SpawnWarningPrefab, spawnPoint, Quaternion.identity);
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
            LeanPool.Despawn(enemy);
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
}