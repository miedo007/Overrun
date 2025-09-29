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
            // Ensure all upgrade views are properly reset before setup
            ResetAllUpgradeViews();
            
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
            
            // Randomly select one upgrade to be premium (higher tier, requires ad)
            var premiumUpgradeIndex = Random.Range(0, upgradeSelection.Count);
            Debug.Log($"[StatUpgradeSelectorScreen] Premium upgrade will be at index: {premiumUpgradeIndex}");
            
            for (var i = 0; i < upgradeSelection.Count; i++)
            {
                var tieredDataGroup = upgradeSelection[i];
                bool isPremium = (i == premiumUpgradeIndex);
                
                var upgradeTierInfo = isPremium 
                    ? GetPremiumUpgrade(tieredDataGroup, waveIndex)
                    : tieredDataGroup.GetRandomTier(_gameData.RarityCurve, tierRange, chanceIncrease);
                
                var upgrade = upgradeTierInfo.Data as ItemData;
                upgradeViews[i].Initialize(upgrade, isPremium);
            }
        }

        private void ResetAllUpgradeViews()
        {
            Debug.Log("[StatUpgradeSelectorScreen] Resetting all upgrade views to ensure clean state");
            // This ensures that all upgrade views start with clean button states
            // Helps prevent the bug where buttons remain disabled from previous waves
            foreach (var upgradeView in upgradeViews)
            {
                // Force reset button states to prevent carryover issues from previous waves
                upgradeView.ForceResetButtonStates();
            }
        }

        private TierInfo GetPremiumUpgrade(TieredDataGroup tieredDataGroup, int waveIndex)
        {
            // Premium upgrades get better tier ranges based on wave progression
            Vector2Int premiumTierRange;
            
            if (waveIndex <= 3)
            {
                // Early waves: Tier 1 guaranteed
                premiumTierRange = new Vector2Int(1, 1);
                Debug.Log("[StatUpgradeSelectorScreen] Early waves - Premium upgrade: Tier 1");
            }
            else if (waveIndex <= 10)
            {
                // Mid waves: Tier 2-3 range  
                premiumTierRange = new Vector2Int(2, 3);
                Debug.Log("[StatUpgradeSelectorScreen] Mid waves - Premium upgrade: Tier 2-3");
            }
            else
            {
                // Late waves: Tier 4 guaranteed
                premiumTierRange = new Vector2Int(4, 4);
                Debug.Log("[StatUpgradeSelectorScreen] Late waves - Premium upgrade: Tier 4");
            }
            
            // Clamp tier range to available tiers in the group to prevent errors
            var maxTierIndex = tieredDataGroup.Tiers.Count - 1;
            premiumTierRange.x = Mathf.Clamp(premiumTierRange.x, 0, maxTierIndex);
            premiumTierRange.y = Mathf.Clamp(premiumTierRange.y, 0, maxTierIndex);
            
            // High chance increase for premium upgrades
            var premiumChanceIncrease = 0.8f; // 80% chance bias toward higher tier
            
            var upgradeTierInfo = tieredDataGroup.GetRandomTier(_gameData.RarityCurve, premiumTierRange, premiumChanceIncrease);
            Debug.Log($"[StatUpgradeSelectorScreen] ⭐ Premium upgrade selected: {upgradeTierInfo.Data.name} (Tier range: {premiumTierRange.x}-{premiumTierRange.y}, Max available: {maxTierIndex})");
            
            return upgradeTierInfo;
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