using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Project.Game.Tutorials
{
    public class PlatformHintManager : MonoBehaviour
    {
        [Header("Platform-Specific Hints")]
        [SerializeField] private GameObject desktopHint;
        [SerializeField] private GameObject mobileHint;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        private IdleHintBase activeHint;
        private bool isMobile;
        private bool detectionComplete = false;

        // Import the JavaScript function
        [DllImport("__Internal")]
        private static extern void DetectMobile();

        private void Awake()
        {
            // Start mobile detection
            StartMobileDetection();
        }

        private void StartMobileDetection()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Only detect mobile on WebGL builds, not in Editor
            string url = UnityEngine.Application.absoluteURL.ToLower();
            if (url.Contains("crazygames.com"))
            {
                if (debugMode)
                {
                    Debug.Log("[PlatformHintManager] Starting mobile detection via JavaScript...");
                }
                
                // Call JavaScript function - result will come back via OnMobileDetected
                DetectMobile();
                return;
            }
#endif
            // Fallback: Editor or non-CrazyGames = Desktop
            if (debugMode)
            {
                Debug.Log("[PlatformHintManager] Using fallback detection (Editor/Non-CrazyGames): Desktop");
            }
            SetPlatform(false); // false = desktop
        }

        // This method is called by JavaScript via SendMessage
        public void OnMobileDetected(string result)
        {
            bool isMobileDevice = result == "true";
            
            if (debugMode)
            {
                Debug.Log($"[PlatformHintManager] JavaScript detection result: {(isMobileDevice ? "Mobile" : "Desktop")}");
            }
            
            SetPlatform(isMobileDevice);
        }

        private void SetPlatform(bool mobile)
        {
            isMobile = mobile;
            detectionComplete = true;

            if (debugMode)
            {
                Debug.Log($"[PlatformHintManager] Platform set to: {(isMobile ? "Mobile" : "Desktop")}");
                Debug.Log($"[PlatformHintManager] Screen: {Screen.width}x{Screen.height}");
                Debug.Log($"[PlatformHintManager] URL: {UnityEngine.Application.absoluteURL}");
            }

            // Enable appropriate hint system
            if (desktopHint != null) desktopHint.SetActive(!isMobile);
            if (mobileHint != null) mobileHint.SetActive(isMobile);

            // Get the active hint component
            if (isMobile && mobileHint != null)
                activeHint = mobileHint.GetComponent<IdleHintBase>();
            else if (!isMobile && desktopHint != null)
                activeHint = desktopHint.GetComponent<IdleHintBase>();

            if (activeHint == null)
            {
                Debug.LogError("[PlatformHintManager] No active hint component found!");
            }
        }

        public void Begin(int waveIndex = 0)
        {
            // Wait for detection to complete before starting hints
            if (!detectionComplete)
            {
                StartCoroutine(WaitForDetectionAndBegin(waveIndex));
                return;
            }

            if (debugMode)
            {
                Debug.Log($"[PlatformHintManager] Begin wave {waveIndex} on {(isMobile ? "Mobile" : "Desktop")}");
            }

            activeHint?.Begin(waveIndex);
        }

        private IEnumerator WaitForDetectionAndBegin(int waveIndex)
        {
            // Wait up to 1 second for detection to complete
            float timeout = 1f;
            float elapsed = 0f;
            
            while (!detectionComplete && elapsed < timeout)
            {
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            if (!detectionComplete)
            {
                if (debugMode)
                {
                    Debug.LogWarning("[PlatformHintManager] Detection timeout - defaulting to Desktop");
                }
                SetPlatform(false); // Default to desktop if detection fails
            }

            activeHint?.Begin(waveIndex);
        }

        public void End()
        {
            if (debugMode)
            {
                Debug.Log("[PlatformHintManager] End hints");
            }

            activeHint?.End();
        }
    }
}