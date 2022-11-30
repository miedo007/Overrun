using NaughtyAttributes;
using Project.Game.Enemies;
using Project.Game.Rooms;
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
        [field: SerializeField] public int WaveDurationIncreaseRate { get; private set; } = 3;
        [field: SerializeField] public int MaxWaveDuration { get; private set; } = 60;
        [field: SerializeField] public EnemyData[] Enemies { get; private set; }
        [field: SerializeField] public string[] Waves { get; private set; }
        [field: SerializeField] public RoomData RoomData { get; set; }

        public float GetSpawnDelay(int waveIndex)
        {
            return (float) BaseWaveDuration / GetEnemyCountForWave(waveIndex);
        }

        public int GetEnemyCountForWave(int waveIndex)
        {
            return BaseEnemyCount + (EnemyCountIncreasePerWave * waveIndex);
        }

        public string GetWaveInfo(int index)
        {
            return Waves[index];
        }

        public bool IsLastWave(int waveIndex)
        {
            return waveIndex == Waves.Length - 1;
        }

        public int GetWaveDuration(int waveIndex)
        {
            var waveDurationIncreaseCount = waveIndex / WaveDurationIncreaseRate;
            return Mathf.Min(MaxWaveDuration, BaseWaveDuration + (WaveDurationIncrease * waveDurationIncreaseCount));
        }
        
        #if UNITY_EDITOR
        [Button("Test Level")]
        public void TestLevel()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            var assets = UnityEditor.AssetDatabase.FindAssets("database_levels_default");
            var levelDatabasePath = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]);
            var levelDatabase = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelDatabase>(levelDatabasePath);
            levelDatabase.TestLevel = this;
            levelDatabase.IsTesting = true;
            UnityEditor.EditorApplication.isPlaying = true;
        }
        #endif
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