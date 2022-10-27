using System;
using Mtl.Injection;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public PlayerCharacter Character { get; private set; }
        [field: SerializeField] public PlayerWeaponsController WeaponsController { get; private set; }
        [field: SerializeField] public HeroViewController HeroPrefab { get; private set; }
        
        [Inject] private readonly UltimateJoystick _joystick;


        private void Awake()
        {
            var heroView = Instantiate(HeroPrefab, transform);
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