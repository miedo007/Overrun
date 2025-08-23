using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Returns the screen position of the provided RectTransform
        /// </summary>
        /// <param name="tr">The RectTransform to get a position from</param>
        /// <param name="camera">Optional camera to use instead of the root canvas camera</param>
        /// <returns></returns>
        public static Vector2 GetScreenPosition(this RectTransform tr, Camera camera = null)
        {
            var canvas = tr.GetComponentInParent<Canvas>()?.rootCanvas;
            if (canvas != null && camera == null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                camera = canvas.worldCamera;
            }

            return camera != null ? camera.WorldToScreenPoint(tr.position) : tr.position;
        }

        /// <summary>
        /// Sets the size of the target RectTransform on both axis
        /// </summary>
        public static void SetSize(this RectTransform tr, Rect rect) => SetSize(tr, rect.size);

        /// <summary>
        /// Sets the size of the target RectTransform on both axis
        /// </summary>
        public static void SetSize(this RectTransform tr, Vector2 size)
        {
            tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            tr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
    }
}