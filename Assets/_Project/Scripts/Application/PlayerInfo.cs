using System;
using Mtl.Save;

namespace Project.Application
{
    public class PlayerInfo : ISavable
    {
        public event Action OnChanged;
        public event Action OnCurrencyChanged;
        public event Action OnHeroAdUpgradeStateChanged;  // New event for ad state changes
        public Save Save => _save;

        public IPlayerSave PlayerSave => _save;
        private readonly PlayerSave _save = new PlayerSave();
        private SaveManager _saveManager;

        public void Initialize(SaveManager saveManager)
        {
            _saveManager = saveManager;
        }

        public void IncrementTopStage()
        {
            _save.TopStageIndex++;
            OnChanged?.Invoke();
        }

        public void Create()
        {
        }

        public void Clear()
        {
            _save.Clear();
            OnChanged?.Invoke();
        }

        public void ChangeCurrency(int delta)
        {
            _save.Currency += delta;
            OnCurrencyChanged?.Invoke();
            OnChanged?.Invoke();
            
            // Force immediate save when currency changes
            _saveManager?.Save();
        }
        
        public void ChangeHeroAdUpgradeState(bool hasUsed)
        {
            _save.HasUsedHeroAdUpgradeThisSession = hasUsed;
            OnHeroAdUpgradeStateChanged?.Invoke();
            OnChanged?.Invoke();
        }
        
        public void ChangeHeroAdUpgradeTime(double timestamp)
        {
            _save.LastHeroAdUpgradeTime = timestamp;
            OnHeroAdUpgradeStateChanged?.Invoke();
            OnChanged?.Invoke();
        }
        
        public void NotifyDataReloaded()
        {
            OnChanged?.Invoke();
            OnCurrencyChanged?.Invoke();
        }
    }
}