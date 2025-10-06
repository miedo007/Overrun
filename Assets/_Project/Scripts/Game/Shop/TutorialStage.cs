using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class TutorialStage : MonoBehaviour
    {

        public event Action Closed;
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(Close);
        }

        private void Close()
        {
            button.interactable = false;
            panelRoot.DOScale(0, 0.125f)
                .OnComplete(() =>
                {
                    Closed?.Invoke();
                    gameObject.SetActive(false);
                });
        }

        public void Open()
        {
            button.interactable = false;
            
            gameObject.SetActive(true);
            var targetScale = panelRoot.localScale;
            panelRoot.localScale = Vector3.zero;
            panelRoot.DOScale(targetScale, 0.125f);
            StartCoroutine(EnableButtonRoutine());
        }

        private IEnumerator EnableButtonRoutine()
        {
            yield return new WaitForSeconds(0.0f);
            button.interactable = true;
        }

        public IEnumerator TutorialRoutine()
        {
            throw new NotImplementedException();
        }
    }
}