using UnityEngine;

namespace Project.Game.Rooms
{
    public class RoomWall : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer edge;
        [SerializeField] private SpriteRenderer wall;

        public void Initialize(Vector3 position, float length)
        {
            transform.position = position;
            
            var edgeSize = edge.size;
            edgeSize.x = length;
            edge.size = edgeSize;
            
            var wallSize = wall.size;
            wallSize.x = length + wallSize.y * 2;
            wall.size = wallSize;
        }
    }
}