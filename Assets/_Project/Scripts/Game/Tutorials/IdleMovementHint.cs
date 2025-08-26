using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Tutorials
{
    public class IdleMovementHint : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private CanvasGroup root;          // whole hint (for fade), optional
        [SerializeField] private RectTransform panel;       // graphic group to position
        [SerializeField] private Image dimmer;              // optional overlay, keep disabled or semi-transparent

        [Header("Follow")]
        [SerializeField] private Transform worldTarget;     // player transform
        [SerializeField] private Vector3 worldOffset = new Vector3(0, 1.2f, 0);
        [SerializeField] private Vector2 screenOffset = new Vector2(0f, 60f);
        [SerializeField] private Camera worldCamera;        // auto = Camera.main if null

        [Header("Behavior")]
        [SerializeField] private float idleDelay = 2.0f;          // time with NO input before showing
        [SerializeField] private float recheckEvery = 0.1f;       // polling cadence
        [SerializeField] private float inputThreshold = 0.15f;    // axis threshold
        [SerializeField] private float reactivateDelay = 1.0f;    // cooldown after hiding before it can show again
        [SerializeField] private float fadeTime = 0.15f;          // fade in/out

        private bool _visible;
        private Coroutine _loop;
        private float _idleTimer;
        private float _reactivateTimer;

        public void Initialize(Transform player, Camera cam = null)
        {
            worldTarget = player;
            worldCamera = cam != null ? cam : Camera.main;
        }

        private void Awake()
        {
            if (root != null)
            {
                root.alpha = 0f;
                root.interactable = false;
                root.blocksRaycasts = false;
            }
            if (dimmer != null) dimmer.enabled = false; // hint should not block gameplay
            gameObject.SetActive(false); // start hidden
        }

        public void Arm()
        {
            if (_loop != null) StopCoroutine(_loop);
            _loop = StartCoroutine(IdleLoop());
        }

        public void Disarm()
        {
            if (_loop != null) StopCoroutine(_loop);
            _loop = null;
            HideImmediate();
        }

        private IEnumerator IdleLoop()
        {
            _idleTimer = 0f;
            _reactivateTimer = 0f;

            gameObject.SetActive(true);

            while (true)
            {
                // cooldown before we allow showing again
                if (_reactivateTimer > 0f)
                    _reactivateTimer -= Time.unscaledDeltaTime;

                // movement?
                if (HasMoveInput())
                {
                    _idleTimer = 0f;
                    if (_visible) Hide();
                }
                else
                {
                    _idleTimer += Time.unscaledDeltaTime;
                    if (!_visible && _reactivateTimer <= 0f && _idleTimer >= idleDelay)
                        Show();
                }

                if (_visible) Follow();

                yield return new WaitForSecondsRealtime(recheckEvery);
            }
        }

        private void Show()
        {
            _visible = true;
            if (root != null) StartCoroutine(Fade(root, root.alpha, 1f, fadeTime));
            else gameObject.SetActive(true);

            Follow(); // snap into position immediately
        }

        private void Hide()
        {
            _visible = false;
            if (root != null) StartCoroutine(Fade(root, root.alpha, 0f, fadeTime));
            _reactivateTimer = reactivateDelay;
        }

        private void HideImmediate()
        {
            _visible = false;
            if (root != null) root.alpha = 0f;
            gameObject.SetActive(false);
        }

        private IEnumerator Fade(CanvasGroup cg, float a, float b, float t)
        {
            float elapsed = 0f;
            while (elapsed < t)
            {
                elapsed += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(a, b, elapsed / t);
                yield return null;
            }
            cg.alpha = b;
            // keep the object active even when alpha 0 so it can re-show
        }

        private void Follow()
        {
            if (panel == null || worldTarget == null) return;
            var cam = worldCamera != null ? worldCamera : Camera.main;
            if (cam == null) return;

            Vector3 screen = RectTransformUtility.WorldToScreenPoint(cam, worldTarget.position + worldOffset);
            panel.position = screen + (Vector3)screenOffset;
        }

        private bool HasMoveInput()
        {
            // Keys
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) return true;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) return true;

            // Axes (pads/joysticks/keyboard)
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > inputThreshold) return true;
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > inputThreshold) return true;

            return false;
        }
    }
}
