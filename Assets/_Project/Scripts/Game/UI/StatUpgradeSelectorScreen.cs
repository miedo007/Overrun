using System.Collections.Generic;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.Items;
using Project.Game.Levels;
using Project.Heroes;
using Project.Tiers;
using UnityEngine;

namespace Project.Game.UI
{
    public class StatUpgradeSelectorScreen : UIScreen, IInjectionReady
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private TieredGroupDatabase upgradeDatabase;
        [SerializeField] private List<StatUpgradeView> upgradeViews;

        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly LevelController _levelController;
        
        private  HeroInfo _heroInfo;

        private void Awake()
        {
            foreach (var upgradeView in upgradeViews)
            {
                upgradeView.Selected += OnUpgradeSelected;
            }
        }

        private void OnDestroy()
        {
            foreach (var upgradeView in upgradeViews)
            {
                upgradeView.Selected -= OnUpgradeSelected;
            }
        }

        private void OnUpgradeSelected(ItemData upgradeData)
        {
            _heroInfo.AddItem(upgradeData);
            Close();
        }

        public void Initialize()
        {
            // choose three random unique stat upgrade groups
            // get random tier for each group
            // display choice selectors
            var upgradeSelection = new List<TieredDataGroup>();
            for (int i = 0; i < upgradeViews.Count; i++)
            {
                var randomStatUpgradeGroup = upgradeDatabase.GetRandom();
                while (upgradeSelection.Contains(randomStatUpgradeGroup))
                {
                    randomStatUpgradeGroup = upgradeDatabase.GetRandom();
                }
                upgradeSelection.Add(randomStatUpgradeGroup);
            }

            var tierRange = _gameData.GetItemTierRangeForWave(_levelController.CurrentWaveIndex);
            for (var i = 0; i < upgradeSelection.Count; i++)
            {
                var tieredDataGroup = upgradeSelection[i];
                var upgradeTierInfo = tieredDataGroup.GetRandomTier(_gameData.RarityCurve, tierRange.x, tierRange.y);
                var upgrade = upgradeTierInfo.Data as ItemData;

                upgradeViews[i].Initialize(upgrade);
            }
        }

        public void OnReady()
        {
            _heroInfo = _heroRegistry.ActiveHero;
        }
    }
}