using System;
using System.Collections;
using DG.Tweening;
using Mtl.Injection;
using Mtl.UiFramework;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Project.Game.Levels
{
    public class LevelFailedScreen : UIScreen
    {
        public event Action ConfirmButtonClicked;
        public event Action ReviveButtonClicked;
        
        [field: SerializeField] public Button ConfirmButton { get; private set; }
        [field: SerializeField] public Button ReviveButton { get; private set; }
        [field: SerializeField] public Animation Animation { get; private set; }
        [field: SerializeField] public CanvasGroup Backer { get; private set; }
        [field: SerializeField] public TextMeshProUGUI WaveIndexText { get; private set; }

        [Inject] private readonly LevelController _levelController;

        private Vector3 _buttonScale;
        private bool _reviveUsed = false;
        
        
        private void Awake()
        {
            ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
            ReviveButton.onClick.AddListener(OnReviveButtonClicked);
            
            _buttonScale = ConfirmButton.transform.localScale;
            ConfirmButton.transform.localScale = Vector3.zero;
            ReviveButton.transform.localScale = Vector3.zero;
        }
        
        protected override void OnOpened()
        {
            WaveIndexText.text = $"WAVE\n {_levelController.WaveIndex + 1} of {_levelController.WaveCount}";
            _reviveUsed = false; // Reset revive state
            
            // Reset button states for fresh level failed screen
            ConfirmButton.interactable = true;
            ReviveButton.interactable = true;
            ConfirmButton.gameObject.SetActive(true);
            ReviveButton.gameObject.SetActive(true);
            
            StartCoroutine(WaitForAnimation());
        }

        private IEnumerator WaitForAnimation()
        {
            yield return Backer.DOFade(1, 1f).WaitForCompletion();
            
            Animation.gameObject.SetActive(true);
            Animation.Play();
            
            while (Animation.IsPlaying(Animation.clip.name))
            {
                yield return null;
            }

            // Show both buttons with a slight delay between them
            ConfirmButton.transform.DOScale(_buttonScale, 0.125f);
            yield return new WaitForSeconds(0.1f);
            ReviveButton.transform.DOScale(_buttonScale, 0.125f);
        }

        private void OnConfirmButtonClicked()
        {
            ConfirmButton.gameObject.SetActive(false);
            ReviveButton.gameObject.SetActive(false);
            ConfirmButtonClicked?.Invoke();
        }

        private void OnReviveButtonClicked()
        {
            if (_reviveUsed)
            {
                Debug.Log("[LevelFailedScreen] Revive already used this level");
                return;
            }

            Debug.Log("[LevelFailedScreen] Revive button clicked - showing rewarded ad");
            
            // Disable buttons during ad
            ConfirmButton.interactable = false;
            ReviveButton.interactable = false;
            
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    Debug.Log("[LevelFailedScreen] Rewarded ad completed - giving revive!");
                    _reviveUsed = true;
                    ReviveButtonClicked?.Invoke();
                },
                onAdFailed: () => {
                    Debug.Log("[LevelFailedScreen] Rewarded ad failed - re-enabling buttons");
                    // Re-enable buttons if ad failed
                    ConfirmButton.interactable = true;
                    ReviveButton.interactable = true;
                }
            );
        }
    }
}