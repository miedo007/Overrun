using UnityEngine;
using System.Runtime.InteropServices;

namespace Project.Game.Tutorials
{
    /// <summary>
    /// LEGACY - Use PlatformHintManager instead
    /// Shows a movement hint when the player is idle.
    /// - Always visible on the first wave (wave 0).
    /// - On subsequent waves, only appears after a cooldown period.
    /// - Hides as soon as the player moves.
    /// - Reappears after the player goes idle again for 'idleDelay' seconds.
    /// Attach to the hint UI object (must have a CanvasGroup).
    /// </summary>
    [System.Obsolete("Use PlatformHintManager with DesktopIdleHint/MobileIdleHint instead")]
    public class IdleHint : MonoBehaviour
    {
        [Header("Who to watch (movement)")]
        [SerializeField] private Transform target;                 // Assign the scene instance: player_controller

        [Header("Behaviour")]
        [SerializeField] private float moveThreshold = 0.02f;      // world units/sec to consider "moving"
        [SerializeField] private float idleDelay = 2f;             // seconds idle before showing again
        [SerializeField] private float fadeDuration = 0.15f;       // 0 = instant

        [Header("Wave-based Display")]
        [SerializeField] private float cooldownAfterFirstWave = 10f; // cooldown time for waves after the first

        private CanvasGroup cg;
        private Vector3 lastPos;
        private float idleTimer;
        private float cooldownTimer;
        private bool isVisible;
        private bool tracking;
        private int currentWaveIndex;
        private bool cooldownActive;

        /// <summary>Called by GameController at wave start.</summary>
        public void Begin(int waveIndex = 0)
        {
            if (tracking) return;
            
            currentWaveIndex = waveIndex;
            tracking = true;
            idleTimer = 0f;
            
            // First wave: show immediately
            if (currentWaveIndex == 0)
            {
                cooldownActive = false;
                ShowImmediate();
            }
            // Subsequent waves: start cooldown
            else
            {
                cooldownActive = true;
                cooldownTimer = cooldownAfterFirstWave;
                HideImmediate();
            }
            
            if (target) lastPos = target.position;
        }

        /// <summary>Optional: stop tracking and hide (e.g., between waves / in shop).</summary>
        public void End()
        {
            tracking = false;
            cooldownActive = false;
            cooldownTimer = 0f;
            HideImmediate();
        }

        private void Awake()
        {
            // Hide on CrazyGames mobile web
            if (IsCrazyGamesMobile())
            {
                gameObject.SetActive(false);
                return;
            }
            
            cg = GetComponent<CanvasGroup>();
            if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
            HideImmediate(); // we only show when Begin() is called
        }

        private bool IsCrazyGamesMobile()
        {
            // Check if running on WebGL platform
            if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                return false;
            
            // Check if the URL contains crazygames domain
            string currentUrl = UnityEngine.Application.absoluteURL.ToLower();
            if (!currentUrl.Contains("crazygames.com"))
                return false;
            
            // Multiple detection methods
            bool isMobileResolution = Screen.width <= 768 || Screen.height <= 768;
            bool isLandscapeMobile = (Screen.width <= 1024 && Screen.height <= 768) || (Screen.width <= 768 && Screen.height <= 1024);
            bool hasTouch = Input.touchSupported;
            
            // Return true if any mobile indicator is detected
            return isMobileResolution || isLandscapeMobile || hasTouch;
        }

        private void Update()
        {
            if (!tracking || !target) return;

            // Handle cooldown for non-first waves
            if (cooldownActive)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f)
                {
                    cooldownActive = false;
                }
                // Don't process movement during cooldown
                return;
            }

            Vector3 pos = target.position;
            float speed = (pos - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            lastPos = pos;

            if (speed > moveThreshold)
            {
                idleTimer = 0f;
                if (isVisible) Hide();
            }
            else
            {
                idleTimer += Time.deltaTime;
                if (!isVisible && idleTimer >= idleDelay)
                    Show();
            }
        }

        private void Show()
        {
            isVisible = true;
            StopAllCoroutines();
            StartCoroutine(FadeTo(1f));
        }

        private void Hide()
        {
            isVisible = false;
            StopAllCoroutines();
            StartCoroutine(FadeTo(0f));
        }

        private void ShowImmediate()
        {
            isVisible = true;
            StopAllCoroutines();
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        private void HideImmediate()
        {
            isVisible = false;
            StopAllCoroutines();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        private System.Collections.IEnumerator FadeTo(float targetAlpha)
        {
            cg.blocksRaycasts = targetAlpha >= 1f;
            cg.interactable  = targetAlpha >= 1f;

            if (fadeDuration <= 0f) { cg.alpha = targetAlpha; yield break; }

            float start = cg.alpha, t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
                yield return null;
            }
            cg.alpha = targetAlpha;
        }
    }
}
