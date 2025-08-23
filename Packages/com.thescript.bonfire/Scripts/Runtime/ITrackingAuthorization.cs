using System;
using JetBrains.Annotations;

namespace Mtl.Bonfire
{
    [PublicAPI]
    public interface ITrackingAuthorization
    {
        void RequestAuthorization(Action onComplete);
    }
}