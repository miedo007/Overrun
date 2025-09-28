using System;
using UnityEngine;

namespace Project.Game.Levels
{
    /// <summary>
    /// Manages rewarded ads for CrazyGames (revive, bonus rewards, etc.)
    /// </summary>
    public static class RewardedAds
    {
        // Tracking
        private static int _rewardedAdsShown = 0;
        
        /// <summary>
        /// Show a rewarded ad using CrazyGames SDK
        /// </summary>
        public static void ShowRewardedAd(Action onAdFinished = null, Action onAdFailed = null)
        {
            Debug.Log($"[RewardedAds] ShowRewardedAd called - Platform: {UnityEngine.Application.platform}");
            Debug.Log($"[RewardedAds] Is WebGL: {UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer}");
            
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("[RewardedAds] Calling CrazySDK.Ad.RequestAd for rewarded ad");
            
            try
            {
                CrazyGames.CrazySDK.Ad.RequestAd(
                    CrazyGames.CrazyAdType.Rewarded,
                    () => {
                        // Ad started
                        Debug.Log("[RewardedAds] ✅ Rewarded ad started successfully");
                    },
                    (error) => {
                        // Ad error
                        Debug.Log($"[RewardedAds] ❌ Rewarded ad failed - Code: {error.code}, Message: {error.message}");
                        onAdFailed?.Invoke();
                    },
                    () => {
                        // Ad finished - this is where the reward should be given
                        Debug.Log("[RewardedAds] ✅ Rewarded ad completed successfully - giving reward!");
                        _rewardedAdsShown++;
                        onAdFinished?.Invoke();
                    }
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"[RewardedAds] Exception calling RequestAd: {e.Message}");
                onAdFailed?.Invoke();
            }
#else
            Debug.Log("[RewardedAds] Not WebGL build - simulating rewarded ad for testing");
            // For non-WebGL platforms, simulate successful ad for testing
            onAdFinished?.Invoke();
#endif
        }
        
        /// <summary>
        /// Reset rewarded ad tracking (useful for new game/restart)
        /// </summary>
        public static void Reset()
        {
            _rewardedAdsShown = 0;
            Debug.Log("[RewardedAds] Rewarded ad tracking reset");
        }
        
        /// <summary>
        /// Get debug info about rewarded ad state
        /// </summary>
        public static string GetDebugInfo()
        {
            return $"Rewarded ads shown: {_rewardedAdsShown}";
        }
    }
}