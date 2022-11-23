using System.Collections;
using Cinemachine;
using Project.Application;
using Lean.Pool;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace Project.Feedback
{
    public class FeedbackController : MonoBehaviour
    {
        [SerializeField] private float maxHapticRate = 0.05f;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        
        private float _nextHapticTime;
        private bool _currentlyHitStopped;

        private static bool _screenShakeActive = true;

        
        public void Start()
        {
            _screenShakeActive = GetScreenShakeActive();
            HapticController.hapticsEnabled = GetHapticsEnabled();
        }
        
        private void OnEnable()
        {
            FeedbackData.PlayRequested += OnPlayRequested;
        }

        private void OnDisable()
        {
            FeedbackData.PlayRequested -= OnPlayRequested;
        }
        
        private void OnPlayRequested(FeedbackInfo info)
        {
            if (info.Delay > 0)
            {
                StartCoroutine(DelayedFeedback(info));
                return;
            }
            
            var time = Time.time;
            var data = info.Data;
            
            if (data.PlayHaptics && time >= _nextHapticTime)
            {
                HapticPatterns.PlayPreset(data.HapticPreset);
                _nextHapticTime = time + maxHapticRate;
            }

            if (data.PlayFx)
            {
                var position = info.Position;
                
                var fx = LeanPool.Spawn(data.FxPrefab, position, info.Rotation);
                LeanPool.Despawn(fx, data.FxLifetime);
            }

            if (_screenShakeActive && data.PlayScreenShake)
            {
                impulseSource.GenerateImpulseAt(info.Position, Vector3.one * data.Amplitude);
            }

            if (data.PlayHitStop && !_currentlyHitStopped)
            {
                StartCoroutine(HitStopRoutine(data.HitStopTime));
            }
        }

        private IEnumerator DelayedFeedback(FeedbackInfo info)
        {
            yield return new WaitForSeconds(info.Delay);
            info.Delay = -1f;
            OnPlayRequested(info);
        }

        private IEnumerator HitStopRoutine(float time)
        {
            _currentlyHitStopped = true;
            yield return null;
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(time);
            Time.timeScale = 1;
            _currentlyHitStopped = false;
        }

        public static void EnableHaptics(bool isEnabled)
        {
            //HapticController.hapticsEnabled = isEnabled;
            PlayerPrefs.SetInt(PrefKeys.HapticsEnabled, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static bool GetHapticsEnabled()
        {
            return  PlayerPrefs.GetInt(PrefKeys.HapticsEnabled, 1) == 1;
        }

        public static void EnableScreenShake(bool isEnabled)
        {
            _screenShakeActive = isEnabled;
            PlayerPrefs.SetInt(PrefKeys.ScreenShakeEnabled, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static bool GetScreenShakeActive()
        {
            return  PlayerPrefs.GetInt(PrefKeys.ScreenShakeEnabled, 1) == 1;
        }
    }
}