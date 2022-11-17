using System.Collections.Generic;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
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

        private void Awake()
        {
            foreach (var itemView in itemViews)
            {
                itemView.BuyButtonClicked += OnBuyButtonClicked;
            }
        }

        private void OnBuyButtonClicked(ShopInventoryItemView itemView)
        {
            Debug.Log("Buy Button Clicked");
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

            var maxTier = 3;
            for (var i = 0; i < itemSelection.Count; i++)
            {
                var tieredDataGroup = itemSelection[i];
                var upgradeTierInfo = tieredDataGroup.GetRandomTier(_gameData.RarityCurve, maxTier - 5, maxTier);
                var upgrade = upgradeTierInfo.Data as BaseData;
                itemViews[i].Initialize(upgrade, _heroRegistry.ActiveHero, 0, "CHOOSE");
            }
        }
    }
}