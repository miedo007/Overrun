using Project.Application;
using UnityEngine;

namespace Project.Game
{
    [CreateAssetMenu(fileName = "data_game_", menuName = "Data/GameData", order = 0)]
    public class GameData : ScriptableObject
    {
        [field: SerializeField] public float ShopPriceIncreaseCoeffecient { get; private set; } = 1.4f;
        [field: SerializeField] public float ShopBasePriceMultiplier { get; private set; } = 1;
        [field: SerializeField] public float HealthRegenRate { get; private set; } = 2;
        [field: SerializeField] public float ResellValue { get; set; } = 0.33f;
        [field: SerializeField] public int MaxContainersPerWave { get; set; } = 3;
        [field: SerializeField] public AnimationCurve RarityCurve { get; private set; }

        public Vector2Int GetItemTierRange(int waveIndex)
        {
            return new Vector2Int(waveIndex - 10, Mathf.FloorToInt(waveIndex * 0.45f) + 1 ) ;
        }

        public int GetSellPrice(BaseData data, int waveIndex)
        {
            return Mathf.CeilToInt((data.BasePrice * ShopBasePriceMultiplier) 
                                   * Mathf.Pow(ShopPriceIncreaseCoeffecient, waveIndex) 
                                   * ResellValue);
        }

        public int GetScaledCost(BaseData data, int waveIndex)
        {
            return Mathf.CeilToInt((data.BasePrice * ShopBasePriceMultiplier) * Mathf.Pow(ShopPriceIncreaseCoeffecient, waveIndex));
        }
    }
}