using Mtl.Injection;
using Project.Game.Levels;
using UnityEngine;

namespace Project.Game.Rooms
{
    public class RoomViewController : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private Material levelMaterial;
        [SerializeField] private RoomData[] roomDatas;

        [Inject] private readonly LevelController _levelController;
        private static readonly int GroundColor = Shader.PropertyToID("_GroundColor");
        private static readonly int WallColor = Shader.PropertyToID("_WallColor");

        public void OnReady()
        {
            _levelController.Initialized += OnLevelControllerInitialized;
        }
        
        private void OnLevelControllerInitialized()
        {
            var roomData = _levelController.CurrentLevel.RoomData;
            if (roomData == null)
            {
                var roomDataIndex = _levelController.CurrentLevelIndex % roomDatas.Length;
                roomData = roomDatas[roomDataIndex];
            }
            
            levelMaterial.SetColor(GroundColor, roomData.GroundColor);
            levelMaterial.SetColor(WallColor, roomData.WallColor);
        }

        private void ColorSprites(SpriteRenderer[] spriteRenderers, Color color)
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = color;
            }
        }
    }
}