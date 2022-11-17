using System;
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
        
        public void Initialize()
        {
            Database = Resources.Load<HeroDatabase>("database_heroes_default");
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
                _save.SelectedHero = savedHeroId;
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

        public HeroInfo GetHeroInfo(HeroData heroData)
        {
            return new HeroInfo(heroData, Database.DefaultStats, 0);
        }

        public void SetSelectedHero(string id)
        {
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
    }
}