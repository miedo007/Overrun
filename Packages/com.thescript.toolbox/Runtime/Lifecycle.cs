using System;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Mtl.Toolbox
{
    public static class Lifecycle
    {
        /// <summary>
        /// Executes every second, useful to synchronise events across the game
        /// </summary>
        public static event Action OnSecondPassed;

        /// <summary>
        /// Common lifecyle callback after the Update() is run on scripts
        /// </summary>
        public static event Action OnUpdate;

        /// <summary>
        /// Executes on the next frame before the Update() loop
        /// </summary>
        public static event Action OnNextFrame;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Mtl/Lifecycle/Log Default Player Loop")]
        private static void LogDefaultLoop()
        {
            PlayerLoop.GetDefaultPlayerLoop().LogSystem();
        }

        [UnityEditor.MenuItem("Mtl/Lifecycle/Log Current Player Loop")]
        private static void LogCurrentLoop()
        {
            PlayerLoop.GetCurrentPlayerLoop().LogSystem();
        }

        [UnityEditor.MenuItem("Mtl/Lifecycle/Log Default Player Loop", true)]
        [UnityEditor.MenuItem("Mtl/Lifecycle/Log Current Player Loop", true)]
        private static bool ValidateLogLoop() => Application.isPlaying;
#endif

        /// <summary>
        /// Inject the custom lifecycle systems into the Unity playerloop
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ModifyPlayerLoop()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.InsertAfter(typeof(TimeUpdate), new PlayerLoopSystem
            {
                type = typeof(SecondPassed),
                updateDelegate = SecondPassed.Process
            });

            playerLoop.InsertAfter(typeof(UnityEngine.PlayerLoop.Update.ScriptRunBehaviourUpdate), new PlayerLoopSystem
            {
                type = typeof(Update),
                updateDelegate = Update.Process
            });

            playerLoop.InsertAfter(typeof(PreUpdate), new PlayerLoopSystem
            {
                type = typeof(DelayedFrame),
                updateDelegate = DelayedFrame.Process
            });

            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        /// <summary>
        /// Executes every second, useful to synchronise events across the game
        /// </summary>
        private struct SecondPassed
        {
            private static long _lastTime;

            public static void Process()
            {
                var newTime = (long) Time.realtimeSinceStartupAsDouble;
                if (newTime != _lastTime && OnSecondPassed != null)
                {
                    OnSecondPassed?.SafeInvoke();
                }

                _lastTime = newTime;
            }
        }

        /// <summary>
        /// Common lifecyle callback after the Update() is run on scripts
        /// </summary>
        private struct Update
        {
            public static void Process()
            {
                OnUpdate?.SafeInvoke();
            }
        }

        /// <summary>
        /// Executes on the next frame before the Update() loop
        /// </summary>
        private struct DelayedFrame
        {
            public static void Process()
            {
                var ev = OnNextFrame;
                OnNextFrame = null;
                ev?.SafeInvoke();
            }
        }
    }
}