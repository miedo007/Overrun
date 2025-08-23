using JetBrains.Annotations;

namespace Mtl.Bonfire
{
    [PublicAPI]
    public static class AnalyticsEvents
    {
        [PublicAPI]
        public static class LevelStart
        {
            public const string EventId = "level_start";
            public const string LevelId = "level_id";
            public const string LevelName = "level_name";
            public const string LevelTag = "level_tag";
        }
        
        [PublicAPI]
        public static class LevelSummary
        {
            public const string EventId = "level_summary";
            public const string LevelId = "level_id";
            public const string LevelName = "level_name";
            public const string LevelTag = "level_tag";
            public const string LevelResult = "level_result";
            public const string ObjectiveTargetName = "objective_target_name_{0}";
            public const string ObjectiveTargetCount = "objective_target_count_{0}";
            public const string ObjectiveTargetCleared = "objective_target_cleared_{0}";
            public const string CheckpointCount = "checkpoint_count";
            public const string CheckpointCleared = "checkpoint_cleared";
            public const string MoveCount = "moves_used";
            public const string TimeSpent = "time_spent";
        }
        
        [PublicAPI]
        public static class ResourceTransaction
        {
            public const string EventId = "resource_transaction";
            public const string TransactionType = "transaction_type";
        }
    }
}