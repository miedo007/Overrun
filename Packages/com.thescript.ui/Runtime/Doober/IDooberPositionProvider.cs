using UnityEngine;

namespace Mtl.UI
{
    public interface IDooberPositionProvider
    {
        /// <summary>
        /// The priority of this provider, higher is processed first
        /// </summary>
        int DooberProviderPriority { get; }
        
        Vector2? GetScreenPosition(object forVal, Camera cam);
    }

    public interface IDooberPositionProvider<in T> : IDooberPositionProvider
    {
        Vector2? GetScreenPosition(T forVal, Camera cam);
    }

    public abstract class DooberPositionProvider<T> : IDooberPositionProvider<T>
    {
        public int DooberProviderPriority => 0;
        
        public abstract Vector2? GetScreenPosition(T forVal, Camera cam);

        Vector2? IDooberPositionProvider.GetScreenPosition(object forVal, Camera cam) => GetScreenPosition((T) forVal, cam);
    }
}