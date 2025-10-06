using UnityEngine;
using Mtl.Injection;
using Mtl.Save;
using Project.Application;

namespace Project.MainMenu.HeroSelection
{
    /// <summary>
    /// Tracks hero upgrade ads to prevent abuse - allows one free ad upgrade per game session
    /// Now uses persistent save data to prevent browser refresh exploits
    /// </summary>
    public static class HeroUpgradeAdsTracker
    {
        private static PlayerInfo _playerInfo;
        private static SaveManager _saveManager;
        
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
        /// Check if the player can use an ad upgrade (hasn't used one this session)
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
                return !_playerInfo.PlayerSave.HasUsedHeroAdUpgradeThisSession;
            }
        }
        
        /// <summary>
        /// Mark that the player has used their ad upgrade for this session
        /// </summary>
        public static void MarkAdUpgradeUsed()
        {
            if (_playerInfo?.PlayerSave == null)
            {
                Debug.LogError("[HeroUpgradeAdsTracker] Cannot mark ad used - PlayerInfo not initialized");
                return;
            }
            
            _playerInfo.ChangeHeroAdUpgradeState(true);
            _saveManager?.Save();
            Debug.Log("[HeroUpgradeAdsTracker] Ad upgrade used and saved - no more ad upgrades this session");
        }
        
        /// <summary>
        /// Reset ad upgrade availability (call when starting/completing a game)
        /// </summary>
        public static void ResetForNewSession()
        {
            if (_playerInfo?.PlayerSave == null)
            {
                Debug.LogError("[HeroUpgradeAdsTracker] Cannot reset session - PlayerInfo not initialized");
                return;
            }
            
            _playerInfo.ChangeHeroAdUpgradeState(false);
            _saveManager?.Save();
            Debug.Log("[HeroUpgradeAdsTracker] Ad upgrade availability reset and saved - player can use ad upgrade again");
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
            return $"Ad upgrade used this session: {_playerInfo.PlayerSave.HasUsedHeroAdUpgradeThisSession}";
        }
    }
}