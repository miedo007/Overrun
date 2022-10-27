using System;
using Mtl.Injection;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerController : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public PlayerCharacter Character { get; private set; }
        [field: SerializeField] public PlayerWeaponsController WeaponsController { get; private set; }
        
        [Inject] private readonly UltimateJoystick _joystick;
        [Inject] private readonly HeroRegistry _heroRegistry;

        private HeroData _heroData;
        
        public void OnReady()
        {
            _heroData = _heroRegistry.GetHeroAtIndex(0);
            var heroView = Instantiate(_heroData.Prefab, transform);
            heroView.Initialize(Character);
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
    }
}