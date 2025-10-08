using System.Collections.Generic;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Levels;
using Project.Game.Shop;
using Project.Heroes;
using Project.Tiers;
using UnityEngine;

namespace Project.Game.UI
{
    public class RandomItemSelectorScreen : UIScreen
    {
        [SerializeField] private ShopInventoryItemView[] itemViews;

        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly LevelController _levelController;

        private void Awake()
        {
            foreach (var itemView in itemViews)
            {
                itemView.BuyButtonClicked += OnBuyButtonClicked;
                itemView.RewardedAdBuyClicked += OnPremiumBuyButtonClicked; // Handle premium ad purchases
            }
        }

        private void OnDestroy()
        {
            foreach (var itemView in itemViews)
            {
                itemView.BuyButtonClicked -= OnBuyButtonClicked;
                itemView.RewardedAdBuyClicked -= OnPremiumBuyButtonClicked;
            }
        }

        private void OnBuyButtonClicked(ShopInventoryItemView itemView)
        {
            _heroRegistry.ActiveHero.AddToInventoryFromBaseData(itemView.Data);
            Close();
        }

        private void OnPremiumBuyButtonClicked(ShopInventoryItemView itemView)
        {
            // Same result as regular purchase - just obtained through ad
            _heroRegistry.ActiveHero.AddToInventoryFromBaseData(itemView.Data);
            Close();
        }

        public void Initialize(TieredGroupDatabase database)
        {
            var itemSelection = new List<TieredDataGroup>();
            for (var i = 0; i < itemViews.Length; i++)
            {
                var randomStatUpgradeGroup = database.GetRandom();
                while (itemSelection.Contains(randomStatUpgradeGroup))
                {
                    randomStatUpgradeGroup = database.GetRandom();
                }
                
                itemSelection.Add(randomStatUpgradeGroup);
            }

            // Randomly select one weapon to be premium (tier 1, requires ad)
            var premiumWeaponIndex = Random.Range(0, itemSelection.Count);
            Debug.Log($"[RandomItemSelectorScreen] Premium weapon will be at index: {premiumWeaponIndex}");

            var waveIndex = _levelController.WaveIndex;
            var tierRange = _gameData.GetItemTierRangeForWave(waveIndex);
            var chanceIncrease = _gameData.GetChanceIncreaseForWave(waveIndex);
            
            for (var i = 0; i < itemSelection.Count; i++)
            {
                var tieredDataGroup = itemSelection[i];
                bool isPremium = (i == premiumWeaponIndex);
                
                var upgradeTierInfo = isPremium 
                    ? GetPremiumWeapon(tieredDataGroup)
                    : tieredDataGroup.GetRandomTier(_gameData.RarityCurve, tierRange, chanceIncrease);
                
                var upgrade = upgradeTierInfo.Data as BaseData;
                
                if (isPremium)
                {
                    // Premium weapon: Set up for ad purchase (cost = 0, mark as ad-exclusive)
                    itemViews[i].Initialize(upgrade, _heroRegistry.ActiveHero, 0); // No custom text, cost = 0
                    itemViews[i].SetRewardedAdAvailability(true);
                    itemViews[i].SetAdExclusive(true); // Mark as ad-exclusive for new system
                    Debug.Log($"[RandomItemSelectorScreen] ⭐ Premium weapon configured: {upgrade.DisplayName} - Tier 1, Ad-exclusive");
                }
                else
                {
                    // Regular weapon: Free selection with custom "CHOOSE" text
                    itemViews[i].Initialize(upgrade, _heroRegistry.ActiveHero, 0, "CHOOSE");
                    itemViews[i].SetRewardedAdAvailability(false);
                    itemViews[i].SetAdExclusive(false); // Regular weapon, not ad-exclusive
                    Debug.Log($"[RandomItemSelectorScreen] 🔫 Regular weapon configured: {upgrade.DisplayName} - Tier 0, Shows 'CHOOSE' text");
                }
            }
        }

        private TierInfo GetPremiumWeapon(TieredDataGroup tieredDataGroup)
        {
            // Premium weapons are always tier 1 (guaranteed better than normal tier 0)
            var premiumTierRange = new Vector2Int(1, 1);
            
            // Clamp tier range to available tiers in the group to prevent errors
            var maxTierIndex = tieredDataGroup.Tiers.Count - 1;
            premiumTierRange.x = Mathf.Clamp(premiumTierRange.x, 0, maxTierIndex);
            premiumTierRange.y = Mathf.Clamp(premiumTierRange.y, 0, maxTierIndex);
            
            // High chance increase to ensure tier 1 selection
            var premiumChanceIncrease = 0.9f; // 90% chance bias toward higher tier
            
            var weaponTierInfo = tieredDataGroup.GetRandomTier(_gameData.RarityCurve, premiumTierRange, premiumChanceIncrease);
            Debug.Log($"[RandomItemSelectorScreen] ⭐ Premium weapon selected: {weaponTierInfo.Data.name} (Tier range: {premiumTierRange.x}-{premiumTierRange.y}, Max available: {maxTierIndex})");
            
            return weaponTierInfo;
        }
    }
}