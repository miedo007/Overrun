using System;
using Mtl.Injection;
using Mtl.SagaMap;
using Mtl.UiFramework;
using Project.Application;
using Project.Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.MainMenu.SagaMap
{
    public class SagaMapScreen : UIScreen
    {
        [SerializeField] private SagaMapController sagaMapController;
        [SerializeField] private Image characterImage;
        
        [Inject] private readonly PlayerInfo _playerInfo;

        private int _currentlySelectedLevel = -1;

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
            _currentlySelectedLevel = nodeIndex;
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            var topStage = _playerInfo.PlayerSave.TopStageIndex;
            sagaMapController.Init(topStage);
            OnNodeSelected(topStage);
        }

        public int GetSelectedLevel()
        {
            return _currentlySelectedLevel;
        }
    }
}