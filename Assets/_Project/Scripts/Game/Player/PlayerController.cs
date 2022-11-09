using System;
using Mtl.Injection;
using Project.Game.Collectibles;
using Project.Game.Projectiles;
using Project.Heroes;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerController : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public PlayerCharacter Character { get; private set; }
        [field: SerializeField] public PlayerWeaponsController WeaponsController { get; private set; }
        [field: SerializeField] public StatData SpeedStatData { get; private set; }
        [field: SerializeField] public Collector Collector { get; private set; }
        [field: SerializeField] public CollectibleData[] StoreCurrencyCollectibles { get; private set; }
        
        [Inject] private readonly UltimateJoystick _joystick;
        [Inject] private readonly HeroInfo _heroInfo;
        
        private Transform _transform;
        private StatInfo _speedStat;

        public Vector3 Position => _transform.position;

        private void Awake()
        {
            _transform = transform;
            foreach (var collectible in StoreCurrencyCollectibles)
            {
                collectible.Collected += OnStoreCurrencyCollected;
            }
        }

        private void OnDestroy()
        {
            foreach (var collectible in StoreCurrencyCollectibles)
            {
                collectible.Collected -= OnStoreCurrencyCollected;
            }
        }

        private void OnStoreCurrencyCollected(CollectibleData obj)
        {
            _heroInfo.ShopCurrency += obj.Value;
        }

        public void OnReady()
        {
            var heroView = Instantiate(_heroInfo.Data.Prefab, transform);
            heroView.Initialize(Character);

            foreach (var item in _heroInfo.Data.StartingItems)
            {
                _heroInfo.AddItem(item);
            }

            _heroInfo.ShopCurrency = _heroInfo.Data.StartingShopCurrency;
            
            _speedStat = _heroInfo.GetStat(SpeedStatData);
            _speedStat.Changed += OnSpeedStatChanged;
            OnSpeedStatChanged(_speedStat);
        }

        public void HandleWaveComplete()
        {
            enabled = false;
            Collector.CollectAll();
        }

        private void Start()
        {
            foreach (var weapon in _heroInfo.Data.StartingWeapons)
            {
                _heroInfo.AddWeapon(weapon);
            }
        }

        private void HandleInput()
        {
            var input = new Vector2(_joystick.GetHorizontalAxis(),  _joystick.GetVerticalAxis());
            Character.SetMovementDirection(input);
        }

        private void Update()
        {
            HandleInput();
        }
        
        private void OnSpeedStatChanged(StatInfo stat)
        {
            Character.MaxSpeed = _speedStat.GetFloatValue();
        }
    }
}