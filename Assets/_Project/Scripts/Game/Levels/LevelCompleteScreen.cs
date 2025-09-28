using System;
using System.Collections;
using DG.Tweening;
using Mtl.UiFramework;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Project.Game.Levels
{
    public class LevelCompleteScreen : UIScreen
    {
        [field: SerializeField] public Button ConfirmButton { get; private set; }
        [field: SerializeField] public Animation Animation { get; private set; }

        [Header("Double Reward Feature")]
        [SerializeField] private GameObject doubleRewardPanel;
        [SerializeField] private Button doubleRewardButton;
        [SerializeField] private TextMeshProUGUI rewardAmountText; // Display total reward amount

        public event Action<bool> DoubleRewardClicked; // bool = wasDoubled

        private Vector3 _buttonScale;
        private bool _shouldClose = false;
        private int _totalReward = 0;
        
        public void Initialize(int totalReward)
        {
            _totalReward = totalReward;
            UpdateRewardDisplay();
            Debug.Log($"[LevelCompleteScreen] Initialized with total reward: {totalReward}");
        }

        private void UpdateRewardDisplay()
        {
            if (rewardAmountText != null)
            {
                rewardAmountText.text = _totalReward.ToString();
                Debug.Log($"[LevelCompleteScreen] Updated reward display to: {_totalReward}");
            }
            else
            {
                Debug.LogWarning("[LevelCompleteScreen] rewardAmountText is null - cannot display reward amount!");
            }
        }
        
        private void Awake()
        {
            // Original confirm button - handles "continue with normal reward"
            if (ConfirmButton != null)
            {
                ConfirmButton.onClick.AddListener(OnContinueButtonClicked);
                _buttonScale = ConfirmButton.transform.localScale;
                ConfirmButton.transform.localScale = Vector3.zero;
            }
            
            // Double reward button - handles "watch ad for double reward"
            if (doubleRewardButton != null)
            {
                doubleRewardButton.onClick.AddListener(OnDoubleRewardButtonClicked);
                doubleRewardButton.transform.localScale = Vector3.zero; // Start hidden like confirm button
            }
        }
        
        protected override void OnOpened()
        {
            _shouldClose = false;
            StartCoroutine(WaitForAnimation());
        }

        private IEnumerator WaitForAnimation()
        {
            Animation.Play();
            
            while (Animation.IsPlaying(Animation.clip.name))
            {
                yield return null;
            }

            // Check if we have double reward system available
            if (doubleRewardButton != null)
            {
                // Show both buttons with animations (like original confirm button)
                if (ConfirmButton != null)
                {
                    ConfirmButton.transform.DOScale(_buttonScale, 0.125f);
                    Debug.Log("[LevelCompleteScreen] Confirm button (Continue) animated in");
                }
                
                if (doubleRewardButton != null)
                {
                    doubleRewardButton.transform.DOScale(_buttonScale, 0.125f);
                    Debug.Log("[LevelCompleteScreen] Double reward button animated in");
                }
                
                // Wait for user choice
                yield return new WaitUntil(() => _shouldClose);
            }
            else
            {
                // Fallback to original single-button behavior
                Debug.Log("[LevelCompleteScreen] Using original single confirm button");
                if (ConfirmButton != null)
                {
                    ConfirmButton.transform.DOScale(_buttonScale, 0.125f);
                }
            }
        }

        private void OnContinueButtonClicked()
        {
            Debug.Log("[LevelCompleteScreen] Continue button clicked - proceeding with normal reward");
            DoubleRewardClicked?.Invoke(false);
            _shouldClose = true;
            Close();
        }

        private void OnDoubleRewardButtonClicked()
        {
            Debug.Log("[LevelCompleteScreen] Double reward button clicked - showing rewarded ad");
            
            // Disable buttons during ad
            if (ConfirmButton != null) ConfirmButton.interactable = false;
            if (doubleRewardButton != null) doubleRewardButton.interactable = false;
            
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    Debug.Log("[LevelCompleteScreen] ✅ Rewarded ad completed - doubling reward!");
                    
                    // Update display to show doubled amount (visual only)
                    if (rewardAmountText != null)
                    {
                        rewardAmountText.text = (_totalReward * 2).ToString();
                    }
                    
                    DoubleRewardClicked?.Invoke(true);
                    _shouldClose = true;
                    Close();
                },
                onAdFailed: () => {
                    Debug.Log("[LevelCompleteScreen] ❌ Rewarded ad failed - restoring button state");
                    // Restore button state
                    if (ConfirmButton != null) ConfirmButton.interactable = true;
                    if (doubleRewardButton != null) doubleRewardButton.interactable = true;
                }
            );
        }
    }
}