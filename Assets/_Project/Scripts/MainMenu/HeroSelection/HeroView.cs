using System;
using Mtl.Injection;
using Mtl.Save;
using Project.Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace Project.MainMenu.HeroSelection
{
    public class HeroView : MonoBehaviour, IInjectionReady
    {
        public event Action<HeroData> Selected;
        
        [SerializeField] private Button selectButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Image heroImage;

        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly SaveManager _saveManager;

        private HeroInfo _heroInfo;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }

        private void OnUpgradeButtonClicked()
        {
            _heroRegistry.UpgradeActiveHero();
            _saveManager.Save();
        }

        private void OnSelectButtonClicked()
        {
            _heroRegistry.SetSelectedHero(_heroInfo.Data.name);
            Selected?.Invoke(_heroInfo.Data);
        }

        public void OnReady()
        {
            _heroRegistry.ActiveHeroChanged += OnActiveHeroChanged;
            OnActiveHeroChanged(_heroRegistry.ActiveHero);
        }

        private void OnDestroy()
        {
            if (_heroRegistry != null)
            {
                _heroRegistry.ActiveHeroChanged -= OnActiveHeroChanged;
            }
        }

        private void OnActiveHeroChanged(HeroInfo heroInfo)
        {
            _heroInfo = heroInfo;
            heroImage.sprite = _heroInfo.Data.Sprite;
        }
    }
}