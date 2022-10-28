using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Mtl.Injection;
using Project.Game.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Game.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public EnemyData[] EnemyDatas { get; private set; }

        [Inject] private readonly PlayerController _playerController;
        
        public List<EnemyController> ActiveEnemies { get; private set; } = new();

        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(.33f);

                var enemyData = EnemyDatas[Random.Range(0, EnemyDatas.Length)];
                var enemy = LeanPool.Spawn(enemyData.Prefab, Random.insideUnitCircle * 6, Quaternion.identity,
                    transform);

                enemy.Initialize(enemyData);
                enemy.Killed += OnEnemyKilled;
                
                ActiveEnemies.Add(enemy);
            }
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