using UnityEngine;

namespace Project.Game.Tutorials
{
    public class MobileIdleHint : IdleHintBase
    {
        [Header("Mobile-Specific Settings")]
        [SerializeField] private bool disableHintsOnMobile = true;

        public override void Begin(int waveIndex = 0)
        {
            if (tracking) return;

            currentWaveIndex = waveIndex;
            tracking = true;
            idleTimer = 0f;

            // Mobile: Can disable hints entirely or show them differently
            if (disableHintsOnMobile)
            {
                // Keep tracking but never show hints
                HideImmediate();
                return;
            }

            // Alternative: Show touch-specific hints
            if (currentWaveIndex == 0)
            {
                cooldownActive = false;
                ShowImmediate();
            }
            else
            {
                cooldownActive = true;
                cooldownTimer = cooldownAfterFirstWave;
                HideImmediate();
            }

            if (target) lastPos = target.position;
        }

        public override void End()
        {
            tracking = false;
            cooldownActive = false;
            cooldownTimer = 0f;
            HideImmediate();
        }

        protected override void Show()
        {
            // Don't show if disabled on mobile
            if (disableHintsOnMobile) return;
            
            base.Show();
        }

        protected override void ShowImmediate()
        {
            // Don't show if disabled on mobile
            if (disableHintsOnMobile) return;
            
            base.ShowImmediate();
        }
    }
}