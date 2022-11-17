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
        
        [Inject] private HeroRegistry _heroRegistry;

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
            _heroRegistry.ActiveHeroChanged += OnActiveHeroChanged;
            OnActiveHeroChanged(_heroRegistry.ActiveHero);
        }

        private void OnDestroy()
        {
            _heroRegistry.ActiveHeroChanged -= OnActiveHeroChanged;
        }

        private void OnActiveHeroChanged(HeroInfo heroInfo)
        {
            _heroInfo = heroInfo;
            Initialize();
        }
    }
}