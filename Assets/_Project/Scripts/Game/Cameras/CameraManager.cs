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
        
        [Inject] private readonly PlayerController _player;

        private Vector3 _velocity;
        
        public void OnReady()
        {
            GameplayCam.m_Follow = _player.transform;
            ZoomIn();
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