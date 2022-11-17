using System;
using System.Collections.Generic;
using Mtl.Injection;
using Project.Heroes;
using UnityEngine;

namespace Project.MainMenu.HeroSelection
{
    public class HeroSelectionView : MonoBehaviour
    {
        public Action<HeroData> HeroSelected;

        [SerializeField] private RectTransform parent;
        [SerializeField] private HeroSelectionItemView heroViewPrefab;

        [Inject] private readonly HeroRegistry _heroRegistry;

        private List<HeroSelectionItemView> _heroViews = new();

        public void Start()
        {
            foreach (var hero in _heroRegistry.Database.Heroes)
            {
                var heroView = Instantiate(heroViewPrefab, parent);
                heroView.Initialize(hero);
                heroView.Selected += OnHeroViewSelected;
                _heroViews.Add(heroView);
            }

            _heroRegistry.SetActiveHero(_heroRegistry.Database.DefaultHero);
        }

        private void OnHeroViewSelected(HeroData data)
        {
            HeroSelected?.Invoke(data);
            _heroRegistry.SetActiveHero(data);
        }
    }
}