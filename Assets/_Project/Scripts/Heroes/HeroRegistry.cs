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
        private SaveManager _saveManager;

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
        
        public void Initialize(SaveManager saveManager, HeroDatabase database)
        {
            _saveManager = saveManager;
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
            Debug.Log($"[HeroRegistry] GetHeroInfo - Hero: {heroData.Id}, Requested Level: {level}, Save Level: {_save.GetHeroLevel(heroData.Id)}, Final Level: {heroLevel}");
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
            var selectedHeroData = GetSelectedHero();
            var heroId = selectedHeroData.Id;
            var savedLevel = _save.GetHeroLevel(heroId);
            
            Debug.Log($"[HeroRegistry] LoadSelectedHero - Hero ID: {heroId}, Saved Level: {savedLevel}");
            
            ActiveHero = GetSelectedHeroInfo();
            
            Debug.Log($"[HeroRegistry] LoadSelectedHero - Active Hero Level after load: {ActiveHero.Level}");
        }

        public void SetActiveHero(HeroData heroData)
        {
            ActiveHero = GetHeroInfo(heroData);
        }

        public void UpgradeActiveHero()
        {
            var heroId = ActiveHero.Data.Id;
            Debug.Log($"[HeroRegistry] Upgrading hero {heroId} from level {_save.GetHeroLevel(heroId)}");
            
            _save.IncrementHeroLevel(heroId);
            
            var newLevel = _save.GetHeroLevel(heroId);
            Debug.Log($"[HeroRegistry] Hero {heroId} upgraded to level {newLevel}");
            
            SetActiveHero(ActiveHero.Data);
            OnChanged?.Invoke();
            Debug.Log("[HeroRegistry] OnChanged event triggered for save");
            
            ActiveHeroChanged?.Invoke(ActiveHero);
            
            // Force immediate save when hero upgrades
            _saveManager?.Save();
            Debug.Log("[HeroRegistry] Immediate hero upgrade save completed!");
        }

        /// <summary>
        /// Force the save system to mark this registry as dirty for saving
        /// </summary>
        public void ForceSaveUpdate()
        {
            Debug.Log("[HeroRegistry] Forcing save update - triggering OnChanged event");
            OnChanged?.Invoke();
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