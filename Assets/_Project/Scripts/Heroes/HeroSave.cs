using System;
using System.Collections.Generic;
using Mtl.Save;
using Newtonsoft.Json;

namespace Project.Heroes
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class HeroSave : Save, IHeroSave
    {
        [JsonProperty] public string SelectedHero { get; set; }
        [JsonProperty] public HeroLevels HeroLevels { get; set; } = new ();

        public void Clear()
        {
            SelectedHero = "";
            HeroLevels = new HeroLevels();
        }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class HeroLevels
    {
        [JsonProperty] public List<HeroLevelPair> HeroLevelPairs { get; set; } = new();
    }
    
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class HeroLevelPair
    {
        [JsonProperty] public string Hero { get; set; }
        [JsonProperty] public string Level { get; set; }
    }

    public interface IHeroSave
    {
        string SelectedHero { get; }
        HeroLevels HeroLevels { get; }
        void Clear();
    }
}