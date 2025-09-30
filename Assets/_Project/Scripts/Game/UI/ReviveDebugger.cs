using UnityEngine;
using Project.Game.Levels;
using Project.Game.Player;

namespace Project.Game.UI
{
    /// <summary>
    /// Debug component to test revive functionality and verify fixes
    /// Add this to any GameObject in the scene during development
    /// </summary>
    public class ReviveDebugger : MonoBehaviour
    {
        [Header("Debug Controls")]
        [SerializeField] private bool enableDebugUI = false; // Set to true to show debug controls
        [SerializeField] private KeyCode testReviveKey = KeyCode.R;
        [SerializeField] private KeyCode checkTimerKey = KeyCode.T;
        [SerializeField] private KeyCode checkWeaponsKey = KeyCode.Y;

        private void Update()
        {
            if (!enableDebugUI) return;

            if (Input.GetKeyDown(testReviveKey))
            {
                Debug.Log("[ReviveDebugger] 🔄 Testing revive state (R key)");
                TestReviveState();
            }
            
            if (Input.GetKeyDown(checkTimerKey))
            {
                Debug.Log("[ReviveDebugger] ⏰ Checking wave timer state (T key)");
                CheckWaveTimerState();
            }
            
            if (Input.GetKeyDown(checkWeaponsKey))
            {
                Debug.Log("[ReviveDebugger] ⚔️ Checking weapon animation state (Y key)");
                CheckWeaponAnimationState();
            }
        }

        private void TestReviveState()
        {
            // This would simulate checking what happens during revive
            Debug.Log("[ReviveDebugger] Checking overall game state for revive readiness...");
            
            var gameController = FindObjectOfType<Project.Game.GameController>();
            if (gameController == null)
            {
                Debug.LogWarning("[ReviveDebugger] ⚠️ GameController not found!");
                return;
            }
            
            Debug.Log("[ReviveDebugger] ✅ GameController found and active");
        }

        private void CheckWaveTimerState()
        {
            var hudScreen = FindObjectOfType<HudScreen>();
            if (hudScreen == null)
            {
                Debug.LogWarning("[ReviveDebugger] ⚠️ HudScreen not found!");
                return;
            }

            var timer = hudScreen.Timer;
            if (timer == null)
            {
                Debug.LogWarning("[ReviveDebugger] ⚠️ WaveTimer component not found!");
                return;
            }

            bool isTimerActive = timer.Root.gameObject.activeSelf;
            bool isWaveTextEnabled = hudScreen.WaveIndexText.enabled;
            
            Debug.Log($"[ReviveDebugger] Timer Root Active: {isTimerActive}");
            Debug.Log($"[ReviveDebugger] Wave Text Enabled: {isWaveTextEnabled}");
            Debug.Log($"[ReviveDebugger] Wave Text: '{hudScreen.WaveIndexText.text}'");
            
            if (!isTimerActive)
            {
                Debug.LogError("[ReviveDebugger] 🚨 Wave timer is HIDDEN! This is the bug.");
            }
            else
            {
                Debug.Log("[ReviveDebugger] ✅ Wave timer is visible");
            }
        }

        private void CheckWeaponAnimationState()
        {
            var playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning("[ReviveDebugger] ⚠️ PlayerController not found!");
                return;
            }

            // Check both Animation and Animator components
            var animations = playerController.GetComponentsInChildren<Animation>();
            var animators = playerController.GetComponentsInChildren<Animator>();
            
            Debug.Log($"[ReviveDebugger] Found {animations.Length} Animation components:");
            foreach (var anim in animations)
            {
                bool isPlaying = anim.isPlaying;
                Debug.Log($"[ReviveDebugger] - {anim.gameObject.name}: Playing={isPlaying}");
                
                if (isPlaying)
                {
                    Debug.Log($"[ReviveDebugger]   Current Clip: {anim.clip?.name}");
                }
            }
            
            Debug.Log($"[ReviveDebugger] Found {animators.Length} Animator components:");
            foreach (var animator in animators)
            {
                bool isEnabled = animator.enabled;
                Debug.Log($"[ReviveDebugger] - {animator.gameObject.name}: Enabled={isEnabled}");
                
                if (isEnabled && animator.runtimeAnimatorController != null)
                {
                    var currentState = animator.GetCurrentAnimatorStateInfo(0);
                    Debug.Log($"[ReviveDebugger]   Current State: {currentState.shortNameHash} (Speed: {animator.speed})");
                }
            }
        }

        private void OnGUI()
        {
            if (!enableDebugUI) return;

            GUILayout.BeginArea(new Rect(10, 670, 400, 120));
            GUILayout.Label("Revive Debugger", GUI.skin.box);
            
            if (GUILayout.Button("Test Revive State (R)"))
            {
                TestReviveState();
            }
            
            if (GUILayout.Button("Check Wave Timer (T)"))
            {
                CheckWaveTimerState();
            }
            
            if (GUILayout.Button("Check Weapons (Y)"))
            {
                CheckWeaponAnimationState();
            }
            
            GUILayout.Label("Keys: R = Revive, T = Timer, Y = Weapons");
            GUILayout.EndArea();
        }
    }
}