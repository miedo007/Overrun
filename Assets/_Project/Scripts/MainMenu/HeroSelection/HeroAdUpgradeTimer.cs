using UnityEngine;
using Mtl.Save;
using Project.Application;
using System;

namespace Project.MainMenu.HeroSelection
{
    /// <summary>
    /// Simple timer system for hero ad upgrades with persistent cooldown
    /// </summary>
    public static class HeroAdUpgradeTimer
    {
        private static PlayerInfo _playerInfo;
        private static SaveManager _saveManager;
        private static float _cooldownDuration = 300f;
        
        /// <summary>
        /// Event fired when cooldown duration changes
        /// </summary>
        public static event Action OnDurationChanged;
        
        /// <summary>
        /// Cooldown duration in seconds (5 minutes)
        /// </summary>
        public static float CooldownDuration 
        { 
            get => _cooldownDuration;
            set
            {
                if (Math.Abs(_cooldownDuration - value) > 0.1f)
                {
                    Debug.Log($"[HeroAdUpgradeTimer] Duration changed from {_cooldownDuration}s to {value}s");
                    _cooldownDuration = value;
                    OnDurationChanged?.Invoke();
                }
            }
        }
        
        /// <summary>
        /// Initialize the timer with required dependencies
        /// </summary>
        public static void Initialize(PlayerInfo playerInfo, SaveManager saveManager)
        {
            _playerInfo = playerInfo;
            _saveManager = saveManager;
            Debug.Log($"[HeroAdUpgradeTimer] Timer initialized with duration: {CooldownDuration} seconds ({CooldownDuration/60f:F1} minutes)");
        }
        
        /// <summary>
        /// Check if the player can use an ad upgrade (cooldown has finished)
        /// </summary>
        public static bool CanUseAdUpgrade
        {
            get
            {
                if (_playerInfo?.PlayerSave == null)
                    return false;
                
                return GetRemainingCooldownTime() <= 0f;
            }
        }
        
        /// <summary>
        /// Get the remaining cooldown time in seconds
        /// </summary>
        public static float GetRemainingCooldownTime()
        {
            if (_playerInfo?.PlayerSave == null)
                return 0f;
            
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
                return "";
            
            var minutes = Mathf.FloorToInt(remainingTime / 60f);
            var seconds = Mathf.FloorToInt(remainingTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
        
        /// <summary>
        /// Start the cooldown timer after using an ad upgrade
        /// </summary>
        public static void StartCooldown()
        {
            if (_playerInfo?.PlayerSave == null)
            {
                Debug.LogError("[HeroAdUpgradeTimer] Cannot start cooldown - PlayerInfo not initialized");
                return;
            }
            
            var currentTime = GetCurrentUnixTime();
            _playerInfo.ChangeHeroAdUpgradeTime(currentTime);
            _saveManager?.Save();
            Debug.Log($"[HeroAdUpgradeTimer] Cooldown started - {CooldownDuration} seconds ({CooldownDuration/60f:F1} minutes) remaining");
        }
        
        /// <summary>
        /// Get current Unix timestamp in seconds
        /// </summary>
        private static double GetCurrentUnixTime()
        {
            return (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}