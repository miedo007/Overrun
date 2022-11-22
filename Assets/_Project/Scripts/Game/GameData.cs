using Project.Application;
using UnityEngine;

namespace Project.Game
{
    [CreateAssetMenu(fileName = "data_game_", menuName = "Data/GameData", order = 0)]
    public class GameData : ScriptableObject
    {
        [field: SerializeField, Header("Shop")] public float ShopPriceIncreaseCoeffecient { get; private set; } = 1.4f;
        [field: SerializeField] public float ShopBasePriceMultiplier { get; private set; } = 1;
        [field: SerializeField] public float ResellValue { get; set; } = 0.33f;
        [field: SerializeField] public float BaseRerollCost { get; set; } = 3;
        [field: SerializeField] public float RerollIncreasePerWave { get; set; } = 5;
        [field: SerializeField] public float RerollCostCoefficient { get; set; } = 1.2f;
        [field: SerializeField] public AnimationCurve RarityCurve { get; private set; }
        [field: SerializeField, Header("Gameplay")] public float HealthRegenRate { get; private set; } = 2;
        [field: SerializeField] public int MaxContainersPerWave { get; set; } = 3;
        [field: SerializeField, Header("Enemies")] public float HealthScalingPerLevel { get; private set; } = 1.15f;
        [field: SerializeField] public float HealthScalingPerWave { get; private set; } = 1.2f;
        [field: SerializeField] public float DamageScalingPerLevel { get; private set; } = 1.3f;
        [field: SerializeField] public float DamageScalingPerWave { get; private set; } = 1.3f;
        [field: SerializeField, Header("Currency Reward")] public int RewardBaseValue { get; private set; } = 25;
        [field: SerializeField] public float RewardLevelScaling { get; private set; } = 1.25f;
        [field: SerializeField] public float RewardWaveScaling { get; private set; } = 1.25f;
        [field: SerializeField, Header("Upograde Costs")] public int UpgradeBaseCost { get; private set; } = 100;
        [field: SerializeField] public int UpgradeIncreasePerLevel { get; private set; } = 50;
        [field: SerializeField] public float UpgradeCostScaling { get; private set; } = 1.25f;
        
        public Vector2Int GetItemTierRangeForWave(int waveIndex)
        {
            return new Vector2Int(waveIndex - 15, Mathf.FloorToInt(waveIndex * 0.7f) + 1 ) ;
        }
        
        public int GetSellPrice(BaseData data, int waveIndex)
        {
            return Mathf.CeilToInt((data.BasePrice * ShopBasePriceMultiplier) 
                                   * Mathf.Pow(ShopPriceIncreaseCoeffecient, waveIndex) 
                                   * ResellValue);
        }

        public int GetRerollCost(int waveIndex, int rerollCount)
        {
            var cost = BaseRerollCost + (waveIndex * RerollIncreasePerWave);
            return Mathf.RoundToInt(cost * Mathf.Pow(RerollCostCoefficient, rerollCount));
        }

        public int GetScaledCost(BaseData data, int waveIndex)
        {
            return Mathf.CeilToInt((data.BasePrice * ShopBasePriceMultiplier) * Mathf.Pow(ShopPriceIncreaseCoeffecient, waveIndex));
        }

        public int GetCurrencyReward(int levelIndex, int waveIndex)
        {
            var scaled = Mathf.CeilToInt(RewardBaseValue * Mathf.Pow(RewardLevelScaling, levelIndex));
            scaled = Mathf.CeilToInt(scaled * Mathf.Pow(RewardWaveScaling, waveIndex));
            return scaled;
        }
        
        public int GetUpgradeCost(int levelIndex)
        {
            var baseCost = UpgradeBaseCost + (UpgradeIncreasePerLevel * levelIndex);
            var scaled = Mathf.CeilToInt(baseCost * Mathf.Pow(UpgradeCostScaling, levelIndex));
            return scaled;
        }
    }
}