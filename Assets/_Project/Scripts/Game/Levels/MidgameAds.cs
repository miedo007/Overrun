using System;
using UnityEngine;

namespace Project.Game.Levels
{
    /// <summary>
    /// Manages midgame ads for CrazyGames with cooldown and configurable settings
    /// </summary>
    public static class MidgameAds
    {
        // Tweakable settings
        public static int WaveFrequency = 3; // Show ad every X waves  
        public static float CooldownMinutes = 2f; // Minimum time between ads in minutes
        
        // Private tracking
        private static int _adsShown = 0;
        private static float _lastAdTime = -999f; // Time when last ad was shown
        
        /// <summary>
        /// Check if we should show a midgame ad based on wave and cooldown
        /// </summary>
        public static bool ShouldShowAd(int currentWave, bool isLevelComplete = false)
        {
            float timeSinceLastAd = Time.time - _lastAdTime;
            bool cooldownPassed = timeSinceLastAd >= (CooldownMinutes * 60f);
            
            Debug.Log($"[MidgameAds] ShouldShowAd check:");
            Debug.Log($"  - currentWave: {currentWave}");
            Debug.Log($"  - isLevelComplete: {isLevelComplete}");
            Debug.Log($"  - timeSinceLastAd: {timeSinceLastAd:F1}s");
            Debug.Log($"  - cooldownPassed: {cooldownPassed}");
            Debug.Log($"  - _adsShown: {_adsShown}");
            
            if (!cooldownPassed)
            {
                Debug.Log($"[MidgameAds] Cooldown active, need {(CooldownMinutes * 60f) - timeSinceLastAd:F1}s more");
                return false;
            }
            
            bool shouldShow = false;
            
            if (isLevelComplete)
            {
                // Always show on level complete (if cooldown passed)
                shouldShow = true;
                Debug.Log("[MidgameAds] Level complete - showing ad");
            }
            else
            {
                // Show every WaveFrequency waves
                shouldShow = currentWave > 0 && (currentWave % WaveFrequency == 0);
                Debug.Log($"[MidgameAds] Wave check: Wave {currentWave} % {WaveFrequency} = {currentWave % WaveFrequency} -> {shouldShow}");
                Debug.Log($"[MidgameAds] Expected trigger waves: {WaveFrequency}, {WaveFrequency * 2}, {WaveFrequency * 3}, etc.");
            }
            
            return shouldShow;
        }
        
        /// <summary>
        /// Show a midgame ad using CrazyGames SDK
        /// </summary>
        public static void ShowAd(Action onAdFinished = null, Action onAdFailed = null)
        {
            Debug.Log($"[MidgameAds] ShowAd called - Platform: {UnityEngine.Application.platform}");
            Debug.Log($"[MidgameAds] Is WebGL: {UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer}");
            
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("[MidgameAds] Calling CrazySDK.Ad.RequestAd for midgame ad");
            
            try
            {
                CrazyGames.CrazySDK.Ad.RequestAd(
                    CrazyGames.CrazyAdType.Midgame,
                    () => {
                        // Ad started
                        Debug.Log("[MidgameAds] ✅ Ad started successfully");
                        _lastAdTime = Time.time; // Record ad time when it starts
                    },
                    (error) => {
                        // Ad error
                        Debug.Log($"[MidgameAds] ❌ Ad failed - Code: {error.code}, Message: {error.message}");
                        onAdFailed?.Invoke();
                    },
                    () => {
                        // Ad finished
                        Debug.Log("[MidgameAds] ✅ Ad completed successfully");
                        _adsShown++;
                        onAdFinished?.Invoke();
                    }
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"[MidgameAds] Exception calling RequestAd: {e.Message}");
                onAdFailed?.Invoke();
            }
#else
            Debug.Log("[MidgameAds] Not WebGL build - no ad shown (CrazyGames only works in WebGL)");
            // For non-WebGL platforms, just call the finished callback immediately
            onAdFinished?.Invoke();
#endif
        }
        
        /// <summary>
        /// Reset ad tracking (useful for new game/restart)
        /// </summary>
        public static void Reset()
        {
            _adsShown = 0;
            _lastAdTime = -999f;
            Debug.Log("[MidgameAds] Ad tracking reset");
        }
        
        /// <summary>
        /// Get debug info about ad state
        /// </summary>
        public static string GetDebugInfo()
        {
            float timeSinceLastAd = Time.time - _lastAdTime;
            float cooldownRemaining = Mathf.Max(0, (CooldownMinutes * 60f) - timeSinceLastAd);
            
            return $"Ads shown: {_adsShown}, " +
                   $"Cooldown: {cooldownRemaining:F1}s remaining, " +
                   $"Wave freq: every {WaveFrequency} waves";
        }
        
        /// <summary>
        /// Update settings at runtime
        /// </summary>
        public static void UpdateSettings(int waveFrequency, float cooldownMinutes)
        {
            WaveFrequency = waveFrequency;
            CooldownMinutes = cooldownMinutes;
            Debug.Log($"[MidgameAds] Settings updated - Wave freq: {WaveFrequency}, Cooldown: {CooldownMinutes}min");
        }
    }
}