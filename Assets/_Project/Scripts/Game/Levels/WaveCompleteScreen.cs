using System.Collections;
using Mtl.UiFramework;
using Project.Game.UI;
using TMPro;
using UnityEngine;

namespace Project.Game.Levels
{
    public class WaveCompleteScreen : UIScreen
    {
        [SerializeField] private new Animation animation;
        [SerializeField] private AnimationClip openClip;
        [SerializeField] private AnimationClip closeClip;
        [SerializeField] private CurrencyRewardView currencyRewardView;
        [SerializeField] private TextMeshProUGUI completionTypeText;
        
        
        protected override void OnOpened()
        {
            StartCoroutine(WaitForAnimation());
        }

        public void Initialize(int currencyReward, bool isLevelComplete = false)
        {
            if (isLevelComplete)
            {
                completionTypeText.text = "LEVEL";
            }
            
            currencyRewardView.SetValue(currencyReward);
        }

        private IEnumerator WaitForAnimation()
        {
            animation.clip = openClip;
            animation.Play();
            
            while (animation.IsPlaying(openClip.name))
            {
                yield return null;
            }

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            
            animation.clip = closeClip;
            animation.Play();
            
            while (animation.IsPlaying(closeClip.name))
            {
                yield return null;
            }
            
            Close();
        }
    }
}