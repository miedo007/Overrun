using System;
using Mtl.Injection;
using Mtl.Toolbox;
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

        private Material _levelMaterialInstance;

        private void Awake()
        {
            _levelMaterialInstance = new Material(levelMaterial);
        }

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
            
            using var renderers = ListPool.Get<Renderer>();
            GetComponentsInChildren(renderers);

            foreach (var r in renderers)
            {
                if (r.sharedMaterial == levelMaterial)
                {
                    r.sharedMaterial = _levelMaterialInstance;
                }
            }
            
            _levelMaterialInstance.SetColor(GroundColor, roomData.GroundColor);
            _levelMaterialInstance.SetColor(WallColor, roomData.WallColor);
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