using System.Collections.Generic;
using System.Linq;
using Project.Heroes;
using Project.Stats;
using UnityEngine;

namespace Project.Heroes
{
    [CreateAssetMenu(fileName = "hero_database_", menuName = "Data/Heroes/HeroDatabase", order = 0)]
    public class HeroDatabase : ScriptableObject
    {
        [field: SerializeField] public CharacterStats DefaultStats { get; private set; }
        [field: SerializeField] public List<HeroData> Heroes { get; private set; }
        
        public HeroData DefaultHero => Heroes[0];

        public HeroData GetHeroWithId(string savedHeroId)
        {
            return Heroes.FirstOrDefault(x => string.Equals(x.name, savedHeroId));
        }
    }
}