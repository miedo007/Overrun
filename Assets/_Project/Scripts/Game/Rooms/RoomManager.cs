using Mtl.Injection;
using Project.Game.Levels;
using UnityEngine;

namespace Project.Game.Rooms
{
    public class RoomManager : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private RoomWall wallNorth;
        [SerializeField] private RoomWall wallEast;
        [SerializeField] private RoomWall wallSouth;
        [SerializeField] private RoomWall wallWest;
        [SerializeField] private SpriteRenderer floor;
        
        [field: SerializeField] public Rect Rect { get; private set; }
        
        [field: SerializeField] public Transform CameraBounds { get; private set; }

        [Inject] private readonly LevelController _levelController;
        

        public void OnReady()
        {
            _levelController.Initialized += OnLevelControllerInitialized;
        }

        private void OnLevelControllerInitialized()
        {
            var levelData = _levelController.CurrentLevel;
            var rect = new Rect(-levelData.RoomSize * 0.5f, levelData.RoomSize);
            Rect = rect;
            
            floor.size = levelData.RoomSize + new Vector2(1,1);
            wallNorth.Initialize(new Vector3(0, rect.yMax + 0.5f), rect.width);
            wallSouth.Initialize(new Vector3(0, rect.yMin - 0.5f), rect.width);
            wallEast.Initialize(new Vector3(rect.xMax + 0.5f, 0), rect.height);
            wallWest.Initialize(new Vector3(rect.xMin - 0.5f, 0), rect.height);
        }

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

        public Vector3 GetRandomPosition()
        {
            return new Vector3(Random.Range(Rect.xMin, Rect.xMax), Random.Range(Rect.yMin, Rect.yMax));
        }

        public bool WillClamp(Vector3 position)
        {
            if (position.x < Rect.xMin)
            {
                position.x = Rect.xMin;
                return true;
            }
            
            if (position.x > Rect.xMax)
            {
                position.x = Rect.xMax;
                return true;
            }
            
            if (position.y < Rect.yMin)
            {
                position.y = Rect.yMin;
                return true;
            }
            
            if (position.y > Rect.yMax)
            {
                position.y = Rect.yMax;
                return true;
            }

            return false;
        }
    }
}