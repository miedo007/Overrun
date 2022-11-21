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
    }
}