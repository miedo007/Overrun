using System.Collections;
using Mtl.Injection;
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

        [Inject] private readonly UIFrame _uiFrame;
        
        private int _currentWave;
        private bool _isLevelComplete;
        
        protected override void OnOpened()
        {
            Debug.Log($"[WaveCompleteScreen] OnOpened - currentWave: {_currentWave}, isLevelComplete: {_isLevelComplete}");
            StartCoroutine(WaitForAnimation());
        }

        public void Initialize(int currencyReward, bool isLevelComplete = false, int currentWave = 0)
        {
            _uiFrame.Get<HudScreen>().ShowCurrencyBar();
            
            _currentWave = currentWave;
            _isLevelComplete = isLevelComplete;
            
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

            // Check if we should show a midgame ad (but not on level complete)
            bool shouldShowAd = MidgameAds.ShouldShowAd(_currentWave, _isLevelComplete);
            
            Debug.Log($"[WaveCompleteScreen] Wave {_currentWave}, isLevelComplete: {_isLevelComplete}, shouldShowAd: {shouldShowAd}");
            Debug.Log($"[WaveCompleteScreen] Ad debug info: {MidgameAds.GetDebugInfo()}");
            
            if (shouldShowAd)
            {
                Debug.Log($"[WaveCompleteScreen] Showing midgame ad after wave {_currentWave}");
                
                bool adCompleted = false;
                MidgameAds.ShowAd(
                    onAdFinished: () => adCompleted = true,
                    onAdFailed: () => adCompleted = true
                );
                
                // Wait for ad to complete
                yield return new WaitUntil(() => adCompleted);
            }

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            
            animation.clip = closeClip;
            animation.Play();
            
            while (animation.IsPlaying(closeClip.name))
            {
                yield return null;
            }
            
            _uiFrame.Get<HudScreen>().HideCurrencyBar();
            
            Close();
        }
    }
}