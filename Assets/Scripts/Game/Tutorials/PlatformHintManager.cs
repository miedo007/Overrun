using UnityEngine;

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

        private void Awake()
        {
            // Detect platform
            isMobile = IsMobilePlatform();

            if (debugMode)
            {
                Debug.Log($"[PlatformHintManager] Platform detected: {(isMobile ? "Mobile" : "Desktop")}");
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
            if (debugMode)
            {
                Debug.Log($"[PlatformHintManager] Begin wave {waveIndex} on {(isMobile ? "Mobile" : "Desktop")}");
            }

            activeHint?.Begin(waveIndex);
        }

        public void End()
        {
            if (debugMode)
            {
                Debug.Log($"[PlatformHintManager] End hints");
            }

            activeHint?.End();
        }

        private bool IsMobilePlatform()
        {
            // Only apply mobile detection on WebGL builds
            if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                return false;

            // Only apply to CrazyGames domain
            string url = UnityEngine.Application.absoluteURL.ToLower();
            if (!url.Contains("crazygames.com"))
                return false;

            // Much more restrictive mobile detection - only very small screens
            bool isVerySmallScreen = Screen.width <= 480 || Screen.height <= 480;
            bool isSmallLandscape = Screen.width <= 854 && Screen.height <= 480;
            
            return isVerySmallScreen || isSmallLandscape;
        }
    }
}