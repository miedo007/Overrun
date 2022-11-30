using System;
using System.Collections.Generic;
using Mtl.Save;
using Project.Heroes;
using UnityEngine;

namespace Project.Application
{
    public class InProgressSessionInfo : ISavable
    {
        public event Action OnChanged;
        public Save Save => _save;

        public IInProgressSessionSave InProgressSave => _save;

        private readonly InProgressSessionSave _save = new InProgressSessionSave();

        public void SaveProgress(int levelIndex, int waveIndex, float health, HeroInfo heroInfo)
        {
            _save.InProgress = true;
            _save.HeroId = heroInfo.Data.Id;
            _save.LevelIndex = levelIndex;
            _save.WaveIndex = waveIndex;
            _save.Health = health;
            _save.ShopCurrency = heroInfo.ShopCurrency;

            _save.Weapons = new List<string>();
            foreach (var weaponData in heroInfo.CurrentWeapons)
            {
                _save.Weapons.Add(weaponData.Id);
            }
            
            _save.Items = new List<string>();
            foreach (var itemData in heroInfo.Items)
            {
                _save.Items.Add(itemData.Id);
            }
            
            OnChanged?.Invoke();
        }

        public void Create()
        {
        }

        public bool IsValid()
        {
            return _save.LevelIndex >= 0 &&
                   _save.WaveIndex >= 0 &&
                   _save.Health > 0 &&
                   _save.ShopCurrency >= 0 &&
                   !string.IsNullOrEmpty(_save.HeroId) &&
                   _save.Items != null &&
                   _save.Weapons != null &&
                   _save.Weapons.Count > 0;
        }

        public void ClearProgress()
        {
            _save.Clear();
            OnChanged?.Invoke();
        }
    }
}