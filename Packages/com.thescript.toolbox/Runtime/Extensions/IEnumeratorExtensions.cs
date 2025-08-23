using System;
using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Mtl.Toolbox
{
    // ReSharper disable once InconsistentNaming
    [PublicAPI]
    public static class IEnumeratorExtensions
    {
        private static readonly Type RecursiveType = ToRecursiveInternal(null).GetType();

        /// <summary>
        /// Wraps an enumerator so MoveNext() properly handles nested enumerators
        /// </summary>
        /// <param name="enumerator">The enumerator to wrap</param>
        /// <returns>The wrapped enumerator</returns>
        public static IEnumerator ToRecursive(this IEnumerator enumerator)
        {
            if (enumerator == null || enumerator.GetType() == RecursiveType)
            {
                return enumerator;
            }

            return ToRecursiveInternal(enumerator);
        }

        private static IEnumerator ToRecursiveInternal(IEnumerator enumerator)
        {
            using var stack = StackPool.Get<IEnumerator>();
            stack.Push(enumerator);
            while (stack.Count > 0)
            {
                var e = stack.Peek();
                while (e.MoveNext())
                {
                    if (e.Current is IEnumerator subEnumerator)
                    {
                        stack.Push(subEnumerator);
                        // Push a null entry since breaking will pop the stack and we want to process subEnumerator
                        stack.Push(null);
                        break;
                    }

                    if (e.Current is AsyncOperation async)
                    {
                        while (!async.isDone)
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return e.Current;
                    }
                }

                stack.Pop();
            }

            stack.Clear();
        }

        /// <summary>
        /// Executes an enumerator until it is done
        /// </summary>
        /// <param name="enumerator">The enumerator to fully execute</param>
        /// <param name="msTimeout">When greater than 0, the enumerator will forcibly stop when running for longer than the set value</param>
        /// <param name="makeRecursive">If true, calls ToRecursive() on the enumerator first</param>
        public static void ExecuteInline(this IEnumerator enumerator, uint msTimeout, bool makeRecursive = true)
        {
            if (makeRecursive)
            {
                enumerator = enumerator.ToRecursive();
            }

            var stopwatch = Stopwatch.StartNew();
            while (enumerator.MoveNext())
            {
                if (msTimeout > 0 && stopwatch.ElapsedMilliseconds > msTimeout)
                {
                    Debug.LogError($"[Enumerator] Enumerator '{enumerator.GetType()}' timed-out while executing inline");
                    break;
                }
            }

            stopwatch.Stop();
        }
    }
}