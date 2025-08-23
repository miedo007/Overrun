using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtl.Bonfire
{
    [PublicAPI]
    public class AnalyticsProviderGroup : IAnalyticsProvider
    {
        private readonly List<IAnalyticsProvider> _analyticsProviders = new List<IAnalyticsProvider>();

        public void Add(IAnalyticsProvider provider)
        {
            _analyticsProviders.Add(provider);
        }

        public void SendEvent(string eventName, Dictionary<string, object> eventParams)
        {
            foreach (var analyticsProvider in _analyticsProviders)
            {
                analyticsProvider.SendEvent(eventName, eventParams);
            }
        }
    }
}