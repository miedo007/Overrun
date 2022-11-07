using JetBrains.Annotations;

namespace Tromagon.Runtime.Utils
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