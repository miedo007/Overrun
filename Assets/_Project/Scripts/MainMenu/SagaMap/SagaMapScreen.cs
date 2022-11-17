using System;
using Mtl.Injection;
using Mtl.SagaMap;
using Mtl.UiFramework;
using Project.Application;
using Project.Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MainMenu.SagaMap
{
    public class SagaMapScreen : UIScreen
    {
        public event Action<int> LevelSelected;
        
        [SerializeField] private SagaMapController sagaMapController;
        [SerializeField] private Image characterImage;
        
        [Inject] private readonly PlayerInfo _playerInfo;

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
            sagaMapController.Init(_playerInfo.PlayerSave.TopStageIndex);
        }
    }
}