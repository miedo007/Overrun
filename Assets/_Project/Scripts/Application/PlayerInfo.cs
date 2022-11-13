using System;
using Mtl.Save;

namespace Project.Application
{
    public class PlayerInfo : ISavable
    {
        public event Action OnChanged;
        public event Action OnCurrencyChanged;
        public Save Save => _save;

        public IPlayerSave PlayerSave => _save;
        private readonly PlayerSave _save = new PlayerSave();

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
        }
    }
}