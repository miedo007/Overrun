using System;
using System.Collections.Generic;
using System.Linq;
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

        public void IncrementHeroLevel(string heroId)
        {
            HeroLevels.IncrementHeroLevel(heroId);
        }

        public int GetHeroLevel(string heroId)
        {
            return HeroLevels.GetHeroLevel(heroId);
        }
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class HeroLevels
    {
        [JsonProperty] public List<HeroLevelPair> HeroLevelPairs { get; set; } = new();

        public void IncrementHeroLevel(string heroId)
        {
            var heroLevelPair = HeroLevelPairs.FirstOrDefault(x => x.HeroId == heroId);
            if (heroLevelPair == null)
            {
                heroLevelPair = new HeroLevelPair();
                heroLevelPair.HeroId = heroId;
                heroLevelPair.Level = 0;
                HeroLevelPairs.Add(heroLevelPair);
            }

            heroLevelPair.Level++;
        }

        public int GetHeroLevel(string heroId)
        {
            var heroLevelPair = HeroLevelPairs.FirstOrDefault(x => x.HeroId == heroId);
            if (heroLevelPair == null)
            {
                return 0;
            }
            
            return heroLevelPair.Level;
        }
    }
    
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class HeroLevelPair
    {
        [JsonProperty] public string HeroId { get; set; }
        [JsonProperty] public int Level { get; set; }
    }

    public interface IHeroSave
    {
        string SelectedHero { get; }
        HeroLevels HeroLevels { get; }
        void Clear();
    }
}