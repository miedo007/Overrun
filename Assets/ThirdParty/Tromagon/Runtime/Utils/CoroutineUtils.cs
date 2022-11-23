using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Tromagon.Utils
{
    [PublicAPI]
    public static class CoroutineUtils
    {
        public static MonoBehaviour WaitUntil(this MonoBehaviour owner, Func<bool> predicate, Action callback)
        {
            owner.StartCoroutine(InternalWaitUntil(predicate, callback));
            return owner;
        }
        
        public static MonoBehaviour WaitForFrames(this MonoBehaviour owner, int frameCount, Action callback)
        {
            owner.StartCoroutine(InternalWaitForFrames(frameCount, callback));
            return owner;
        }
        
        public static MonoBehaviour WaitForSeconds(this MonoBehaviour owner, int seconds, Action callback)
        {
            owner.StartCoroutine(InternalWaitForTime(seconds, callback));
            return owner;
        }
        
        public static MonoBehaviour WaitForSecondsRealtime(this MonoBehaviour owner, int seconds, Action callback)
        {
            owner.StartCoroutine(InternalWaitForSecondsRealtime(seconds, callback));
            return owner;
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