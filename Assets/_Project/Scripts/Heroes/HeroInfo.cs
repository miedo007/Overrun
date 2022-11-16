using System;
using System.Collections.Generic;
using Project.Stats;
using System.Linq;
using Mtl.Injection;
using Project.Application;
using Project.Game.Items;
using Project.Game.Weapons;
using Project.Tiers;
using UnityEngine;

namespace Project.Heroes
{
    [System.Serializable]
    public class HeroInfo
    {
        public event Action WeaponsWillChange;
        public event Action WeaponsChanged; 
        public event Action ItemsWillChange; 
        public event Action ItemsChanged; 
        public event Action ShopCurrencyChanged; 
        public event Action<int> CollectedContainersChanged; 

        public HeroData Data { get; private set; }

        public List<StatInfo> Stats { get; private set; } = new();
        public List<WeaponData> CurrentWeapons { get; private set; } = new();
        public List<ItemData> Items { get; private set; } = new();

        private int _waveRewards;
        private float _shopCurrency;

        public int WaveRewards
        {
            get => _waveRewards;
            set
            {
                if (_waveRewards == value)
                {
                    return;
                }

                var delta = value - _waveRewards;
                _waveRewards = value;
                CollectedContainersChanged?.Invoke(delta);
            }
        }

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
            
            WeaponsWillChange?.Invoke();
            CurrentWeapons.Add(weaponData);
            WeaponsChanged?.Invoke();
        }
        
        public void RemoveWeaponAtIndex(int weaponIndex)
        {
            if (weaponIndex >= CurrentWeapons.Count)
            {
                Debug.LogError($"No weapon at index {weaponIndex}");
                return;
            }
            
            WeaponsWillChange?.Invoke();
            CurrentWeapons.RemoveAt(weaponIndex);
            WeaponsChanged?.Invoke();
        }
        

        public StatInfo GetStat(StatData statData)
        {
            return Stats.FirstOrDefault(x => x.Data == statData);
        }

        public void AddItem(ItemData item)
        {
            ItemsWillChange?.Invoke();
            
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
            
            WeaponsWillChange?.Invoke();
            CurrentWeapons.Remove(weaponData);
            WeaponsChanged?.Invoke();
        }

        public void RemoveItem(ItemData itemData)
        {
            ItemsWillChange?.Invoke();
            Items.Remove(itemData);
            foreach (var statModifier in itemData.StatModifiers)
            {
                var statInfo = GetStat(statModifier.StatData);
                statInfo.RemoveModifier(statModifier);
            }
            ItemsChanged?.Invoke();
            
        }

        public bool CanMergeWeapon(WeaponData weaponData)
        {
            if (CurrentWeapons.Count(x => weaponData == x) < 2)
            {
                return false;
            }
            
            var weaponDatabase = InjectionContainer.Instance.Injector.Get<TieredGroupDatabase>("weapons");
            
            var tieredGroup = weaponDatabase.GetGroupForBaseData(weaponData);
            if (tieredGroup != null)
            {
                return tieredGroup.CanUpgradeTier(weaponData);
            }

            return false;
        }

        public bool MergeWeapon(WeaponData weaponData)
        {
            if (!CanMergeWeapon(weaponData))
            {
                Debug.Log("Merge failed");
                return false;
            }

            var weaponDatabase = InjectionContainer.Instance.Injector.Get<TieredGroupDatabase>("weapons");
            var tieredGroup = weaponDatabase.GetGroupForBaseData(weaponData);
            var nextTier = tieredGroup.GetNextTier(weaponData);
            if (nextTier == null)
            {
                Debug.Log("Merge failed = Tier is Null");
                return false;
            }
            
            // remove old weapons
            // TODO: Remove from correct slot. For now, just remove first two matching weaponData
            
            RemoveWeapon(weaponData);
            RemoveWeapon(CurrentWeapons.FirstOrDefault(x => x == weaponData));
            AddWeapon(nextTier.Data as WeaponData);
            
            return true;
        }

        public bool CanMerge(BaseData data)
        {
            var weaponData = data as WeaponData;
            return weaponData != null && CanMergeWeapon(weaponData);
        }

        public void AddToInventoryFromBaseData(BaseData objData)
        {
            if (objData as WeaponData != null)
            {
                AddWeapon(objData as WeaponData);
            }
            else
            {
                AddItem(objData as ItemData);
            }
        }
    }
}