using Project.Game.Enemies;
using UnityEngine;

namespace Project.Game.Levels
{
    [CreateAssetMenu(fileName = "data_level_", menuName = "Data/Levels/LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public int BaseEnemyCount { get; private set; } = 60;
        [field: SerializeField] public int EnemyCountIncreasePerWave { get; private set; } = 15;
        [field: SerializeField] public int BaseWaveDuration { get; private set; } = 45;
        [field: SerializeField] public int WaveDurationIncrease { get; private set; } = 10;
        [field: SerializeField] public int MaxWaveDuration { get; private set; } = 60;
        [field: SerializeField] public WaveInfo[] Waves { get; private set; }
        
        public float GetSpawnDelay(int waveIndex)
        {
            return (float) BaseWaveDuration / GetEnemyCountForWave(waveIndex);
        }

        public int GetEnemyCountForWave(int waveIndex)
        {
            return BaseEnemyCount + (EnemyCountIncreasePerWave * waveIndex);
        }

        public WaveInfo GetWaveInfo(int index)
        {
            return Waves[index];
        }

        public bool IsLastWave(int waveIndex)
        {
            return waveIndex == Waves.Length - 1;
        }

        public int GetWaveDuration(int waveIndex)
        {
            return Mathf.Min(MaxWaveDuration, BaseWaveDuration + (WaveDurationIncrease * waveIndex));
        }
    }

    [System.Serializable]
    public class WaveInfo
    {
        [field: SerializeField] public EnemyData[] Enemies { get; private set; }
        [field: SerializeField] public string SpawnCode { get; private set; } = "000";

        public EnemyData GetRandomEnemy()
        {
            return Enemies[Random.Range(0, Enemies.Length)];
        }
    }
}