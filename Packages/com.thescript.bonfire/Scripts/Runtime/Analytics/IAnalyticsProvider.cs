using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtl.Bonfire
{
    [PublicAPI]
    public interface IAnalyticsProvider
    {
        void SendEvent(string eventName, Dictionary<string, object> eventParams);
    }
}