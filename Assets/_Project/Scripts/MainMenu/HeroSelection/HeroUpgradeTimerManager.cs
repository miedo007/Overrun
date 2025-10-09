using UnityEngine;

namespace Project.MainMenu.HeroSelection
{
    /// <summary>
    /// MonoBehaviour component to handle hero upgrade timer updates
    /// Attach this to a GameObject in the hero selection scene
    /// </summary>
    public class HeroUpgradeTimerManager : MonoBehaviour
    {
        [Header("Timer Settings")]
        [SerializeField, Tooltip("Cooldown duration in seconds (5 minutes = 300)")]
        private float cooldownDuration = 300f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        private void Awake()
        {
            // Set the cooldown duration EARLY in initialization to prevent other systems from using default
            HeroUpgradeAdsTracker.CooldownDuration = cooldownDuration;
            
            if (enableDebugLogs)
            {
                Debug.Log($"[HeroUpgradeTimerManager] Timer manager initialized with cooldown duration: {cooldownDuration} seconds");
            }
        }
        
        private void Start()
        {
            // Double-check that our duration is properly set
            if (enableDebugLogs)
            {
                Debug.Log($"[HeroUpgradeTimerManager] Current cooldown duration: {HeroUpgradeAdsTracker.CooldownDuration} seconds");
            }
        }
        
        private void Update()
        {
            // The timer logic is handled in HeroUpgradeAdsTracker
            // This component just exists to expose the cooldown duration setting
        }
        
        /// <summary>
        /// Reset the cooldown for testing purposes
        /// </summary>
        [ContextMenu("Reset Cooldown")]
        public void ResetCooldown()
        {
            HeroUpgradeAdsTracker.ResetCooldown();
            Debug.Log("[HeroUpgradeTimerManager] Cooldown reset via context menu");
        }
        
        /// <summary>
        /// Force trigger cooldown for testing purposes
        /// </summary>
        [ContextMenu("Trigger Cooldown")]
        public void TriggerCooldown()
        {
            HeroUpgradeAdsTracker.MarkAdUpgradeUsed();
            Debug.Log("[HeroUpgradeTimerManager] Cooldown triggered via context menu");
        }
        
        /// <summary>
        /// Get debug info
        /// </summary>
        [ContextMenu("Debug Info")]
        public void ShowDebugInfo()
        {
            Debug.Log($"[HeroUpgradeTimerManager] {HeroUpgradeAdsTracker.GetDebugInfo()}");
        }
    }
}