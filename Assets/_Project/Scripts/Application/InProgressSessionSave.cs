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
        [JsonProperty] public bool InProgress { get; set; }
        [JsonProperty] public int LevelIndex { get; set; }
        [JsonProperty] public int WaveIndex { get; set; }
        [JsonProperty] public float ShopCurrency { get; set; }
        [JsonProperty] public float Health { get; set; }
        [JsonProperty] public List<string> Weapons { get; set; }
        [JsonProperty] public List<string> Items { get; set; }

        public void Clear()
        {
            InProgress = false;
            LevelIndex = 0;
            WaveIndex = 0;
            ShopCurrency = 0;
            Health = 0;
            Weapons = null;
            Items = null;
        }
    }

    public interface IInProgressSessionSave
    {
        bool InProgress { get; }
        int LevelIndex { get; }
        int WaveIndex { get; }
        float ShopCurrency { get; }
        float Health { get; }
        List<string> Weapons { get; set; }
        List<string>  Items { get; set; }
        void Clear();
    }
}