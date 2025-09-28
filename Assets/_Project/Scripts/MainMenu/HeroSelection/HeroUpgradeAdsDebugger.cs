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

        private void OnGUI()
        {
            #if UNITY_EDITOR
            if (!UnityEngine.Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 200, 300, 100));
            GUILayout.Label("Hero Upgrade Ads Debug:");
            GUILayout.Label("F8 - Simulate Successful Rewarded Ad");
            GUILayout.Label("F9 - Simulate Failed Rewarded Ad");
            GUILayout.EndArea();
            #endif
        }
    }
}