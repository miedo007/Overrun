using System;
using System.Collections;
using DG.Tweening;
using Mtl.UiFramework;
using TMPro;
using UnityEngine;

namespace Project.Game.Levels
{
    public class WaveTimerScreen : UIScreen
    {
        public event Action TimerCompleted;
        
        [field: SerializeField] public Animation Animation { get; private set; }
        [field: SerializeField] public AnimationClip DisplayClip { get; private set; }
        [field: SerializeField] public RectTransform Root { get; private set; }
        [field: SerializeField] public AnimationClip HideClip { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TimeText { get; private set; }

        private int _currentTime;
        private readonly WaitForSeconds _waitForSecond = new WaitForSeconds(1);

        public void DisplayWithDuration(int duration)
        {
            _currentTime = duration;
            UpdateClock();
            Animation.clip = DisplayClip;
            Animation.Play();
        }

        private void UpdateClock()
        {
            TimeText.text = $":{_currentTime:00}";
        }

        public void StartTimer()
        {
            StartCoroutine(TimerRoutine());
        }

        private IEnumerator TimerRoutine()
        {
            while (_currentTime > 0)
            {
                yield return _waitForSecond;
                _currentTime--;
                UpdateClock();
                Root.localScale = Vector3.one * 1.25f;
                Root.DOScale(1, 0.125f);
            }
            
            StartCoroutine(HideRoutine());
            TimerCompleted?.Invoke();
        }
        
        public IEnumerator HideRoutine()
        {
            Animation.clip = HideClip;
            Animation.Play();
            yield return WaitForAnimation();
            Close();
        }

        private IEnumerator WaitForAnimation()
        {
            Animation.Play();
            
            while (Animation.IsPlaying(Animation.clip.name))
            {
                yield return null;
            }
        }
        
    }
}