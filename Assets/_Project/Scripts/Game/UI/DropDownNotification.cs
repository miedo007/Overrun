using DG.Tweening;
using MTLSimpleAudio;
using UnityEngine;

namespace Project.Game.UI
{
    public class DropDownNotification : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rect;
        [SerializeField] private AudioData audio;
        
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Display()
        {
            canvasGroup.DOKill();
            rect.DOKill();
            
            audio.Play();
            gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1,0.2f);
            
            canvasGroup.DOFade(0,0.2f)
                .SetDelay(3)
                .OnComplete(()=>gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            canvasGroup.DOKill();
            rect.DOKill();
            
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}