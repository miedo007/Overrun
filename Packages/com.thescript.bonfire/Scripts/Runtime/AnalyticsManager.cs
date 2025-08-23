using System.Collections.Generic;
using JetBrains.Annotations;
using Mtl.Toolbox;
using LevelStart = Mtl.Bonfire.AnalyticsEvents.LevelStart;
using LevelSummary = Mtl.Bonfire.AnalyticsEvents.LevelSummary;

namespace Mtl.Bonfire
{
    [PublicAPI]
    public class AnalyticsManager : IAnalyticsManager
    {
        private readonly List<IAnalyticsProvider> _providers;
        private readonly Dictionary<string, Dictionary<string, object>> _eventBuffer;

        public AnalyticsManager()
        {
            _providers = new List<IAnalyticsProvider>();
            _eventBuffer = new Dictionary<string, Dictionary<string, object>>();
        }

        public void Register(IAnalyticsProvider analyticsProvider)
        {
            if (analyticsProvider is IAnalyticsInitializer initializer)
            {
                initializer.Initialize();
            }

            _providers.Add(analyticsProvider);
        }

        public void SendLevelStartEvent(LevelStartEvent levelStartEvent)
        {
            if (!_eventBuffer.TryGetValue(LevelStart.EventId, out var fields))
            {
                fields = new Dictionary<string, object>();
                _eventBuffer.Add(LevelStart.EventId, fields);
            }

            fields.Clear(); // Clear to be sure we don't send fields that should not have been populated
            fields[LevelStart.LevelId] = levelStartEvent.LevelId;
            fields[LevelStart.LevelName] = levelStartEvent.LevelName;
            SendEvent(LevelStart.EventId, fields);
        }

        public void SendLevelSummaryEvent(LevelSummaryEvent levelSummaryEvent)
        {
            if (!_eventBuffer.TryGetValue(LevelSummary.EventId, out var fields))
            {
                fields = new Dictionary<string, object>();
                _eventBuffer.Add(LevelSummary.EventId, fields);
            }

            fields.Clear(); // Clear to be sure we don't send fields that should not have been populated
            fields[LevelSummary.LevelId] = levelSummaryEvent.LevelId;
            fields[LevelSummary.LevelName] = levelSummaryEvent.LevelName;
            fields[LevelSummary.LevelResult] = levelSummaryEvent.Completed ? 1 : 0;
            if (levelSummaryEvent.CheckpointCount.HasValue)
            {
                fields[LevelSummary.CheckpointCount] = levelSummaryEvent.CheckpointCount;
            }

            if (levelSummaryEvent.CheckpointCleared.HasValue)
            {
                fields[LevelSummary.CheckpointCleared] = levelSummaryEvent.CheckpointCleared;
            }

            SendEvent(LevelSummary.EventId, fields);
        }

        public void SendEvent(string eventName, params (string, object)[] eventParams)
        {
            using var dict = DictionaryPool.Get(eventParams);
            SendEvent(eventName, dict);
        }

        public void SendEvent(string eventName, Dictionary<string, object> eventParams)
        {
            foreach (var analyticsProvider in _providers)
            {
                analyticsProvider.SendEvent(eventName, eventParams);
            }
        }
    }
}