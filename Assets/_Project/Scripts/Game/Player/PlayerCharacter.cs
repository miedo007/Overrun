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

        public void SetMovementDirection(Vector2 direction)
        {
            if (Mathf.Approximately(direction.sqrMagnitude, 0))
            {
                // Decelerate
                Velocity *= 1 - (Drag * Time.deltaTime);
            }
            else
            {
                Velocity += direction * Acceleration * Time.deltaTime;
            }

            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);
            Rigidbody.position += Velocity * Time.deltaTime;
        }
    }
}