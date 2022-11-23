using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Tromagon.Runtime.Utils
{
    [PublicAPI]
    public class RoutineRunner : MonoBehaviour
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
        public event Action OnFixedUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
    }
}