using UnityEngine;

namespace Project.Game.Enemies
{
    [CreateAssetMenu(fileName = "data_enemy_", menuName = "Data/Enemies/EnemyData", order = 0)]
    public class EnemyData : ScriptableObject
    {
        [field: SerializeField] public EnemyController Prefab { get; private set; }
        [field: SerializeField] public float BaseHealth { get; private set; } = 3;
        
    }
}