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
        [SerializeField] private List<StatUpgradeView> upgradeViews;

        [Inject("stat_upgrades")] private  readonly TieredGroupDatabase _upgradeDatabase;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly LevelController _levelController;

        private readonly Dictionary<TieredDataGroup, int> _groupCountDict = new();

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
                var randomStatUpgradeGroup = _upgradeDatabase.GetRandom();

                var validSelectionMade = false;
                while (!validSelectionMade)
                {
                    randomStatUpgradeGroup = _upgradeDatabase.GetRandom();
                    if (DoesNotExceedMaxCount(randomStatUpgradeGroup) && !upgradeSelection.Contains(randomStatUpgradeGroup))
                    {
                        upgradeSelection.Add(randomStatUpgradeGroup);
                        IncreaseCount(randomStatUpgradeGroup);
                        validSelectionMade = true;
                    }
                }
            }
            
            var waveIndex = _levelController.WaveIndex;
            var tierRange = _gameData.GetItemTierRangeForWave(waveIndex);
            var chanceIncrease = _gameData.GetChanceIncreaseForWave(waveIndex);
            for (var i = 0; i < upgradeSelection.Count; i++)
            {
                var tieredDataGroup = upgradeSelection[i];
                var upgradeTierInfo = tieredDataGroup.GetRandomTier(_gameData.RarityCurve, tierRange, chanceIncrease);
                var upgrade = upgradeTierInfo.Data as ItemData;

                upgradeViews[i].Initialize(upgrade);
            }
        }

        private void IncreaseCount(TieredDataGroup group)
        {
            if (group.MaxCount <= 0)
            {
                return;
            }

            if (!_groupCountDict.ContainsKey(group))
            {
                _groupCountDict.Add(group, 0);
            }

            _groupCountDict[group]++;
        }

        private bool DoesNotExceedMaxCount(TieredDataGroup group)
        {
            if (group.MaxCount <= 0)
            {
                return true;
            }

            if (!_groupCountDict.ContainsKey(group))
            {
                return true;
            }

            return _groupCountDict[group] < group.MaxCount;
        }

        public void OnReady()
        {
            _heroInfo = _heroRegistry.ActiveHero;
        }
    }
}