using UnityEngine;

namespace Project.Game.Levels
{
    /// <summary>
    /// Debug utility for testing rewarded ads in editor
    /// Add this to any GameObject in the scene
    /// </summary>
    public class RewardedAdsDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Start()
        {
            Debug.Log("=== REWARDED ADS DEBUGGER ACTIVE ===");
            Debug.Log("[RewardedAdsDebugger] Press F5 to test rewarded ad");
            Debug.Log("[RewardedAdsDebugger] Press F6 to reset rewarded ad tracking");
            Debug.Log("[RewardedAdsDebugger] Press F7 to show debug info");
            Debug.Log("=====================================");
        }
        
        private void Update()
        {
            // F5 to test rewarded ad
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("[RewardedAdsDebugger] F5 pressed - Testing rewarded ad...");
                TestRewardedAd();
            }
            
            // F6 to reset rewarded ad tracking
            if (Input.GetKeyDown(KeyCode.F6))
            {
                Debug.Log("[RewardedAdsDebugger] F6 pressed - Resetting rewarded ad tracking...");
                RewardedAds.Reset();
            }
            
            // F7 to show debug info
            if (Input.GetKeyDown(KeyCode.F7))
            {
                Debug.Log($"[RewardedAdsDebugger] Debug info: {RewardedAds.GetDebugInfo()}");
            }
        }
        
        private void TestRewardedAd()
        {
            Debug.Log("[RewardedAdsDebugger] Manually triggering rewarded ad...");
            
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    Debug.Log("[RewardedAdsDebugger] Manual rewarded ad completed successfully!");
                },
                onAdFailed: () => {
                    Debug.Log("[RewardedAdsDebugger] Manual rewarded ad failed!");
                }
            );
        }
        
        private void OnGUI()
        {
            // Show debug info on screen
            GUI.color = Color.white;
            GUI.backgroundColor = Color.black;
            
            GUILayout.BeginArea(new Rect(10, 120, 300, 100));
            GUILayout.Label("REWARDED ADS DEBUG", GUI.skin.box);
            GUILayout.Label($"Debug Info: {RewardedAds.GetDebugInfo()}");
            GUILayout.Label("F5: Test Ad | F6: Reset | F7: Log Info");
            
            if (GUILayout.Button("Test Rewarded Ad"))
            {
                TestRewardedAd();
            }
            
            GUILayout.EndArea();
        }
#endif
    }
}