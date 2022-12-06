using System;
using Cinemachine;
using Mtl.Injection;
using Project.Game.Levels;
using Project.Game.Player;
using UnityEngine;

namespace Project.Game.Cameras
{
    public class CameraManager : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public Camera Camera { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera GameplayCam { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera WaveIntroCam { get; private set; }
        [field: SerializeField] public BoxCollider2D CameraBounds { get; private set; }
        [field: SerializeField] public Vector2 CameraBoundsPadding { get; private set; }
        [field: SerializeField] public Vector2 CameraBoundsOffset { get; private set; }
        [field: SerializeField] public CinemachineConfiner2D Confiner { get; private set; }
        
        [Inject] private readonly PlayerController _player;
        [Inject] private readonly LevelController _levelController;

        private Vector3 _velocity;
        
        public void OnReady()
        {
            GameplayCam.m_Follow = _player.transform;
            ZoomIn();
            _levelController.Initialized += OnLevelControllerInitialized;
        }

        private void OnLevelControllerInitialized()
        {
            var levelData = _levelController.CurrentLevel;

            var cameraBoundsSize = levelData.RoomSize + CameraBoundsPadding;
            CameraBounds.size = cameraBoundsSize;
            CameraBounds.offset += CameraBoundsOffset;
            Confiner.InvalidateCache();
        }

        public void ZoomOut()
        {
            WaveIntroCam.Priority = 0;
            GameplayCam.Priority = 100;
        }

        public void ZoomIn()
        {
            WaveIntroCam.Priority = 100;
            GameplayCam.Priority = 0;
        }
    }
}