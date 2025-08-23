using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class UnityThreading
    {
        // True will only be assigned on the thread that raises the static ctor
        // So any other thread will be false
        // ReSharper disable once ThreadStaticFieldHasInitializer
        [ThreadStatic]
        public static readonly bool IsMainThread = true;

        private static readonly TaskFactory MainThreadTaskFactory;
        
        public static bool IsPlaying { get; private set; }
#if !UNITY_EDITOR
            = true;
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Touch()
        {
            // Touch method to ensure the main thread is the one to static init this class
        }

        static UnityThreading()
        {
            // Grab a scheduler for Unity's main thread
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            MainThreadTaskFactory = new TaskFactory(scheduler);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += state => IsPlaying = state == UnityEditor.PlayModeStateChange.EnteredPlayMode;
#endif
        }

        /// <summary>
        /// Execute an Action on the main thread (supports being called from the main thread)
        /// </summary>
        /// <param name="a">The Action to execute</param>
        public static Task ExecuteOnMainThread(this Action a)
        {
            if (!IsMainThread)
            {
                return MainThreadTaskFactory.StartNew(a);
            }

            try
            {
                a.Invoke();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                try
                {
                    // Create a new throw with the proper context to pass to the task failure
                    throw new Exception("Failed to execute task on main thread (see inner exception for details)", e);
                }
                catch (Exception exception)
                {
                    return Task.FromException(exception);
                }
            }
        }

        /// <summary>
        /// Execute a Func on the main thread (supports being called from the main thread)
        /// </summary>
        /// <param name="f">The Func to execute</param>
        public static Task<T> ExecuteOnMainThread<T>(this Func<T> f)
        {
            if (!IsMainThread)
            {
                return MainThreadTaskFactory.StartNew(f);
            }

            try
            {
                return Task.FromResult(f.Invoke());
            }
            catch (Exception e)
            {
                try
                {
                    // Create a new throw with the proper context to pass to the task failure
                    throw new Exception("Failed to execute task on main thread (see inner exception for details)", e);
                }
                catch (Exception exception)
                {
                    return Task.FromException<T>(exception);
                }
            }
        }
    }
}