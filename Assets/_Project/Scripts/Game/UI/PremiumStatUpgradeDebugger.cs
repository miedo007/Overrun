using UnityEngine;
using Project.Game.Levels;

namespace Project.Game.UI
{
    /// <summary>
    /// Debug component to test premium stat upgrade functionality in the editor
    /// Add this to any GameObject in the scene during development
    /// </summary>
    public class PremiumStatUpgradeDebugger : MonoBehaviour
    {
        [Header("Debug Controls")]
        [SerializeField] private bool enableDebugButtons = true;
        [SerializeField] private KeyCode testSuccessKey = KeyCode.F1;
        [SerializeField] private KeyCode testFailureKey = KeyCode.F2;

        private void Update()
        {
            if (!enableDebugButtons) return;

            if (Input.GetKeyDown(testSuccessKey))
            {
                Debug.Log("[PremiumStatUpgradeDebugger] 🎯 Testing successful premium stat upgrade ad (F1)");
                TestPremiumUpgradeAd(true);
            }
            
            if (Input.GetKeyDown(testFailureKey))
            {
                Debug.Log("[PremiumStatUpgradeDebugger] 💥 Testing failed premium stat upgrade ad (F2)");
                TestPremiumUpgradeAd(false);
            }
        }

        private void TestPremiumUpgradeAd(bool shouldSucceed)
        {
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    if (shouldSucceed)
                    {
                        Debug.Log("[PremiumStatUpgradeDebugger] ✅ Simulated premium stat upgrade ad success!");
                    }
                },
                onAdFailed: () => {
                    if (!shouldSucceed)
                    {
                        Debug.Log("[PremiumStatUpgradeDebugger] ❌ Simulated premium stat upgrade ad failure!");
                    }
                }
            );
        }

        private void OnGUI()
        {
            if (!enableDebugButtons) return;

            GUILayout.BeginArea(new Rect(10, 400, 350, 120));
            GUILayout.Label("Premium Stat Upgrade Debugger", GUI.skin.box);
            
            if (GUILayout.Button("Test Successful Premium Stat Ad (F1)"))
            {
                TestPremiumUpgradeAd(true);
            }
            
            if (GUILayout.Button("Test Failed Premium Stat Ad (F2)"))
            {
                TestPremiumUpgradeAd(false);
            }
            
            GUILayout.Label("Keys: F1 = Success, F2 = Failure");
            GUILayout.EndArea();
        }
    }
}