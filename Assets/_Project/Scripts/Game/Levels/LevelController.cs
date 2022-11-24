using System;
using System.Collections;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.Enemies;
using UnityEngine;

namespace Project.Game.Levels
{
    public class LevelController : MonoBehaviour
    {
        public event Action WaveStarted;
        public event Action WaveCompleted;
        public event Action LevelCompleted;

        [field: SerializeField] public LevelDatabase Levels { get; private set; }

        [Inject] private readonly UIFrame _uiFrame;

        public LevelData CurrentLevel { get; private set; }
        public int CurrentLevelIndex { get; private set; }
        public int WaveIndex { get; private set; }
        public WaveInfo CurrentWaveInfo{ get; private set; }
        public bool IsFinalWave { get; private set; }

        public void BeginNextWave(int levelIndex, float delay)
        {
            CurrentLevelIndex = levelIndex;
            CurrentLevel = Levels.GetLevel(levelIndex);
            IsFinalWave = WaveIndex == CurrentLevel.Waves.Length - 1;
            StartCoroutine(BeginWaveRoutine(delay));
        }

        private IEnumerator BeginWaveRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            CurrentWaveInfo = CurrentLevel.GetWaveInfo(WaveIndex);

            var timerScreen = _uiFrame.Open<WaveTimerScreen>();
            timerScreen.DisplayWithDuration(CurrentLevel.GetWaveDuration(WaveIndex));
            timerScreen.TimerCompleted += OnTimerCompleted;
            timerScreen.StartTimer();
            
            WaveStarted?.Invoke();
        }

        private void OnTimerCompleted()
        {
            var timerScreen = _uiFrame.Get<WaveTimerScreen>();
            timerScreen.Close();
            timerScreen.TimerCompleted -= OnTimerCompleted;
            
            if (CurrentLevel.IsLastWave(WaveIndex))
            {
                LevelCompleted?.Invoke();
            }
            else
            {
                WaveCompleted?.Invoke();
                WaveIndex++;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                OnTimerCompleted();
            }
        }
    }
}