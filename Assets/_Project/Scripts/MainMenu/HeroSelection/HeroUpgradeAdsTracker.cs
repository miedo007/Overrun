using UnityEngine;
using Mtl.Injection;
using Mtl.Save;
using Project.Application;

namespace Project.MainMenu.HeroSelection
{
    /// <summary>
    /// Tracks hero upgrade ads with timer-based cooldown system
    /// Uses persistent save data to prevent browser refresh exploits
    /// </summary>
    public static class HeroUpgradeAdsTracker
    {
        private static PlayerInfo _playerInfo;
        private static SaveManager _saveManager;
        
        /// <summary>
        /// Configurable cooldown duration in seconds (exposed for tweaking)
        /// </summary>
        public static float CooldownDuration { get; set; } = 300f; // Default: 5 minutes
        
        /// <summary>
        /// Initialize the tracker with required dependencies
        /// </summary>
        public static void Initialize(PlayerInfo playerInfo, SaveManager saveManager)
        {
            _playerInfo = playerInfo;
            _saveManager = saveManager;
            Debug.Log($"[HeroUpgradeAdsTracker] Initialized - Current state: {GetDebugInfo()}");
        }
        
        /// <summary>
        /// Check if the player can use an ad upgrade (cooldown has finished)
        /// </summary>
        public static bool CanUseAdUpgrade
        {
            get
            {
                if (_playerInfo?.PlayerSave == null)
                {
                    Debug.LogWarning("[HeroUpgradeAdsTracker] PlayerInfo not initialized - defaulting to false");
                    return false;
                }
                
                // Check if cooldown has finished
                return GetRemainingCooldownTime() <= 0f;
            }
        }
        
        /// <summary>
        /// Get the remaining cooldown time in seconds
        /// </summary>
        public static float GetRemainingCooldownTime()
        {
            if (_playerInfo?.PlayerSave == null)
            {
                return 0f;
            }
            
            var lastAdTime = _playerInfo.PlayerSave.LastHeroAdUpgradeTime;
            var currentTime = GetCurrentUnixTime();
            var timeSinceLastAd = currentTime - lastAdTime;
            var remainingTime = CooldownDuration - (float)timeSinceLastAd;
            
            return Mathf.Max(0f, remainingTime);
        }
        
        /// <summary>
        /// Get formatted countdown string for UI display
        /// </summary>
        public static string GetCooldownDisplayText()
        {
            var remainingTime = GetRemainingCooldownTime();
            
            if (remainingTime <= 0f)
            {
                return "UPGRADE WITH AD";
            }
            
            var minutes = Mathf.FloorToInt(remainingTime / 60f);
            var seconds = Mathf.FloorToInt(remainingTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
        
        /// <summary>
        /// Mark that the player has used their ad upgrade and start cooldown timer
        /// </summary>
        public static void MarkAdUpgradeUsed()
        {
            if (_playerInfo?.PlayerSave == null)
            {
                Debug.LogError("[HeroUpgradeAdsTracker] Cannot mark ad used - PlayerInfo not initialized");
                return;
            }
            
            var currentTime = GetCurrentUnixTime();
            _playerInfo.ChangeHeroAdUpgradeTime(currentTime);
            _saveManager?.Save();
            Debug.Log($"[HeroUpgradeAdsTracker] Ad upgrade used at {currentTime} - cooldown active for {CooldownDuration} seconds");
        }
        
        /// <summary>
        /// Reset ad upgrade availability (for testing or admin purposes)
        /// </summary>
        public static void ResetCooldown()
        {
            if (_playerInfo?.PlayerSave == null)
            {
                Debug.LogError("[HeroUpgradeAdsTracker] Cannot reset cooldown - PlayerInfo not initialized");
                return;
            }
            
            _playerInfo.ChangeHeroAdUpgradeTime(0);
            _saveManager?.Save();
            Debug.Log("[HeroUpgradeAdsTracker] Ad upgrade cooldown reset - player can use ad upgrade again");
        }
        
        /// <summary>
        /// Get current Unix timestamp in seconds
        /// </summary>
        private static double GetCurrentUnixTime()
        {
            return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
        }
        
        /// <summary>
        /// Get debug info about current state
        /// </summary>
        public static string GetDebugInfo()
        {
            if (_playerInfo?.PlayerSave == null)
            {
                return "PlayerInfo not initialized";
            }
            
            var lastAdTime = _playerInfo.PlayerSave.LastHeroAdUpgradeTime;
            var remainingTime = GetRemainingCooldownTime();
            var canUse = CanUseAdUpgrade;
            
            return $"Last ad time: {lastAdTime}, Remaining cooldown: {remainingTime:F1}s, Can use: {canUse}";
        }
    }
}