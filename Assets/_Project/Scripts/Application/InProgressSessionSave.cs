using System;
using System.Collections.Generic;
using Mtl.Save;
using Newtonsoft.Json;

namespace Project.Application
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class InProgressSessionSave : Save, IInProgressSessionSave
    {
        [JsonProperty] public bool InProgress { get; set; } = false;
        [JsonProperty] public string HeroId { get; set; } = "";
        [JsonProperty] public int LevelIndex { get; set; } = -1;
        [JsonProperty] public int WaveIndex { get; set; } = -1;
        [JsonProperty] public float ShopCurrency { get; set; } = -1;
        [JsonProperty] public float Health { get; set; } = -1;
        [JsonProperty] public List<string> Weapons { get; set; } = new();
        [JsonProperty] public List<string> Items { get; set; } = new();

        public void Clear()
        {
            InProgress = false;
            HeroId = "";
            LevelIndex = -1;
            WaveIndex = -1;
            ShopCurrency = -1;
            Health = -1;
            Weapons = new();
            Items = new();
        }
    }

    public interface IInProgressSessionSave
    {
        bool InProgress { get; }
        string HeroId { get; }
        int LevelIndex { get; }
        int WaveIndex { get; }
        float ShopCurrency { get; }
        float Health { get; }
        List<string> Weapons { get; set; }
        List<string>  Items { get; set; }
        void Clear();
    }
}