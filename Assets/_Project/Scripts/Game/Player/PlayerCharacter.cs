using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerCharacter : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public float MaxSpeed { get; private set; }
        [field: SerializeField] public float Acceleration { get; private set; }
        [field: SerializeField] public float Drag { get; private set; }
        
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
            if (newPosition.x < -7.5f)
            {
                newPosition.x = -7.5f;
                newVelocity.x = 0;
            }
            else if (newPosition.x > 7.5f)
            {
                newPosition.x = 7.5f;
                newVelocity.x = 0;
            }
            
            if (newPosition.y < -8f)
            {
                newPosition.y = -8f;
                newVelocity.y = 0;
            }
            else if (newPosition.y > 7.5f)
            {
                newPosition.y = 7.5f;
                newVelocity.y = 0;
            }

            Velocity = newVelocity;
            Rigidbody.position = newPosition;
        }
    }
}