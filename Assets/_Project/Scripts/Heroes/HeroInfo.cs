using System;
using System.Collections.Generic;
using Project.Stats;
using System.Linq;
using Project.Game.Items;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Heroes
{
    [System.Serializable]
    public class HeroInfo
    {
        public event Action CurrentWeaponsChanged; 
        public event Action ShopCurrencyChanged; 

        public HeroData Data { get; private set; }

        public List<StatInfo> Stats { get; private set; } = new();
        public List<WeaponData> CurrentWeapons { get; private set; } = new();

        public int ShopCurrency
        {
            get => _shopCurrency;
            set
            {
                if (_shopCurrency != value)
                {
                    _shopCurrency = Mathf.Clamp(value, 0, int.MaxValue);
                    ShopCurrencyChanged?.Invoke();
                }
            }
        }

        private int _shopCurrency;
    
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

        public void AddWeapon(WeaponData weaponData)
        {
            if (CurrentWeapons.Count == Data.WeaponSlots)
            {
                return;
            }
            
            CurrentWeapons.Add(weaponData);
            CurrentWeaponsChanged?.Invoke();
        }
        
        public void RemoveWeaponAtIndex(int weaponIndex)
        {
            if (weaponIndex >= CurrentWeapons.Count)
            {
                Debug.LogError($"No weapon at index {weaponIndex}");
                return;
            }
            
            CurrentWeapons.RemoveAt(weaponIndex);
            CurrentWeaponsChanged?.Invoke();
        }
        

        public StatInfo GetStat(StatData statData)
        {
            return Stats.FirstOrDefault(x => x.Data == statData);
        }

        public void AddItem(ItemData item)
        {
            foreach (var statModifier in item.StatModifiers)
            {
                var statInfo = GetStat(statModifier.StatData);
                statInfo.AddModifier(statModifier);
            }
        }
    }
}