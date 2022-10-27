using System;
using Lean.Pool;
using TMPro;
using UnityEngine;

namespace Project.PopupText
{
    public class PopupTextItem : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Text { get; private set; }
        public Vector3 TargetPosition { get; private set; }
        public float SpawnTime { get; private set; }

        public bool IsReadyForRemoval()
        {
            if (_currentTime >= _duration)
            {
                LeanPool.Despawn(this);
                return true;
            }

            return false;
        }

        private Transform _transform;
        private Vector3 _spawnPosition;
        private float _currentTime;
        private float _duration;

        private void Awake()
        {
            _transform = transform;
        }

        public void Initialize(string text, Color color, Vector3 targetPosition, float duration)
        {
            _currentTime = 0;
            _spawnPosition = _transform.position;
            Text.text = text;
            Text.color = color;
            TargetPosition = targetPosition;
            SpawnTime = Time.time;
            _duration = duration;
        }

        public void Step(float dt)
        {
            _currentTime += dt;
            _transform.position = Vector3.Lerp(_spawnPosition, TargetPosition, _currentTime / _duration);
        }
    }
}