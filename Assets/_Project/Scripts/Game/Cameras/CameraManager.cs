using Cinemachine;
using Mtl.Injection;
using Project.Game.Player;
using UnityEngine;

namespace Project.Game.Cameras
{
    public class CameraManager : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public Camera Camera { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera GameplayCam { get; private set; }
        
        [Inject] private readonly PlayerController _player;

        private Vector3 _velocity;
        
        public void OnReady()
        {
            GameplayCam.m_Follow = _player.transform;
        }
    }
}