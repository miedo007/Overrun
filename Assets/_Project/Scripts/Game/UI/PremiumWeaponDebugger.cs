using UnityEngine;
using Project.Game.Levels;

namespace Project.Game.UI
{
    /// <summary>
    /// Debug component to test premium weapon functionality in the editor
    /// Add this to any GameObject in the scene during development
    /// </summary>
    public class PremiumWeaponDebugger : MonoBehaviour
    {
        [Header("Debug Controls")]
        [SerializeField] private bool enableDebugButtons = true;
        [SerializeField] private KeyCode testSuccessKey = KeyCode.F3;
        [SerializeField] private KeyCode testFailureKey = KeyCode.F4;

        private void Update()
        {
            if (!enableDebugButtons) return;

            if (Input.GetKeyDown(testSuccessKey))
            {
                Debug.Log("[PremiumWeaponDebugger] 🎯 Testing successful premium weapon ad (F3)");
                TestPremiumWeaponAd(true);
            }
            
            if (Input.GetKeyDown(testFailureKey))
            {
                Debug.Log("[PremiumWeaponDebugger] 💥 Testing failed premium weapon ad (F4)");
                TestPremiumWeaponAd(false);
            }
        }

        private void TestPremiumWeaponAd(bool shouldSucceed)
        {
            RewardedAds.ShowRewardedAd(
                onAdFinished: () => {
                    if (shouldSucceed)
                    {
                        Debug.Log("[PremiumWeaponDebugger] ✅ Simulated premium weapon ad success!");
                    }
                },
                onAdFailed: () => {
                    if (!shouldSucceed)
                    {
                        Debug.Log("[PremiumWeaponDebugger] ❌ Simulated premium weapon ad failure!");
                    }
                }
            );
        }

        private void OnGUI()
        {
            if (!enableDebugButtons) return;

            GUILayout.BeginArea(new Rect(10, 530, 350, 120));
            GUILayout.Label("Premium Weapon Debugger", GUI.skin.box);
            
            if (GUILayout.Button("Test Successful Premium Weapon Ad (F3)"))
            {
                TestPremiumWeaponAd(true);
            }
            
            if (GUILayout.Button("Test Failed Premium Weapon Ad (F4)"))
            {
                TestPremiumWeaponAd(false);
            }
            
            GUILayout.Label("Keys: F3 = Success, F4 = Failure");
            GUILayout.EndArea();
        }
    }
}