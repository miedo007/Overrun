using System.Collections;
using Mtl.UiFramework;
using TMPro;
using UnityEngine;

namespace Project.Game.Levels
{
    public class WaveIntroScreen : UIScreen
    {
        [SerializeField] private new Animation animation;
        [SerializeField] private TextMeshProUGUI waveIndexText;

        public void DisplayWithWaveIndex(int waveIndex, int waveCount, bool isFinalWave)
        {
            //waveIndexText.text = isFinalWave ? "FINAL\nWAVE!" : $"WAVE\n<size=150%>{waveIndex + 1}</size> of <size=150%>{waveCount}</size>";
            waveIndexText.text = isFinalWave ? "FINAL\nWAVE!" : $"WAVE\n<size=150%>{waveIndex + 1}";
            StartCoroutine(WaitForAnimation());
        }

        private IEnumerator WaitForAnimation()
        {
            animation.Play();
            
            while (animation.IsPlaying(animation.clip.name))
            {
                yield return null;
            }
            
            Close();
        }
    }
}