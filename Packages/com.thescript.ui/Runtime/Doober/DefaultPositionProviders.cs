using Mtl.Toolbox;
using UnityEngine;

namespace Mtl.UI
{
    /// <summary>
    /// Default provider for RectTransform, will get the screen position based on the rect and provided camera
    /// </summary>
    internal class RectTransformProvider : DooberPositionProvider<RectTransform>
    {
        public override Vector2? GetScreenPosition(RectTransform forVal, Camera cam) => forVal.GetScreenPosition(cam);
    }

    /// <summary>
    /// Default provider for Vector2 positions, assumes screen-space
    /// </summary>
    internal class Vector2Provider : DooberPositionProvider<Vector2>
    {
        public override Vector2? GetScreenPosition(Vector2 forVal, Camera cam) => forVal;
    }
}