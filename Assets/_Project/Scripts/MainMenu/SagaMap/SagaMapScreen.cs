using System;
using Mtl.SagaMap;
using Mtl.UiFramework;
using UnityEngine;

namespace Project.Scripts.MainMenu.SagaMap
{
    public class SagaMapScreen : UIScreen
    {
        public event Action<int> LevelSelected;
        
        [SerializeField] private SagaMapController sagaMapController;

        private void Awake()
        {
            sagaMapController.OnNodeSelected += OnNodeSelected;
        }

        private void OnDestroy()
        {
            sagaMapController.OnNodeSelected -= OnNodeSelected;
        }

        private void OnNodeSelected(int nodeIndex)
        {
            LevelSelected?.Invoke(nodeIndex);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            sagaMapController.Init(0);
        }
    }
}