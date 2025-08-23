using JetBrains.Annotations;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class MathUtils
    {
        public static float Mod(float x, float m) 
        {
            return (x % m + m) % m;
        } 
    }
}