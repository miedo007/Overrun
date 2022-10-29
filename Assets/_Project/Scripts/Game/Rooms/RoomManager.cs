using UnityEngine;

namespace Project.Game.Rooms
{
    public class RoomManager : MonoBehaviour
    {
        [field: SerializeField] public Rect Rect { get; private set; }
        
        public Vector3 GetValidPositionInRadius(Vector3 referencePosition, float radius)
        {
            var randomPosition = referencePosition + ((Vector3)Random.insideUnitCircle * radius);
            return ClampToRoomRect(randomPosition);
        }

        private Vector3 ClampToRoomRect(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, Rect.xMin, Rect.xMax);
            position.y = Mathf.Clamp(position.y, Rect.yMin, Rect.yMax);
            return position;
        }
        
        public Vector3 ClampToRoomRect(Vector3 position, out bool didClampX, out bool didClampY)
        {
            didClampX = false;
            didClampY = false;
            
            if (position.x < Rect.xMin)
            {
                position.x = Rect.xMin;
                didClampX = true;
            }
            else if (position.x > Rect.xMax)
            {
                position.x = Rect.xMax;
                didClampX = true;
            }
            
            if (position.y < Rect.yMin)
            {
                position.y = Rect.yMin;
                didClampY = true;
            }
            else if (position.y > Rect.yMax)
            {
                position.y = Rect.yMax;
                didClampY = true;
            }
            
            return position;
        }
    }
}