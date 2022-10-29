using System;
using Mtl.Injection;
using Project.Heroes;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerHealthController : MonoBehaviour
    {
        public event Action Initialized;
        public event Action<float, float> Changed;
        public event Action Depleted;

        [field: SerializeField] public StatData HealthStat { get; private set; }

        private StatInfo _healthStatInfo;
        private float _currentHealth;
        
        [Inject] private readonly HeroInfo _heroInfo;

        public float MaxHealth => _healthStatInfo.GetFloatValue();

        public float CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                var previousValue = _currentHealth;
                _currentHealth = value;
                if (_currentHealth <= 0)
                {
                    _currentHealth = 0;
                    Depleted?.Invoke();
                }

                var maxHp = _healthStatInfo.GetFloatValue();
                Changed?.Invoke(previousValue / maxHp, _currentHealth / maxHp);
            }
        }


        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            _healthStatInfo = _heroInfo.GetStat(HealthStat);
            CurrentHealth = MaxHealth;
            Initialized?.Invoke();
        }

        public void ReduceHealth(float amount)
        {
            CurrentHealth -= amount;
        }
    }
}