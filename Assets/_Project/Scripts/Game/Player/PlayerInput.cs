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

        [Header("Auto Hide Settings")]
        [SerializeField] private bool autoHideJoystick = true;      // Enable auto hide joystick
        [SerializeField] private float hideDelay = 2f;              // Delay before hiding after keyboard use
        [SerializeField] private float showDelay = 0.5f;            // Delay before showing after touch

        private bool _isJoystickVisible = true;
        private float _lastKeyboardInputTime;
        private float _lastTouchInputTime;
        private InputMethod _lastInputMethod = InputMethod.None;

        public bool IsActive { get; set; } = true;

        private enum InputMethod
        {
            None,
            Keyboard,
            Touch
        }

        private void Update()
        {
            if (!autoHideJoystick) return;

            HandleInputMethodDetection();
            HandleJoystickVisibility();
        }

        private void HandleInputMethodDetection()
        {
            // Check for keyboard input
            bool hasKeyboardInput = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) ||
                                   Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ||
                                   Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ||
                                   Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow);

            // Check for touch/joystick input
            bool hasTouchInput = Input.touchCount > 0 || 
                                (Mathf.Abs(_ultimateJoystick.GetHorizontalAxis()) > stickDeadzone || 
                                 Mathf.Abs(_ultimateJoystick.GetVerticalAxis()) > stickDeadzone);

            if (hasKeyboardInput)
            {
                _lastKeyboardInputTime = Time.time;
                if (_lastInputMethod != InputMethod.Keyboard)
                {
                    _lastInputMethod = InputMethod.Keyboard;
                }
            }

            if (hasTouchInput)
            {
                _lastTouchInputTime = Time.time;
                if (_lastInputMethod != InputMethod.Touch)
                {
                    _lastInputMethod = InputMethod.Touch;
                }
            }
        }

        private void HandleJoystickVisibility()
        {
            bool shouldShowJoystick = ShouldShowJoystick();

            if (shouldShowJoystick && !_isJoystickVisible)
            {
                ShowJoystick();
            }
            else if (!shouldShowJoystick && _isJoystickVisible)
            {
                HideJoystick();
            }
        }

        private bool ShouldShowJoystick()
        {
            float timeSinceKeyboard = Time.time - _lastKeyboardInputTime;
            float timeSinceTouch = Time.time - _lastTouchInputTime;

            // Show joystick if:
            // 1. Touch input was used recently
            // 2. Or if enough time has passed since keyboard input and we had touch input before
            // 3. Or if no input method has been detected yet
            if (_lastInputMethod == InputMethod.Touch && timeSinceTouch < showDelay)
            {
                return true;
            }

            if (_lastInputMethod == InputMethod.Keyboard && timeSinceKeyboard > hideDelay && timeSinceTouch < timeSinceKeyboard)
            {
                return true;
            }

            if (_lastInputMethod == InputMethod.None)
            {
                return true;
            }

            return false;
        }

        private void ShowJoystick()
        {
            _isJoystickVisible = true;
            _canvasGroup.interactable = true;
            _canvasGroup.DOFade(1, 0.125f);
        }

        private void HideJoystick()
        {
            _isJoystickVisible = false;
            _canvasGroup.interactable = false;
            _canvasGroup.DOFade(0f, 0.125f); // Completely hide the joystick
        }

        public void Show()
        {
            IsActive = true;
            gameObject.SetActive(true);
            _canvasGroup.interactable = true;
            _ultimateJoystick.ResetJoystick();
            _canvasGroup.DOFade(1, 0.125f);
            _isJoystickVisible = true;
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
            _isJoystickVisible = false;
        }

        public float GetHorizontalAxis()
        {
            if (!IsActive) return 0f;

            // Keyboard (QWERTY + AZERTY + arrows)
            int left  = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) ? 1 : 0;
            int right = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) ? 1 : 0;
            float keyX = right - left;

            // On‑screen joystick (only when visible/interactable)
            float joyX = (_canvasGroup.interactable && _isJoystickVisible) ? _ultimateJoystick.GetHorizontalAxis() : 0f;

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

            float joyY = (_canvasGroup.interactable && _isJoystickVisible) ? _ultimateJoystick.GetVerticalAxis() : 0f;

            if (Mathf.Abs(keyY) > keyPriorityDeadzone) return Mathf.Clamp(keyY, -1f, 1f);
            return Mathf.Abs(joyY) > stickDeadzone ? joyY : 0f;
        }
    }
}
