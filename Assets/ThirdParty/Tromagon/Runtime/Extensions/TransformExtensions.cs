using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tromagon.Extensions
{
    [PublicAPI]
    public static class TransformUtils
    {
        public static Transform RemoveAllChildren(this Transform transform, Func<GameObject, bool> predicate = null)
        {
            var enumerator = transform.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var go = ((Transform) enumerator.Current)?.gameObject;
                if (predicate != null && !predicate(go)) continue;
                Object.Destroy(go);
            }

            return transform;
        }
    }
}

