using DG.Tweening;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private UltimateJoystick _ultimateJoystick;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Keyboard/Stick")]
        [SerializeField] private float keyPriorityDeadzone = 0.01f; // keyboard threshold
        [SerializeField] private float stickDeadzone = 0.1f;        // ignore tiny joystick drift

        public bool IsActive { get; set; } = true;

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
            if (!IsActive) return 0f;

            // Keyboard (QWERTY + AZERTY + arrows)
            int left  = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) ? 1 : 0;
            int right = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) ? 1 : 0;
            float keyX = right - left;

            // On‑screen joystick (only when visible/interactable)
            float joyX = _canvasGroup.interactable ? _ultimateJoystick.GetHorizontalAxis() : 0f;

            if (Mathf.Abs(keyX) > keyPriorityDeadzone) return Mathf.Clamp(keyX, -1f, 1f);
            return Mathf.Abs(joyX) > stickDeadzone ? joyX : 0f;
        }
        
        public float GetVerticalAxis()
        {
            if (!IsActive) return 0f;

            // Keyboard (QWERTY + AZERTY + arrows)
            int down = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) ? 1 : 0;
            int up   = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) ? 1 : 0;
            float keyY = up - down;

            float joyY = _canvasGroup.interactable ? _ultimateJoystick.GetVerticalAxis() : 0f;

            if (Mathf.Abs(keyY) > keyPriorityDeadzone) return Mathf.Clamp(keyY, -1f, 1f);
            return Mathf.Abs(joyY) > stickDeadzone ? joyY : 0f;
        }
    }
}
