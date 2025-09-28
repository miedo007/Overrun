using UnityEngine;

namespace Project.Game.Levels
{
    /// <summary>
    /// Debug utility for testing midgame ads in editor
    /// Add this to any GameObject in the scene
    /// </summary>
    public class MidgameAdsDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Start()
        {
            Debug.Log("=== MIDGAME ADS DEBUGGER ACTIVE ===");
            Debug.Log("[MidgameAdsDebugger] Press F2 to force show midgame ad");
            Debug.Log("[MidgameAdsDebugger] Press F3 to reset ad tracking");
            Debug.Log("[MidgameAdsDebugger] Press F4 to show debug info");
            Debug.Log("====================================");
        }
        
        private void Update()
        {
            // F2 to force show a midgame ad
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("[MidgameAdsDebugger] F2 pressed - Forcing midgame ad...");
                ForceShowMidgameAd();
            }
            
            // F3 to reset ad tracking
            if (Input.GetKeyDown(KeyCode.F3))
            {
                Debug.Log("[MidgameAdsDebugger] F3 pressed - Resetting ad tracking...");
                MidgameAds.Reset();
            }
            
            // F4 to show debug info
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Debug.Log($"[MidgameAdsDebugger] Debug info: {MidgameAds.GetDebugInfo()}");
            }
        }
        
        private void ForceShowMidgameAd()
        {
            Debug.Log("[MidgameAdsDebugger] Manually triggering midgame ad...");
            
            MidgameAds.ShowAd(
                onAdFinished: () => {
                    Debug.Log("[MidgameAdsDebugger] Manual ad completed successfully!");
                },
                onAdFailed: () => {
                    Debug.Log("[MidgameAdsDebugger] Manual ad failed!");
                }
            );
        }
        
        private void OnGUI()
        {
            // Show debug info on screen
            GUI.color = Color.white;
            GUI.backgroundColor = Color.black;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            GUILayout.Label("MIDGAME ADS DEBUG", GUI.skin.box);
            GUILayout.Label($"Debug Info: {MidgameAds.GetDebugInfo()}");
            GUILayout.Label("F2: Force Ad | F3: Reset | F4: Log Info");
            
            if (GUILayout.Button("Force Midgame Ad"))
            {
                ForceShowMidgameAd();
            }
            
            GUILayout.EndArea();
        }
#endif
    }
}