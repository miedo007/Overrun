using System;
using System.Linq;
using Mtl.Save;
using UnityEngine;

namespace Project.Heroes
{
    public class HeroRegistry : ISavable
    {
        public event Action OnChanged;
        public event Action<HeroInfo> ActiveHeroChanged;

        private HeroInfo _activeHeroInfo;

        private readonly HeroSave _save = new HeroSave();

        public Save Save => _save;
        public IHeroSave HeroSave => _save;
        public HeroDatabase Database { get; private set; }

        public HeroInfo ActiveHero
        {
            get => _activeHeroInfo;
            set
            {
                _activeHeroInfo = value;
                ActiveHeroChanged?.Invoke(_activeHeroInfo);
            }
        }
        
        public void Initialize(HeroDatabase database)
        {
            Database = database;
            ActiveHero = GetSelectedHeroInfo();
        }

        public HeroData GetHeroDataAtIndex(int index)
        {
            return Database.Heroes[index];
        }

        public HeroData GetSelectedHero()
        {
            var savedHeroId = _save.SelectedHero;
            if (string.IsNullOrEmpty(savedHeroId))
            {
                savedHeroId = Database.Heroes[0].name;
                SetSelectedHero(savedHeroId);
            }
            
            var heroData = Database.GetHeroWithId(savedHeroId);
            if (heroData == null)
            {
                Debug.LogError($"Error retrieving hero data with id {savedHeroId}");
                heroData = Database.DefaultHero;
            }
            
            return heroData;
        }

        public HeroInfo GetSelectedHeroInfo()
        {
            var heroData = GetSelectedHero();
            return GetHeroInfo(heroData);
        }

        public HeroInfo GetHeroInfo(HeroData heroData, int level = -1)
        {
            var heroLevel = level < 0 ? _save.GetHeroLevel(heroData.Id) : level;
            return new HeroInfo(heroData, Database.DefaultStats, heroLevel);
        }

        public void SetSelectedHero(string id)
        {
            Debug.Log($"selected hero changed :: {id}");
            _save.SelectedHero = id;
            OnChanged?.Invoke();
        }

        public void LoadSelectedHero()
        {
            ActiveHero = GetSelectedHeroInfo();
        }

        public void SetActiveHero(HeroData heroData)
        {
            ActiveHero = GetHeroInfo(heroData);
        }

        public void UpgradeActiveHero()
        {
            var heroId = ActiveHero.Data.Id;
            _save.IncrementHeroLevel(heroId);
            SetActiveHero(ActiveHero.Data);
            OnChanged?.Invoke();
            ActiveHeroChanged?.Invoke(ActiveHero);
        }

        public HeroData GetHeroData(string heroId)
        {
            return Database.Heroes.FirstOrDefault(x => x.Id == heroId);
        }

        public void SetActiveHero(string heroId)
        {
            SetActiveHero(GetHeroData(heroId));
        }
    }
}