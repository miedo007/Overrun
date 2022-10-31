using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Game.Enemies
{
    [CreateAssetMenu(fileName = "database_enemies_", menuName = "Data/Enemies/EnemyDatabase", order = 0)]
    public class EnemyDatabase : ScriptableObject
    {
        [field: SerializeField] public List<EnemyData> Enemies { get; private set; }

        public EnemyData GetRandom()
        {
            return Enemies[Random.Range(0, Enemies.Count)];
        }

        public EnemyData GetEnemyWithCode(char enemyCode)
        {
            var enemy = Enemies.FirstOrDefault(x => x.Code == enemyCode);
            if (enemy == null)
            {
                enemy = Enemies[0];
                Debug.LogWarning($"Invalid spawn code :: {enemyCode} :: Replacing with default enemy");
            }

            return enemy;
        }
    }
}