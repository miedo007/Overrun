using System.Collections;
using Mtl.UiFramework;
using UnityEngine;

namespace Project.Game.Levels
{
    public class WaveCompleteScreen : UIScreen
    {
        [SerializeField] private new Animation animation;

        protected override void OnOpened()
        {
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