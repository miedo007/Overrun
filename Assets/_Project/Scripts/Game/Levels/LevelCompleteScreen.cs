using System.Collections;
using DG.Tweening;
using Mtl.UiFramework;
using UnityEngine.UI;
using UnityEngine;

namespace Project.Game.Levels
{
    public class LevelCompleteScreen : UIScreen
    {
        
        [field: SerializeField] public Button ConfirmButton { get; private set; }
        [field: SerializeField] public Animation Animation { get; private set; }

        private Vector3 _buttonScale;
        
        private void Awake()
        {
            ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
            _buttonScale = ConfirmButton.transform.localScale;
            ConfirmButton.transform.localScale = Vector3.zero;
        }
        
        protected override void OnOpened()
        {
            StartCoroutine(WaitForAnimation());
        }

        private IEnumerator WaitForAnimation()
        {
            Animation.Play();
            
            while (Animation.IsPlaying(Animation.clip.name))
            {
                yield return null;
            }

            ConfirmButton.transform.DOScale(_buttonScale, 0.125f);
        }

        private void OnConfirmButtonClicked()
        {
            if (IsOpened)
            {
                Close();
            }
        }
    }
}