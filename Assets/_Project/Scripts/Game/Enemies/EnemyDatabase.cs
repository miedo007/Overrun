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
    }
}