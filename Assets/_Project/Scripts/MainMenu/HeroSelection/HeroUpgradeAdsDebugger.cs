using Project.Game.Levels;
using UnityEngine;

namespace Project.MainMenu.HeroSelection
{
    /// <summary>
    /// Updated debug helper for testing hero upgrade ad timer functionality
    /// </summary>
    public class HeroUpgradeAdsDebugger : MonoBehaviour
    {
        private void Update()
        {
            #if UNITY_EDITOR
            // F5 - Reset cooldown timer
            if (Input.GetKeyDown(KeyCode.F5))
            {
                ResetCooldownTimer();
            }
            
            // F6 - Check current ad upgrade status
            if (Input.GetKeyDown(KeyCode.F6))
            {
                CheckAdUpgradeStatus();
            }
            
            // F7 - Trigger cooldown timer
            if (Input.GetKeyDown(KeyCode.F7))
            {
                TriggerCooldownTimer();
            }
            
            // F8 - Set short cooldown (30 seconds)
            if (Input.GetKeyDown(KeyCode.F8))
            {
                SetShortCooldown();
            }
            
            // F9 - Set long cooldown (10 minutes)
            if (Input.GetKeyDown(KeyCode.F9))
            {
                SetLongCooldown();
            }
            #endif
        }

        private void ResetCooldownTimer()
        {
            Debug.Log("[HeroUpgradeAdsDebugger] 🔄 F5 - Resetting ad upgrade cooldown timer");
            HeroUpgradeAdsTracker.ResetCooldown();
            Debug.Log("[HeroUpgradeAdsDebugger] ✅ Cooldown timer reset - player can use ad upgrade again");
        }
        
        private void CheckAdUpgradeStatus()
        {
            Debug.Log($"[HeroUpgradeAdsDebugger] 📊 F6 - {HeroUpgradeAdsTracker.GetDebugInfo()}");
            Debug.Log($"[HeroUpgradeAdsDebugger] Can use ad upgrade: {(HeroUpgradeAdsTracker.CanUseAdUpgrade ? "✅ YES" : "❌ NO")}");
            Debug.Log($"[HeroUpgradeAdsDebugger] Remaining time: {HeroUpgradeAdsTracker.GetRemainingCooldownTime():F1} seconds");
            Debug.Log($"[HeroUpgradeAdsDebugger] Display text: '{HeroUpgradeAdsTracker.GetCooldownDisplayText()}'");
        }
        
        private void TriggerCooldownTimer()
        {
            Debug.Log("[HeroUpgradeAdsDebugger] ⏰ F7 - Triggering ad upgrade cooldown timer");
            HeroUpgradeAdsTracker.MarkAdUpgradeUsed();
            Debug.Log($"[HeroUpgradeAdsDebugger] ✅ Cooldown started - {HeroUpgradeAdsTracker.CooldownDuration} second timer active");
        }
        
        private void SetShortCooldown()
        {
            Debug.Log("[HeroUpgradeAdsDebugger] ⚡ F8 - Setting short cooldown: 30 seconds");
            HeroUpgradeAdsTracker.CooldownDuration = 30f;
            HeroUpgradeAdsTracker.MarkAdUpgradeUsed();
            Debug.Log("[HeroUpgradeAdsDebugger] ✅ Short cooldown active for testing");
        }
        
        private void SetLongCooldown()
        {
            Debug.Log("[HeroUpgradeAdsDebugger] 🐌 F9 - Setting long cooldown: 10 minutes");
            HeroUpgradeAdsTracker.CooldownDuration = 600f;
            HeroUpgradeAdsTracker.MarkAdUpgradeUsed();
            Debug.Log("[HeroUpgradeAdsDebugger] ✅ Long cooldown active for testing");
        }

        private void OnGUI()
        {
            #if UNITY_EDITOR
            if (!UnityEngine.Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 200, 400, 200));
            GUILayout.Label("Hero Upgrade Ads Timer Debug:", GUI.skin.box);
            
            var canUse = HeroUpgradeAdsTracker.CanUseAdUpgrade;
            var remaining = HeroUpgradeAdsTracker.GetRemainingCooldownTime();
            var displayText = HeroUpgradeAdsTracker.GetCooldownDisplayText();
            
            GUILayout.Label($"Cooldown Duration: {HeroUpgradeAdsTracker.CooldownDuration} seconds");
            GUILayout.Label($"Can Use Ad: {(canUse ? "✅ YES" : "❌ NO")}");
            GUILayout.Label($"Remaining Time: {remaining:F1} seconds");
            GUILayout.Label($"Display Text: '{displayText}'");
            
            GUILayout.Space(10);
            GUILayout.Label("Debug Controls:");
            GUILayout.Label("F5 - Reset Cooldown Timer");
            GUILayout.Label("F6 - Check Status & Debug Info");
            GUILayout.Label("F7 - Trigger Cooldown Timer");
            GUILayout.Label("F8 - Set Short Cooldown (30s)");
            GUILayout.Label("F9 - Set Long Cooldown (10min)");
            
            GUILayout.EndArea();
            #endif
        }
    }
}