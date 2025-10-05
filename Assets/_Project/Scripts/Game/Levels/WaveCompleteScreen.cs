using System;
using System.Collections;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Levels
{
    public class WaveCompleteScreen : UIScreen
    {
        [SerializeField] private new Animation animation;
        [SerializeField] private AnimationClip openClip;
        [SerializeField] private AnimationClip closeClip;
        [SerializeField] private CurrencyRewardView currencyRewardView;
        [SerializeField] private TextMeshProUGUI completionTypeText;

        [Header("Double Reward (Level Complete Only)")]
        [SerializeField] private GameObject doubleRewardPanel;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button doubleRewardButton;

        [Inject] private readonly UIFrame _uiFrame;
        
        public event Action<bool> DoubleRewardClicked; // bool = wasDoubled
        
        private int _currentWave;
        private bool _isLevelComplete;
        private int _baseReward;
        private bool _rewardWasDoubled = false;
        
        private void Awake()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueButtonClicked);
            }
            
            if (doubleRewardButton != null)
            {
                doubleRewardButton.onClick.AddListener(OnDoubleRewardButtonClicked);
            }
        }
        
        protected override void OnOpened()
        {
            Debug.Log($"[WaveCompleteScreen] OnOpened - currentWave: {_currentWave}, isLevelComplete: {_isLevelComplete}");
            _rewardWasDoubled = false;
            StartCoroutine(WaitForAnimation());
        }

        public void Initialize(int currencyReward, bool isLevelComplete = false, int currentWave = 0)
        {
            _uiFrame.Get<HudScreen>().ShowCurrencyBar();
            
            _currentWave = currentWave;
            _isLevelComplete = isLevelComplete;
            _baseReward = currencyReward;
            
            if (isLevelComplete)
            {
                completionTypeText.text = "LEVEL";
            }
            
            currencyRewardView.SetValue(currencyReward);
            
            // Show double reward option only for level complete
            if (doubleRewardPanel != null)
            {
                doubleRewardPanel.SetActive(isLevelComplete);
            }
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
            
            Debug.Log($"[WaveCompleteScreen] Just completed wave {_currentWave}, isLevelComplete: {_isLevelComplete}, shouldShowAd: {shouldShowAd}");
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

            if (_isLevelComplete && doubleRewardPanel != null)
            {
                // For level complete, show buttons and wait for user choice
                yield return new WaitUntil(() => _shouldClose);
            }
            else
            {
                // For wave complete, wait for click as before
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            }
            
            animation.clip = closeClip;
            animation.Play();
            
            while (animation.IsPlaying(closeClip.name))
            {
                yield return null;
            }
            
            _uiFrame.Get<HudScreen>().HideCurrencyBar();
            
            Close();
        }

        private bool _shouldClose = false;

        private void OnContinueButtonClicked()
        {
            Debug.Log("[WaveCompleteScreen] Continue button clicked - proceeding with normal reward");
            DoubleRewardClicked?.Invoke(false);
            _shouldClose = true;
        }

        private void OnDoubleRewardButtonClicked()
        {
            Debug.Log("[WaveCompleteScreen] Double reward button clicked - showing rewarded ad");
            
            // Disable buttons during ad
            if (continueButton != null) continueButton.interactable = false;
            if (doubleRewardButton != null) doubleRewardButton.interactable = false;
            
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    Debug.Log("[WaveCompleteScreen] ✅ Rewarded ad completed - doubling reward!");
                    _rewardWasDoubled = true;
                    
                    // Update the displayed reward
                    var doubledReward = _baseReward * 2;
                    currencyRewardView.SetValue(doubledReward);
                    
                    DoubleRewardClicked?.Invoke(true);
                    _shouldClose = true;
                },
                onAdFailed: () => {
                    Debug.Log("[WaveCompleteScreen] ❌ Rewarded ad failed - restoring button state");
                    // Restore button state
                    if (continueButton != null) continueButton.interactable = true;
                    if (doubleRewardButton != null) doubleRewardButton.interactable = true;
                }
            );
        }
    }
}