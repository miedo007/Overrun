using System;
using UnityEngine;

namespace Project.Game.Targets
{
    public class Target : MonoBehaviour
    {
        public static event Action<Target> Activated;
        public static event Action<Target> Deactivated;

        private bool _activated = false;
        private Transform _transform;

        public bool IsActivated => _activated;
        public Vector3 Position => _transform.position;

        private void Awake()
        {
            _transform = transform;
        }

        public void Activate()
        {
            if (_activated)
            {
                return;
            }

            _activated = true;
            Activated?.Invoke(this);
        }
        
        public void Deactivate()
        {
            if (!_activated)
            {
                return;
            }

            _activated = false;
            Deactivated?.Invoke(this);
        }
    }
}