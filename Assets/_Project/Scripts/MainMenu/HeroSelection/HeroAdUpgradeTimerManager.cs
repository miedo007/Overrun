using UnityEngine;

namespace Project.MainMenu.HeroSelection
{
    /// <summary>
    /// Manager component for configuring hero ad upgrade cooldown duration
    /// Runs early to ensure duration is set before HeroView initializes
    /// </summary>
    [DefaultExecutionOrder(-1000)] // Ensure this runs before HeroView
    public class HeroAdUpgradeTimerManager : MonoBehaviour
    {
        [Header("Timer Settings")]
        [SerializeField] private float cooldownDuration = 300f; // 5 minutes default
        [Space]
        [SerializeField] private bool usePreset = false;
        [SerializeField] private CooldownPreset preset = CooldownPreset.FiveMinutes;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        
        public enum CooldownPreset
        {
            ThirtySeconds = 30,
            OneMinute = 60,
            TwoMinutes = 120,
            FiveMinutes = 300,
            TenMinutes = 600
        }
        
        private void Awake()
        {
            // Use preset if enabled, otherwise use custom duration
            var finalDuration = usePreset ? (float)preset : cooldownDuration;
            
            // Set the cooldown duration in the timer system BEFORE anything else initializes
            HeroAdUpgradeTimer.CooldownDuration = finalDuration;
            
            if (enableDebugLogs) 
                Debug.Log($"[HeroAdUpgradeTimerManager] EARLY INITIALIZATION - Set cooldown duration: {finalDuration} seconds ({finalDuration/60f:F1} minutes)");
        }
        
        private void Start()
        {
            // Verify the duration was set correctly and didn't get overridden
            var currentDuration = HeroAdUpgradeTimer.CooldownDuration;
            var expectedDuration = usePreset ? (float)preset : cooldownDuration;
            
            if (enableDebugLogs) 
                Debug.Log($"[HeroAdUpgradeTimerManager] VERIFICATION - Current duration: {currentDuration}s, Expected: {expectedDuration}s");
            
            // If duration was somehow changed, force it back
            if (Mathf.Abs(currentDuration - expectedDuration) > 0.1f)
            {
                HeroAdUpgradeTimer.CooldownDuration = expectedDuration;
                Debug.LogWarning($"[HeroAdUpgradeTimerManager] Duration was overridden! Restored from {currentDuration}s to {expectedDuration}s");
            }
        }
        
        /// <summary>
        /// Reset the cooldown for testing purposes
        /// </summary>
        [ContextMenu("Reset Cooldown")]
        public void ResetCooldown()
        {
            // You would implement this based on your PlayerInfo system
            Debug.Log("[HeroAdUpgradeTimerManager] Reset cooldown - implement this based on your needs");
        }
        
        /// <summary>
        /// Get the current cooldown duration in a readable format
        /// </summary>
        public string GetCurrentDurationText()
        {
            var duration = HeroAdUpgradeTimer.CooldownDuration;
            var minutes = Mathf.FloorToInt(duration / 60f);
            var seconds = Mathf.FloorToInt(duration % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
        
        /// <summary>
        /// Force set the duration (for runtime changes)
        /// </summary>
        [ContextMenu("Apply Duration Now")]
        public void ApplyDurationNow()
        {
            var finalDuration = usePreset ? (float)preset : cooldownDuration;
            HeroAdUpgradeTimer.CooldownDuration = finalDuration;
            Debug.Log($"[HeroAdUpgradeTimerManager] Forced duration to: {finalDuration} seconds ({finalDuration/60f:F1} minutes)");
        }
    }
}