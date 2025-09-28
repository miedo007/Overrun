using UnityEngine;

namespace Project.Game.Levels
{
    /// <summary>
    /// Configuration for midgame ads that you can tweak in the Inspector
    /// Attach this to any GameObject in your scene to configure ad settings
    /// </summary>
    public class MidgameAdsConfig : MonoBehaviour
    {
        [Header("Ad Frequency Settings")]
        [SerializeField] private int waveFrequency = 3;
        [Tooltip("Show ads every X waves (e.g., 3 = every 3rd wave)")]
        
        [SerializeField] private float cooldownMinutes = 2f;
        [Tooltip("Minimum time between ads in minutes to avoid spamming users")]
        
        [Header("Debug Info (Read Only)")]
        [SerializeField, TextArea(3, 5)] private string debugInfo = "";
        
        private void Start()
        {
            // Apply settings on start
            MidgameAds.UpdateSettings(waveFrequency, cooldownMinutes);
            Debug.Log($"[MidgameAdsConfig] Applied settings - Wave freq: {waveFrequency}, Cooldown: {cooldownMinutes}min");
        }
        
        private void Update()
        {
            // Update debug info display
            debugInfo = MidgameAds.GetDebugInfo();
            
            // Apply changes if tweaked in Inspector during runtime
            if (MidgameAds.WaveFrequency != waveFrequency || 
                MidgameAds.CooldownMinutes != cooldownMinutes)
            {
                MidgameAds.UpdateSettings(waveFrequency, cooldownMinutes);
            }
        }
        
        [ContextMenu("Reset Ad Tracking")]
        private void ResetAdTracking()
        {
            MidgameAds.Reset();
            Debug.Log("[MidgameAdsConfig] Ad tracking reset via context menu");
        }
        
        [ContextMenu("Test Show Ad")]
        private void TestShowAd()
        {
            Debug.Log("[MidgameAdsConfig] Testing ad via context menu...");
            MidgameAds.ShowAd(
                () => Debug.Log("[MidgameAdsConfig] Test ad completed!"),
                () => Debug.Log("[MidgameAdsConfig] Test ad failed!")
            );
        }
    }
}