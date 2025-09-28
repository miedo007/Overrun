using System;
using System.Collections;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.UI;
using UnityEngine;

namespace Project.Game.Levels
{
    public class LevelController : MonoBehaviour
    {
        public event Action WaveStarted;
        public event Action WaveCompleted;
        public event Action LevelCompleted;
        public event Action Initialized;

        [field: SerializeField] public LevelDatabase LevelDatabase { get; private set; }

        [Inject] private readonly UIFrame _uiFrame;

        public LevelData CurrentLevel { get; private set; }
        public int CurrentLevelIndex { get; private set; }
        public int WaveIndex { get; private set; }
        public bool IsFinalWave => CurrentLevel.IsLastWave(WaveIndex);
        public int WaveCount => CurrentLevel.Waves.Length;

        public void Initialize(int levelIndex, int waveIndex)
        {
            CurrentLevelIndex = levelIndex;
            CurrentLevel = LevelDatabase.GetLevel(levelIndex);
            WaveIndex = waveIndex;
            Initialized?.Invoke();
        }
        
        public void BeginNextWave(float delay)
        {
            StartCoroutine(BeginWaveRoutine(delay));
        }

        private IEnumerator BeginWaveRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            var timer = _uiFrame.Get<HudScreen>().Timer;
            timer.DisplayWithDuration(CurrentLevel.GetWaveDuration(WaveIndex));
            timer.TimerCompleted += OnTimerCompleted;
            timer.StartTimer();
            
            WaveStarted?.Invoke();
        }

        private void OnTimerCompleted()
        {
            var timer = _uiFrame.Get<HudScreen>().Timer;
            timer.TimerCompleted -= OnTimerCompleted;
            
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

        public void RestartCurrentWave()
        {
            Debug.Log($"[LevelController] Restarting current wave {WaveIndex + 1}");
            
            // Stop any existing timer
            var timer = _uiFrame.Get<HudScreen>().Timer;
            timer.TimerCompleted -= OnTimerCompleted;
            timer.Stop();
            
            // Restart the current wave immediately
            BeginNextWave(0f);
        }

        public void ContinueCurrentWave()
        {
            Debug.Log($"[LevelController] Continuing current wave {WaveIndex + 1} (timer keeps running)");
            
            // Timer continues running - we don't stop or restart it
            // This method is used for revives where we want to continue from where we left off
        }

        #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                var timer = _uiFrame.Get<HudScreen>().Timer;
                timer.Stop();
            }
        }
        
        #endif
    }
}