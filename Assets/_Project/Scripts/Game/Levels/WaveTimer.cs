using System;
using System.Collections;
using DG.Tweening;
using Project.Application;
using TMPro;
using UnityEngine;

namespace Project.Game.Levels
{
    public class WaveTimer : MonoBehaviour
    {
        public event Action TimerCompleted;
        
        [field: SerializeField] public Animation Animation { get; private set; }
        [field: SerializeField] public AnimationClip DisplayClip { get; private set; }
        [field: SerializeField] public RectTransform Root { get; private set; }
        [field: SerializeField] public AnimationClip HideClip { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TimeText { get; private set; }

        private int _currentTime;
        private readonly WaitForSeconds _waitForSecond = new WaitForSeconds(1);

        private void Awake()
        {
            Root.gameObject.SetActive(false);
        }

        public void DisplayWithDuration(int duration)
        {
            Root.gameObject.SetActive(true);
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
                
                if (_currentTime > 5)
                {
                    Root.localScale = Vector3.one * 1.25f;
                    Root.DOScale(1, 0.125f);
                }
                else
                {
                    Root.localScale = Vector3.one * 1.4f;
                    Root.DOScale(1, 0.125f);
                    TimeText.color = Colors.GetColor(Colors.Negative);
                    TimeText.DOColor(Color.white, 0.8f)
                        .SetEase(Ease.OutQuad)
                        .SetDelay(0.125f);
                }
            }

            Stop();
        }
        
        public IEnumerator HideRoutine()
        {
            Animation.clip = HideClip;
            Animation.Play();
            yield return WaitForAnimation();
        }

        private IEnumerator WaitForAnimation()
        {
            Animation.Play();
            
            while (Animation.IsPlaying(Animation.clip.name))
            {
                yield return null;
            }
        }

        public void Stop()
        {
            StopAllCoroutines();
            StartCoroutine(HideRoutine());
            TimerCompleted?.Invoke();
        }
    }
}