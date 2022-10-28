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

        [SerializeField] private Vector2 gameplayCamOffset;
        [SerializeField] private float smoothTime = 0.2f;

        [Inject] private readonly PlayerController _player;

        private Vector3 _velocity;

        /*
        private void LateUpdate()
        {
            if (_player == null)
            {
                return;
            }
            
            var targetPosition = _player.Position;
            var camPosition = GameplayCam.transform.position;
            targetPosition.z = camPosition.z;
            GameplayCam.transform.position = Vector3.SmoothDamp(camPosition, targetPosition, ref _velocity, smoothTime);
        }*/
        public void OnReady()
        {
            GameplayCam.m_Follow = _player.transform;
        }
    }
}