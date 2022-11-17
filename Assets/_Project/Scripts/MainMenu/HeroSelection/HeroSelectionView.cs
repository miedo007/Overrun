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

        [Inject] private readonly HeroesInfo _heroesInfo;

        private List<HeroSelectionItemView> _heroViews = new();

        public void Start()
        {
            foreach (var hero in _heroesInfo.Database.Heroes)
            {
                var heroView = Instantiate(heroViewPrefab, parent);
                heroView.Initialize(hero);
                _heroViews.Add(heroView);
            }

            _heroesInfo.SetActiveHero(_heroesInfo.Database.DefaultHero);
        }
    }
}