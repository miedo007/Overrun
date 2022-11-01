using Mtl.Injection;
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
        
        [Inject] private readonly UltimateJoystick _joystick;
        [Inject] private readonly HeroInfo _heroInfo;
        
        private Transform _transform;
        private StatInfo _speedStat;

        public Vector3 Position => _transform.position;

        private void Awake()
        {
            _transform = transform;
        }

        public void OnReady()
        {
            var heroView = Instantiate(_heroInfo.Data.Prefab, transform);
            heroView.Initialize(Character);

            foreach (var item in _heroInfo.Data.StartingItems)
            {
                _heroInfo.AddItem(item);
            }

            _speedStat = _heroInfo.GetStat(SpeedStatData);
            _speedStat.Changed += OnSpeedStatChanged;
            OnSpeedStatChanged(_speedStat);
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