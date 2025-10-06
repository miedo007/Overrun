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

        public void Clear()
        {
            Currency = 0;
            TopStageIndex = 0;
            HasUsedHeroAdUpgradeThisSession = false;
        }
    }

    public interface IPlayerSave
    {
        int Currency { get; }
        int TopStageIndex { get; }
        bool HasUsedHeroAdUpgradeThisSession { get; }
        void Clear();
    }
}