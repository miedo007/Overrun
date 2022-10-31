using UnityEngine;

namespace Project.Game.Levels
{
    [CreateAssetMenu(fileName = "database_levels_", menuName = "Data/Levels/LevelDatabase", order = 0)]
    public class LevelDatabase : ScriptableObject
    {
        [field: SerializeField] public LevelData[] Levels { get; private set; }

        public LevelData GetLevel(int index)
        {
            return Levels[index % Levels.Length];
        }
    }
}