using System;
using System.Collections;
using Mtl.Injection;
using Project.Game.Items;
using Project.Game.Levels;
using Project.Heroes;
using Project.PopupText;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerHealthController : MonoBehaviour
    {
        public event Action Initialized;
        public event Action<float, float> Changed;
        public event Action DamageTaken;
        public event Action Depleted;

        [field: SerializeField] public StatData HealthStat { get; private set; }
        [field: SerializeField] public StatData HealthRegenStat { get; private set; }
        [field: SerializeField] public ItemBehaviourTrigger HealthLostTrigger { get; private set; }

        private StatInfo _healthStatInfo;
        private StatInfo _healthRegenStatInfo;
        
        private float _currentHealth;
        
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly LevelController _levelController;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly PopupTextManager _popupTextManager;

        public float MaxHealth { get; private set; }

        public float CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                var previousValue = _currentHealth;
                _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
                
                if (_currentHealth <= 0)
                {
                    _currentHealth = 0;
                    StopCoroutine(HealthRegenRoutine());
                    Depleted?.Invoke();
                }
                
                Changed?.Invoke(previousValue / MaxHealth, _currentHealth / MaxHealth);
            }
        }

        public float CurrentPercentage => _currentHealth / MaxHealth;
        public bool IsFull => _currentHealth >= MaxHealth;

        private void Start()
        {
            _levelController.WaveStarted += OnWaveStarted;
            _levelController.WaveCompleted += OnWaveCompleted;
            _levelController.LevelCompleted += OnWaveCompleted;
        }

        public void Initialize()
        {
            _healthStatInfo = _heroRegistry.ActiveHero.GetStat(HealthStat);
            _healthStatInfo.Changed += OnHealthStatChanged;

            _healthRegenStatInfo = _heroRegistry.ActiveHero.GetStat(HealthRegenStat);
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
            if (delta > 0)
            {
                CurrentHealth += delta;
            }
            else
            {
                CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
            }
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

                var delta = MaxHealth * _healthRegenStatInfo.GetFloatValue();
                var newHealth = CurrentHealth + delta;
                
                // Don't let negative health regen kill the player. Clamp to 0.1
                newHealth = Mathf.Clamp(newHealth, 0.1f, MaxHealth); 
                
                if (!Mathf.Approximately(newHealth, CurrentHealth))
                {
                    if (delta < 0)
                    {
                        HealthLostTrigger.Trigger(transform.position);
                    }
                    
                    CurrentHealth = newHealth;
                    
                    _popupTextManager.DisplayTextAtPosition($"{Math.Round(delta, 1)}",
                        transform.position,
                        delta > 0
                            ? _popupTextManager.PlayerHealingPrefab
                            : _popupTextManager.PlayerDamagePrefab);
                }
            }
        }
        
        public void ReduceHealth(float amount)
        {
            CurrentHealth -= amount;
            HealthLostTrigger.Trigger(transform.position);
            DamageTaken?.Invoke();
        }

        public string GetHealthString()
        {
            var currentHealthDisplay = _currentHealth - Mathf.Round(_currentHealth) == 0
                ? $"{_currentHealth:0}"
                : $"{_currentHealth:0.0}";
            
            var maxHealthDisplay = MaxHealth - Mathf.Round(MaxHealth) == 0
                ? $"{MaxHealth:0}"
                : $"{MaxHealth:0.0}";

            return $"{currentHealthDisplay}<size=75%><alpha=#AA>/{maxHealthDisplay}";
        }

        public void HealByPercentage(float percentage)
        {
            CurrentHealth += (MaxHealth * percentage);
        }
        
        public void HealByAmount(float amount)
        {
            CurrentHealth += amount;
        }
    }
}