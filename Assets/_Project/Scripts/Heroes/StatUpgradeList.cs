using Mtl.Injection;
using Project.Game.UI;
using Tromagon.Extensions;
using UnityEngine;

namespace Project.Heroes
{
    public class StatUpgradeList : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private StatDeltaView statViewPrefab;
        [SerializeField] private RectTransform parent;
        
        [Inject] private HeroRegistry _heroRegistry;

        private HeroInfo _currentHeroInfo;
        private HeroInfo _nextLevelHeroInfo;

        private void Initialize()
        {
            parent.RemoveAllChildren();

            _nextLevelHeroInfo = _heroRegistry.GetHeroInfo(_currentHeroInfo.Data, _currentHeroInfo.Level + 1);

            if (_currentHeroInfo == null)
            {
                return;
            }
            
            foreach (var stat in _currentHeroInfo.Stats)
            {
                var nextLevelStat = _nextLevelHeroInfo.GetStat(stat.Data);
                var statView = Instantiate(statViewPrefab, parent);
                statView.Initialize(stat, nextLevelStat);
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
            _currentHeroInfo = heroInfo;
            Initialize();
        }
    }
}