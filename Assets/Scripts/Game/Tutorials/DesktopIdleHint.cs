using UnityEngine;

namespace Project.Game.Tutorials
{
    public class DesktopIdleHint : IdleHintBase
    {
        public override void Begin(int waveIndex = 0)
        {
            if (tracking) return;

            currentWaveIndex = waveIndex;
            tracking = true;
            idleTimer = 0f;

            // Desktop: Always show hints immediately on first wave
            if (currentWaveIndex == 0)
            {
                cooldownActive = false;
                ShowImmediate();
            }
            // Subsequent waves: start cooldown
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
    }
}