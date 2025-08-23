using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class MonoBehaviourExtensions
    {
        public static Coroutine WaitUntil(this MonoBehaviour owner, Func<bool> predicate, Action callback)
        {
            return owner.StartCoroutine(InternalWaitUntil(predicate, callback));
        }
        
        public static Coroutine WaitForFrames(this MonoBehaviour owner, int frameCount, Action callback)
        {
            return owner.StartCoroutine(InternalWaitForFrames(frameCount, callback));
        }
        
        public static Coroutine WaitForSeconds(this MonoBehaviour owner, float seconds, Action callback)
        {
            return owner.StartCoroutine(InternalWaitForTime(seconds, callback));
        }
        
        public static Coroutine WaitForSecondsRealtime(this MonoBehaviour owner, float seconds, Action callback)
        {
            return owner.StartCoroutine(InternalWaitForSecondsRealtime(seconds, callback));
        }

        private static IEnumerator InternalWaitForFrames(int frameCount, Action callback)
        {
            while (--frameCount >= 0)
            {
                yield return new WaitForEndOfFrame();
            }

            callback();
        }

        private static IEnumerator InternalWaitForTime(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback();
        }
        
        private static IEnumerator InternalWaitForSecondsRealtime(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback();
        }
        
        private static IEnumerator InternalWaitUntil(Func<bool> predicate, Action callback)
        {
            yield return new WaitUntil(predicate);
            callback();
        }
    }
}