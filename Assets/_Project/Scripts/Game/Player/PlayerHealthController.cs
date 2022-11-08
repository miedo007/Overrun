using System;
using System.Collections;
using Mtl.Injection;
using Project.Game.Levels;
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
        [field: SerializeField] public StatData HealthRegenStat { get; private set; }
        [field: SerializeField] public StatData HealthStealStat { get; private set; }

        private StatInfo _healthStatInfo;
        private StatInfo _healthRegenStatInfo;
        private StatInfo _healthStealRegenStatInfo;
        
        private float _currentHealth;
        
        [Inject] private readonly HeroInfo _heroInfo;
        [Inject] private readonly LevelController _levelController;
        [Inject] private readonly GameData _gameData;

        public float MaxHealth { get; private set; }

        public float CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                var previousValue = _currentHealth;
                _currentHealth = Mathf.Clamp(value, 0, MaxHealth);

                if (Mathf.Approximately(previousValue, _currentHealth))
                {
                    return;
                }
                
                if (_currentHealth <= 0)
                {
                    _currentHealth = 0;
                    StopCoroutine(HealthRegenRoutine());
                    Depleted?.Invoke();
                }
                
                Changed?.Invoke(previousValue / MaxHealth, _currentHealth / MaxHealth);
            }
        }


        private void Start()
        {
            Initialize();
            _levelController.WaveStarted += OnWaveStarted;
            _levelController.WaveCompleted += OnWaveCompleted;
            _levelController.LevelCompleted += OnWaveCompleted;
        }

        public void Initialize()
        {
            _healthStatInfo = _heroInfo.GetStat(HealthStat);
            _healthStatInfo.Changed += OnHealthStatChanged;

            _healthRegenStatInfo = _heroInfo.GetStat(HealthRegenStat);
            _healthRegenStatInfo.Changed += OnHealthRegenStatChanged;

            MaxHealth = _healthStatInfo.GetFloatValue();
            CurrentHealth = MaxHealth;
            Initialized?.Invoke();
        }

        private void OnHealthStatChanged(StatInfo stat)
        {
            var previousMaxHealth = MaxHealth;
            MaxHealth = stat.GetFloatValue();

            var delta = MaxHealth - previousMaxHealth;
            CurrentHealth += delta;
        }

        private void OnHealthRegenStatChanged(StatInfo stat)
        {
            
        }

        private void OnWaveStarted()
        {
            StartCoroutine(HealthRegenRoutine());
        }

        private void OnWaveCompleted()
        {
            StopAllCoroutines();
        }

        private IEnumerator HealthRegenRoutine()
        {
            while (enabled)
            {
                var time = _gameData.HealthRegenRate;
                while (time > 0)
                {
                    yield return null;
                    time -= Time.deltaTime;
                }
                
                var newHealth = CurrentHealth + _healthRegenStatInfo.GetFloatValue();
                // Don't let negative health regen kill the player. Clamp to 0.1
                CurrentHealth = Mathf.Max(newHealth, 0.1f);
            }
        }
        
        public void ReduceHealth(float amount)
        {
            CurrentHealth -= amount;
        }

        public string GetHealthString()
        {
            return $"{_currentHealth:0.0}<size=75%><alpha=#AA>/{MaxHealth:0.0}";
        }
    }
}