using System;
using Mtl.Injection;
using Project.Application;
using Project.Game.UI;
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

        private HeroInfo _heroInfo;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
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