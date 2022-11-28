using System;
using System.Collections.Generic;
using Lean.Pool;
using Mtl.Injection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.PopupText
{
    [RequireComponent(typeof(AutoInjector))]
    public class PopupTextManager : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private PopupTextItem textItemPrefab;
        [SerializeField] private Vector3[] offsets;
        [SerializeField] private float displayDuration = 0.375f;
        
        [Inject] private readonly Camera _camera;

        private Quaternion _rotation;
        private Transform _transform;
        
        private readonly List<PopupTextItem> _activeTextItems = new List<PopupTextItem>();
        
        [field:SerializeField] public PopupTextItem DamagePrefab { get; set; }
        [field:SerializeField] public PopupTextItem CritDamagePrefab { get; set; }
        [field:SerializeField] public PopupTextItem PlayerDamagePrefab { get; set; }


        private void Awake()
        {
            _transform = transform;
        }

        public void OnReady()
        {
            var forward = _camera.transform.forward;
            forward.z *= -1;
            _rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
        
        public void DisplayTextAtPosition(string text, Vector3 position, PopupTextItem textPrefab = null)
        {
            var targetPosition = position + offsets[Random.Range(0, offsets.Length)];
            var textItem = LeanPool.Spawn(textPrefab == null ? textItemPrefab : textPrefab, position, _rotation, _transform);
            textItem.Initialize(text, targetPosition);
            _activeTextItems.Add(textItem);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            foreach (var activeTextItem in _activeTextItems)
            {
                activeTextItem.Step(dt);
            }
            
            _activeTextItems.RemoveAll(x => x.IsReadyForRemoval());
        }
    }
}