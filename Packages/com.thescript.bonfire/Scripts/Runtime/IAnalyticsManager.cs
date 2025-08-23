using JetBrains.Annotations;

namespace Mtl.Bonfire
{
    [PublicAPI]
    public interface IAnalyticsManager : IAnalyticsProvider
    {
        void SendLevelStartEvent(LevelStartEvent levelStartEvent);
        void SendLevelSummaryEvent(LevelSummaryEvent levelSummaryEvent);
    }
}