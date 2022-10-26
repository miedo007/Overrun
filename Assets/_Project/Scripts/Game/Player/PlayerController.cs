using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public PlayerCharacter Character { get; private set; }

        private void HandleInput()
        {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"),  Input.GetAxisRaw("Vertical"));
            Character.SetMovementDirection(input.normalized);
        }

        private void Update()
        {
            HandleInput();
        }
    }
}