using UnityEngine;

namespace Mtl.UI
{
    public class RotateOverTime : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _axis = Vector3.forward;

        [SerializeField]
        private float _speed = 1;

        [SerializeField]
        private bool _useUnscaledTime;

        private void Update()
        {
            transform.Rotate(_axis, _speed * (_useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));
        }
    }
}