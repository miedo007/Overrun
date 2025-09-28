using UnityEngine;

namespace Project.Game.Levels
{
    public class DoubleRewardDebugger : MonoBehaviour
    {
        private void Update()
        {
            #if UNITY_EDITOR
            // F8 - Simulate successful double reward ad
            if (Input.GetKeyDown(KeyCode.F8))
            {
                SimulateSuccessfulDoubleRewardAd();
            }
            
            // F9 - Simulate failed double reward ad
            if (Input.GetKeyDown(KeyCode.F9))
            {
                SimulateFailedDoubleRewardAd();
            }
            #endif
        }

        private void SimulateSuccessfulDoubleRewardAd()
        {
            Debug.Log("[DoubleRewardDebugger] 🎬 F8 - Simulating SUCCESSFUL double reward ad");
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => Debug.Log("[DoubleRewardDebugger] ✅ Double reward ad SUCCESS callback triggered"),
                onAdFailed: () => Debug.Log("[DoubleRewardDebugger] ❌ This shouldn't happen")
            );
        }

        private void SimulateFailedDoubleRewardAd()
        {
            Debug.Log("[DoubleRewardDebugger] 🎬 F9 - Simulating FAILED double reward ad");
            
            // Since RewardedAds doesn't currently have a failure mode in editor,
            // we just log what would happen
            Debug.Log("[DoubleRewardDebugger] ❌ Simulated ad failure - buttons should be re-enabled");
        }

        private void OnGUI()
        {
            #if UNITY_EDITOR
            if (!UnityEngine.Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 400, 300, 100));
            GUILayout.Label("Double Reward Debug:");
            GUILayout.Label("F8 - Simulate Successful Double Reward Ad");
            GUILayout.Label("F9 - Simulate Failed Double Reward Ad");
            GUILayout.EndArea();
            #endif
        }
    }
}