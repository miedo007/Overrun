using UnityEngine;

namespace Project.MainMenu.HeroSelection
{
    /// <summary>
    /// Validation script to test hero upgrade ads abuse prevention
    /// Add this to a scene to monitor the system behavior
    /// </summary>
    public class HeroUpgradeAdsValidator : MonoBehaviour
    {
        [Header("Validation Settings")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private float logInterval = 5f; // Log status every X seconds
        
        private float _lastLogTime;
        
        private void Update()
        {
            if (!enableLogging) return;
            
            if (Time.time - _lastLogTime >= logInterval)
            {
                LogCurrentStatus();
                _lastLogTime = Time.time;
            }
        }
        
        private void LogCurrentStatus()
        {
            if (!enableLogging) return;
            
            Debug.Log($"[HeroUpgradeAdsValidator] {HeroUpgradeAdsTracker.GetDebugInfo()}");
            
            // Check if HeroView exists and log its state
            var heroView = FindObjectOfType<HeroView>();
            if (heroView != null)
            {
                Debug.Log("[HeroUpgradeAdsValidator] HeroView found - UI should reflect current tracker state");
            }
            else
            {
                Debug.Log("[HeroUpgradeAdsValidator] No HeroView found in scene");
            }
        }
        
        [ContextMenu("Test: Mark Ad Used")]
        private void TestMarkAdUsed()
        {
            HeroUpgradeAdsTracker.MarkAdUpgradeUsed();
            Debug.Log("[HeroUpgradeAdsValidator] Manually marked ad as used");
        }
        
        [ContextMenu("Test: Reset For New Session")]
        private void TestResetForNewSession()
        {
            HeroUpgradeAdsTracker.ResetForNewSession();
            Debug.Log("[HeroUpgradeAdsValidator] Manually reset for new session");
        }
        
        [ContextMenu("Test: Show Current Status")]
        private void TestShowCurrentStatus()
        {
            LogCurrentStatus();
        }
    }
}