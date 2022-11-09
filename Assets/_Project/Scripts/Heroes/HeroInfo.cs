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
        public event Action ItemsChanged; 
        public event Action ShopCurrencyChanged; 

        public HeroData Data { get; private set; }

        public List<StatInfo> Stats { get; private set; } = new();
        public List<WeaponData> CurrentWeapons { get; private set; } = new();
        public List<ItemData> Items { get; private set; } = new();

        public float ShopCurrency
        {
            get => _shopCurrency;
            set
            {
                if (!Mathf.Approximately(_shopCurrency, value))
                {
                    _shopCurrency = Mathf.Clamp(value, 0, float.MaxValue);
                    ShopCurrencyChanged?.Invoke();
                }
            }
        }

        private float _shopCurrency;
    
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
            Items.Add(item);
            
            foreach (var statModifier in item.StatModifiers)
            {
                var statInfo = GetStat(statModifier.StatData);
                statInfo.AddModifier(statModifier);
            }
            
            ItemsChanged?.Invoke();
        }

        public int GetShopCurrencyIntValue()
        {
            return Mathf.FloorToInt(_shopCurrency);
        }

        public void RemoveWeapon(WeaponData weaponData)
        {
            CurrentWeapons.Remove(weaponData);
            CurrentWeaponsChanged?.Invoke();
        }

        public void RemoveItem(ItemData itemData)
        {
            Items.Remove(itemData);
            foreach (var statModifier in itemData.StatModifiers)
            {
                var statInfo = GetStat(statModifier.StatData);
                statInfo.RemoveModifier(statModifier);
            }
            ItemsChanged?.Invoke();
            
        }
    }
}