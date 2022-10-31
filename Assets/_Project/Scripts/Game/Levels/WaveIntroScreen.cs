using System;
using System.Collections;
using Mtl.UiFramework;
using TMPro;
using UnityEngine;

namespace Project.Game.Levels
{
    public class WaveIntroScreen : UIScreen
    {
        public event Action IntroCompleted;

        [SerializeField] private new Animation animation;
        [SerializeField] private TextMeshProUGUI waveIndexText;

        public void DisplayWithWaveIndex(int waveIndex)
        {
            waveIndexText.text = $"{waveIndex + 1}";
            StartCoroutine(WaitForAnimation());
        }

        private IEnumerator WaitForAnimation()
        {
            animation.Play();
            
            while (animation.IsPlaying(animation.clip.name))
            {
                yield return null;
            }
            
            IntroCompleted?.Invoke();
        }
    }
}