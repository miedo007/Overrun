using UnityEngine;

namespace Project.Heroes
{
    public class HeroRegistry
    {
        public HeroDatabase Database { get; private set; }

        public HeroRegistry()
        {
            Database = Resources.Load<HeroDatabase>("database_heroes_default");
        }

        public HeroData GetHeroAtIndex(int index)
        {
            return Database.Heroes[index];
        }

        public HeroData GetActiveHero()
        {
            return GetHeroAtIndex(0);
        }

        public HeroInfo GetActiveHeroInfo()
        {
            var heroData = GetActiveHero();
            return new HeroInfo(heroData, Database.DefaultStats, 0);
        }
    }
}