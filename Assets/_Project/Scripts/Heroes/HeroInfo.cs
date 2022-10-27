using System.Collections.Generic;
using Project.Stats;
using System.Linq;

namespace Project.Heroes
{
    public class HeroInfo
    {
        public HeroData Data { get; private set; }

        public List<StatInfo> Stats { get; private set; } = new();
        
        private HeroInfo() {}

        public HeroInfo(HeroData heroData, CharacterStats defaultStats, int heroLevel)
        {
            Data = heroData;
            
            foreach (var stat in defaultStats.Stats)
            {
                // does this hero have an override for this stat?
                StatInfo statOverride = null;
                if (Data.StatOverrides != null)
                {
                    statOverride = Data.StatOverrides.Stats.FirstOrDefault(x => x.Data == stat.Data); 
                }
                
                Stats.Add(statOverride == null ? stat.GetLeveledStatInfo(heroLevel) : statOverride.GetLeveledStatInfo(heroLevel));
            }
        }

        public StatInfo GetStat(StatData statData)
        {
            return Stats.FirstOrDefault(x => x.Data == statData);
        }
    }
}