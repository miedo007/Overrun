using JetBrains.Annotations;

namespace Mtl.Bonfire
{
    [PublicAPI]
    public static class AnalyticsConstants
    {
        [PublicAPI]
        public static class PlayerState
        {
            public const string FreeHardCurrency = "bal_hc_free";
            public const string PaidHardCurrency = "bal_hc_paid";
            public const string Lives = "bal_hc_paid";
            public const string Resources = "resource_bal_{0}";
            public const string GameMode = "game_mode";
            public const string MaxLevel = "max_level";
            public const string LevelAttempts = "level_attempt";
            public const string WinStreak = "win_streak";
            public const string Avatar = "ego";
            public const string AvatarLevel = "ego_level";
            public const string AvatarXp = "ego_xp";
            public const string AvatarSkillName = "ego_skill_{0}";
            public const string AvatarSkillLevel = "ego_skill_level_{0}";
            public const string AvatarSkillXp = "ego_skill_level_{0}";
        }

        [PublicAPI]
        public static class Transaction
        {
            public const string FreeHardCurrencyGain = "hc_free_gained";
            public const string PaidHardCurrencyGain = "hc_paid_gained";
            public const string FreeHardCurrencyUsed = "hc_free_used";
            public const string PaidHardCurrencyUsed = "hc_paid_used";
            public const string RealMoneyUsed = "rm_used";
            public const string ResourceName = "resource_name_{0}";
            public const string ResourceGained = "resource_gained_{0}";
            public const string ResourceUsed = "resource_used_{0}";
            public const string ScreenId = "screen_id";
        }
    }
}