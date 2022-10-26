using System.Collections;
using Mtl.Injection;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public PlayerCharacter Character { get; private set; }
        [field: SerializeField] public PlayerWeaponsController WeaponsController { get; private set; }
        [field: SerializeField] public PlayerViewController ViewController { get; private set; }
        
        [Inject] private readonly UltimateJoystick _joystick;
        

        private void HandleInput()
        {
            var input = new Vector2(_joystick.GetHorizontalAxis(),  _joystick.GetVerticalAxis());
            Character.SetMovementDirection(input);
            WeaponsController.SetFlipped(ViewController.BodySprite.flipX);
        }

        private void Update()
        {
            HandleInput();
        }
    }
}