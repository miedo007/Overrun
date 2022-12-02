using NaughtyAttributes;
using UnityEngine;

namespace Project.Game.Levels
{
    [CreateAssetMenu(fileName = "database_levels_", menuName = "Data/Levels/LevelDatabase", order = 0)]
    public class LevelDatabase : ScriptableObject
    {
        [field: SerializeField] public LevelData[] Levels { get; private set; }
        [field: SerializeField] public LevelData[] RepeatedLevels { get; private set; }
        [field: SerializeField, Header("EDITOR ONLY")] public bool IsTesting { get; set; }
        [field: SerializeField] public LevelData TestLevel { get; set; }
        

        public LevelData GetLevel(int index)
        {
#if UNITY_EDITOR
            if (IsTesting)
            {
                return TestLevel;
            }
#endif

            if (index < Levels.Length)
            {
                return Levels[index];
            }
            
            return RepeatedLevels[index % RepeatedLevels.Length];
        }
    }
}