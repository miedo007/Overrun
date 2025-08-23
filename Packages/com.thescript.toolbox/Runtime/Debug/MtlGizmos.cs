using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class MtlGizmos
    {
        public static void DrawCircle(Vector3 origin, float radius, Vector3 axis, int points = 36) =>
            DrawCircle(origin, radius, axis, Color.white, -1f, true, points);

        public static void DrawCircle(Vector3 origin, float radius, Vector3 axis, Color color, int points = 36) =>
            DrawCircle(origin, radius, axis, color, -1f, true, points);

        public static void DrawCircle(Vector3 origin, float radius, Vector3 axis, Color color, float duration, int points = 36) =>
            DrawCircle(origin, radius, axis, color, duration, true, points);

        public static void DrawCircle(Vector3 origin, float radius, Vector3 axis, Color color, float duration, bool depthTest, int points = 36)
        {
            var plane = new Plane(axis, origin);
            var point = (plane.ClosestPointOnPlane(new Vector3(origin.y, origin.z, origin.x)) - origin).normalized * radius;

            for (var i = 0; i < points; ++i)
            {
                var p0 = origin + Quaternion.AngleAxis(i / (float) points * 360, axis) * point;
                var p1 = origin + Quaternion.AngleAxis((i + 1) / (float) points * 360, axis) * point;
                Debug.DrawLine(p0, p1, color, duration, depthTest);
            }
        }
    }
}