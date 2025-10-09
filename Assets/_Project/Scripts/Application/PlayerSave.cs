using System;
using Mtl.Save;
using Newtonsoft.Json;

namespace Project.Application
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class PlayerSave : Save, IPlayerSave
    {
        [JsonProperty] public int Currency { get; set; }
        [JsonProperty] public int TopStageIndex { get; set; }
        [JsonProperty] public bool HasUsedHeroAdUpgradeThisSession { get; set; } = false;
        [JsonProperty] public double LastHeroAdUpgradeTime { get; set; } = 0;

        public void Clear()
        {
            Currency = 0;
            TopStageIndex = 0;
            HasUsedHeroAdUpgradeThisSession = false;
            LastHeroAdUpgradeTime = 0;
        }
    }

    public interface IPlayerSave
    {
        int Currency { get; }
        int TopStageIndex { get; }
        bool HasUsedHeroAdUpgradeThisSession { get; }
        double LastHeroAdUpgradeTime { get; }
        void Clear();
    }
}