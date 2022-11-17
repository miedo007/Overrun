using System.Collections.Generic;
using Mtl.Injection;
using Project.Heroes;
using UnityEngine;

namespace Project.MainMenu.HeroSelection
{
    public class HeroSelectionView : MonoBehaviour
    {
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
                _heroViews.Add(heroView);
            }

            _heroRegistry.SetActiveHero(_heroRegistry.Database.DefaultHero);
        }
    }
}