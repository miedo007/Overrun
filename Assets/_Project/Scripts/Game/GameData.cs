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
    }
}