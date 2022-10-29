using Mtl.Injection;
using Project.Game.Rooms;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerCharacter : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public float MaxSpeed { get; private set; }
        [field: SerializeField] public float Acceleration { get; private set; }
        [field: SerializeField] public float Drag { get; private set; }

        [Inject] private readonly RoomManager _roomManager;

        public Vector2 Velocity { get; private set; }
        public float MovementSpeed { get; private set; }
        public int HorizontalDirection { get; private set; } = 1;

        public void SetMovementDirection(Vector2 direction)
        {
            var inputMagnitude = direction.magnitude;
            
            if (Mathf.Approximately(inputMagnitude, 0))
            {
                // Decelerate
                Velocity *= 1 - (Drag * Time.deltaTime);
            }
            else
            {
                Velocity += direction * Acceleration * Time.deltaTime;
            }

            var horizontalDirection = direction.x > 0 ? 1 : direction.x < 0 ? -1 : 0;
            if (horizontalDirection != 0)
            {
                HorizontalDirection = horizontalDirection;
            }
            
            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed * inputMagnitude);
            MovementSpeed = Velocity.magnitude;

            var newPosition = Rigidbody.position;
            newPosition += Velocity * Time.deltaTime;

            var newVelocity = Velocity;
            
            newPosition = _roomManager.ClampToRoomRect(
                newPosition, 
                out var didClampX, 
                out var didClampY
                );
            
            if (didClampX)
            {
                newVelocity.x = 0;
            }

            if (didClampY)
            {
                newVelocity.y = 0;
            }

            Velocity = newVelocity;
            Rigidbody.position = newPosition;
        }
    }
}