using System;

namespace Mtl.Bonfire
{
    public interface IBonfire
    {
        void Initialize(Action onComplete);
    }
}