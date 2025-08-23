using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class RectExtensions
    {
        /// <summary>
        /// Clamps the provided position in the target rect
        /// </summary>
        public static void Clamp(this Rect rect, ref Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
            position.z = Mathf.Clamp(position.z, rect.yMin, rect.yMax);
        }
    }
}