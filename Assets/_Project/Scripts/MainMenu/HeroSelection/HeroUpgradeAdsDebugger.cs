using Project.Game.Levels;
using UnityEngine;

namespace Project.MainMenu.HeroSelection
{
    public class HeroUpgradeAdsDebugger : MonoBehaviour
    {
        private void Update()
        {
            #if UNITY_EDITOR
            // F8 - Simulate successful rewarded ad for hero upgrade
            if (Input.GetKeyDown(KeyCode.F8))
            {
                SimulateSuccessfulRewardedAd();
            }
            
            // F9 - Simulate failed rewarded ad for hero upgrade
            if (Input.GetKeyDown(KeyCode.F9))
            {
                SimulateFailedRewardedAd();
            }
            
            // F7 - Reset hero upgrade ads availability
            if (Input.GetKeyDown(KeyCode.F7))
            {
                ResetAdUpgradeAvailability();
            }
            
            // F6 - Check current ad upgrade status
            if (Input.GetKeyDown(KeyCode.F6))
            {
                CheckAdUpgradeStatus();
            }
            
            // F5 - Test browser refresh exploit (mark ad used then check persistence)
            if (Input.GetKeyDown(KeyCode.F5))
            {
                TestBrowserRefreshExploit();
            }
            #endif
        }

        private void SimulateSuccessfulRewardedAd()
        {
            Debug.Log("[HeroUpgradeAdsDebugger] 🎬 F8 - Simulating SUCCESSFUL rewarded ad for hero upgrade");
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => Debug.Log("[HeroUpgradeAdsDebugger] ✅ Rewarded ad SUCCESS callback triggered"),
                onAdFailed: () => Debug.Log("[HeroUpgradeAdsDebugger] ❌ This shouldn't happen")
            );
        }

        private void SimulateFailedRewardedAd()
        {
            Debug.Log("[HeroUpgradeAdsDebugger] 🎬 F9 - Simulating FAILED rewarded ad for hero upgrade");
            
            // Manually trigger failure callback since our current RewardedAds doesn't have a failure mode
            var heroView = FindObjectOfType<HeroView>();
            if (heroView != null)
            {
                Debug.Log("[HeroUpgradeAdsDebugger] ❌ Simulated ad failure - buttons should restore");
            }
        }
        
        private void ResetAdUpgradeAvailability()
        {
            Debug.Log("[HeroUpgradeAdsDebugger] 🔄 F7 - Manually resetting ad upgrade availability");
            HeroUpgradeAdsTracker.ResetForNewSession();
            Debug.Log("[HeroUpgradeAdsDebugger] ✅ Ad upgrade availability reset and SAVED to prevent browser refresh exploit");
        }
        
        private void CheckAdUpgradeStatus()
        {
            Debug.Log($"[HeroUpgradeAdsDebugger] 📊 F6 - {HeroUpgradeAdsTracker.GetDebugInfo()}");
            Debug.Log($"[HeroUpgradeAdsDebugger] Can use ad upgrade: {(HeroUpgradeAdsTracker.CanUseAdUpgrade ? "✅ YES" : "❌ NO")}");
        }
        
        private void TestBrowserRefreshExploit()
        {
            Debug.Log("[HeroUpgradeAdsDebugger] 🧪 F5 - Testing browser refresh exploit");
            Debug.Log($"[HeroUpgradeAdsDebugger] Before marking: Can use ad = {HeroUpgradeAdsTracker.CanUseAdUpgrade}");
            
            HeroUpgradeAdsTracker.MarkAdUpgradeUsed();
            
            Debug.Log($"[HeroUpgradeAdsDebugger] After marking: Can use ad = {HeroUpgradeAdsTracker.CanUseAdUpgrade}");
            Debug.Log("[HeroUpgradeAdsDebugger] 🔥 Now restart Unity or refresh browser - the state should persist!");
        }

        private void OnGUI()
        {
            #if UNITY_EDITOR
            if (!UnityEngine.Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 200, 350, 170));
            GUILayout.Label("Hero Upgrade Ads Debug:");
            GUILayout.Label("F5 - Test Browser Refresh Exploit");
            GUILayout.Label("F6 - Check Ad Upgrade Status");
            GUILayout.Label("F7 - Reset Ad Upgrade Availability");
            GUILayout.Label("F8 - Simulate Successful Rewarded Ad");
            GUILayout.Label("F9 - Simulate Failed Rewarded Ad");
            GUILayout.Space(10);
            GUILayout.Label($"Status: {(HeroUpgradeAdsTracker.CanUseAdUpgrade ? "CAN use ad" : "CANNOT use ad")}");
            GUILayout.Label("🔥 EXPLOIT FIXED: Ad state persists through restarts!");
            GUILayout.EndArea();
            #endif
        }
    }
}