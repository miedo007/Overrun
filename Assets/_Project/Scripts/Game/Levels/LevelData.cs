using Project.Game.Enemies;
using UnityEngine;

namespace Project.Game.Levels
{
    [CreateAssetMenu(fileName = "data_level_", menuName = "Data/Levels/LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public WaveInfo[] Waves { get; private set; }

        public WaveInfo GetWaveInfo(int index)
        {
            return Waves[index];
        }

        public bool IsLastWave(int waveIndex)
        {
            return waveIndex == Waves.Length - 1;
        }
    }

    [System.Serializable]
    public class WaveInfo
    {
        [field: SerializeField] public string SpawnCode { get; private set; } = "a3,b1,_,a2,a3,_,a2,b1,b1,_,a2,a3,b1,b1";
        [field: SerializeField] public EnemyData[] Enemies { get; private set; }
        [field: SerializeField] public int Duration { get; private set; } = 30;
    }
}