using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class BoundsExtensions
    {
        /// <summary>
        /// Gets the rect that represents the bounds
        /// </summary>
        public static Rect GetRect(this Bounds bounds) => new Rect(bounds.center, bounds.size);
    }
}