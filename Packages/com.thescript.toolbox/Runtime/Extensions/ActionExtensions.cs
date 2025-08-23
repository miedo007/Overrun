using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class ActionExtensions
    {
        /// <summary>
        /// Invokes the delegate while try-catching each invocation target
        /// </summary>
        public static void SafeInvoke(this Action action) =>
            SafeInvokeInternal(action);

        /// <summary>
        /// Invokes the delegate while try-catching each invocation target
        /// </summary>
        public static void SafeInvoke<T0>(this Action<T0> action, T0 arg0) =>
            SafeInvokeInternal(action, arg0);

        /// <summary>
        /// Invokes the delegate while try-catching each invocation target
        /// </summary>
        public static void SafeInvoke<T0, T1>(this Action<T0, T1> action, T0 arg0, T1 arg1) =>
            SafeInvokeInternal(action, arg0, arg1);

        /// <summary>
        /// Invokes the delegate while try-catching each invocation target
        /// </summary>
        public static void SafeInvoke<T0, T1, T2>(this Action<T0, T1, T2> action, T0 arg0, T1 arg1, T2 arg2) =>
            SafeInvokeInternal(action, arg0, arg1, arg2);

        /// <summary>
        /// Invokes the delegate while try-catching each invocation target
        /// </summary>
        public static void SafeInvoke<T0, T1, T2, T3>(this Action<T0, T1, T2, T3> action, T0 arg0, T1 arg1, T2 arg2, T3 arg3) =>
            SafeInvokeInternal(action, arg0, arg1, arg2, arg3);

        private static void SafeInvokeInternal(MulticastDelegate multicastDelegate, params object[] args)
        {
            var actionCopy = (MulticastDelegate)multicastDelegate.Clone();
            foreach (var invocation in actionCopy.GetInvocationList())
            {
                try
                {
                    invocation.DynamicInvoke(args);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}