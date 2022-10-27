using System.Collections;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Game.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [field: SerializeField] public EnemyData[] EnemyDatas { get; private set; }

        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(.5f);

                var enemyData = EnemyDatas[Random.Range(0, EnemyDatas.Length)];
                var enemy = LeanPool.Spawn(enemyData.Prefab, Random.insideUnitCircle * 6, Quaternion.identity,
                    transform);

                enemy.Initialize(enemyData);
            }
        }
    }
}