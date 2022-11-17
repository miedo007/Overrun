using Mtl.Injection;
using Project.Heroes;
using Tromagon.Extensions;
using UnityEngine;

namespace Project.Game.UI
{
    public class StatsView : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private StatView statViewPrefab;
        [SerializeField] private RectTransform parent;
        
        [Inject] private HeroesInfo _heroesInfo;

        private HeroInfo _heroInfo;

        private void Initialize()
        {
            parent.RemoveAllChildren();

            if (_heroInfo == null)
            {
                return;
            }
            
            foreach (var stat in _heroInfo.Stats)
            {
                var statView = Instantiate(statViewPrefab, parent);
                statView.Initialize(stat);
                statView.gameObject.SetActive(true);
            }
        }

        public void OnReady()
        {
            _heroesInfo.ActiveHeroChanged += OnActiveHeroChanged;
        }

        private void OnDestroy()
        {
            _heroesInfo.ActiveHeroChanged -= OnActiveHeroChanged;
            OnActiveHeroChanged(_heroesInfo.ActiveHero);
        }

        private void OnActiveHeroChanged(HeroInfo heroInfo)
        {
            _heroInfo = heroInfo;
            Debug.Log("Active Hero Changed");
            Debug.Log(_heroInfo.Data.name);
            Initialize();
        }
    }
}