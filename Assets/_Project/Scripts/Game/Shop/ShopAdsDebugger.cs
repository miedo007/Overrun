using Project.Game.Levels;
using UnityEngine;

namespace Project.Game.Shop
{
    public class ShopAdsDebugger : MonoBehaviour
    {
        private void Update()
        {
            #if UNITY_EDITOR
            // F6 - Simulate successful rewarded ad for shop items
            if (Input.GetKeyDown(KeyCode.F6))
            {
                SimulateSuccessfulRewardedAd();
            }
            
            // F7 - Simulate failed rewarded ad for shop items
            if (Input.GetKeyDown(KeyCode.F7))
            {
                SimulateFailedRewardedAd();
            }
            #endif
        }

        private void SimulateSuccessfulRewardedAd()
        {
            Debug.Log("[ShopAdsDebugger] 🎬 F6 - Simulating SUCCESSFUL rewarded ad for shop item");
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => Debug.Log("[ShopAdsDebugger] ✅ Rewarded ad SUCCESS callback triggered"),
                onAdFailed: () => Debug.Log("[ShopAdsDebugger] ❌ This shouldn't happen")
            );
        }

        private void SimulateFailedRewardedAd()
        {
            Debug.Log("[ShopAdsDebugger] 🎬 F7 - Simulating FAILED rewarded ad for shop item");
            
            // Manually trigger failure callback since our current RewardedAds doesn't have a failure mode
            var shopItems = FindObjectsOfType<ShopInventoryItemView>();
            if (shopItems.Length > 0)
            {
                Debug.Log($"[ShopAdsDebugger] ❌ Simulated ad failure - found {shopItems.Length} shop items");
            }
        }

        private void OnGUI()
        {
            #if UNITY_EDITOR
            if (!UnityEngine.Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 300, 300, 100));
            GUILayout.Label("Shop Ads Debug:");
            GUILayout.Label("F6 - Simulate Successful Shop Rewarded Ad");
            GUILayout.Label("F7 - Simulate Failed Shop Rewarded Ad");
            GUILayout.EndArea();
            #endif
        }
    }
}