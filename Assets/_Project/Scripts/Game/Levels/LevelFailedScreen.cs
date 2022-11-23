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
        
        [field: SerializeField] public Button ConfirmButton { get; private set; }
        [field: SerializeField] public Animation Animation { get; private set; }
        [field: SerializeField] public CanvasGroup Backer { get; private set; }
        [field: SerializeField] public TextMeshProUGUI WaveIndexText { get; private set; }

        [Inject] private readonly LevelController _levelController;

        private Vector3 _buttonScale;
        
        
        private void Awake()
        {
            ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
            _buttonScale = ConfirmButton.transform.localScale;
            ConfirmButton.transform.localScale = Vector3.zero;
        }
        
        protected override void OnOpened()
        {
            WaveIndexText.text = $"WAVE {_levelController.WaveIndex + 1}";
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

            ConfirmButton.transform.DOScale(_buttonScale, 0.125f);
        }

        private void OnConfirmButtonClicked()
        {
            ConfirmButton.gameObject.SetActive(false);
            ConfirmButtonClicked?.Invoke();
        }
    }
}