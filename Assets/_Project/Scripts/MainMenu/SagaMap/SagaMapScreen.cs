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
        private bool _isOpen = false;

        private void Awake()
        {
            sagaMapController.OnNodeSelected += OnNodeSelected;
        }

        private void OnDestroy()
        {
            sagaMapController.OnNodeSelected -= OnNodeSelected;
            
            if (_playerInfo != null)
            {
                _playerInfo.OnChanged -= RefreshSagaMap;
            }
        }

        private void OnNodeSelected(int nodeIndex)
        {
            _currentlySelectedLevel = nodeIndex;
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            _isOpen = true;
            
            _playerInfo.OnChanged += RefreshSagaMap;
            
            RefreshSagaMap();
        }
        
        protected override void OnClosed()
        {
            base.OnClosed();
            _isOpen = false;
            
            _playerInfo.OnChanged -= RefreshSagaMap;
        }
        
        private void RefreshSagaMap()
        {
            if (!_isOpen) return;
            
            var topStage = _playerInfo.PlayerSave.TopStageIndex;
            Debug.Log($"[SagaMapScreen] Refreshing saga map with TopStageIndex: {topStage}");
            sagaMapController.Init(topStage);
            OnNodeSelected(topStage);
        }

        public int GetSelectedLevel()
        {
            return _currentlySelectedLevel;
        }
    }
}