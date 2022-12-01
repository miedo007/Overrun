using DG.Tweening;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private UltimateJoystick _ultimateJoystick;
        [SerializeField] private CanvasGroup _canvasGroup;
        public bool IsActive { get; set; }

        public void Show()
        {
            IsActive = true;
            gameObject.SetActive(true);
            _canvasGroup.interactable = true;
            _ultimateJoystick.ResetJoystick();

            _canvasGroup.DOFade(1, 0.125f);
        }
        
        public void Hide(bool immediate = false)
        {
            _canvasGroup.interactable = false;
            
            if (!immediate)
            {
                _canvasGroup.DOFade(0, 0.125f)
                    .OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        IsActive = false;
                    });
            }
            else
            {
                _canvasGroup.alpha = 0;
                gameObject.SetActive(false);
                IsActive = false;
            }
        }
        
        public float GetHorizontalAxis()
        {
            return _canvasGroup.interactable ? _ultimateJoystick.GetHorizontalAxis() : 0;
        }
        
        public float GetVerticalAxis()
        {
            return _canvasGroup.interactable ? _ultimateJoystick.GetVerticalAxis() : 0;
        }
    }
}