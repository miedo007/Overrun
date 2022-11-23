using System.Collections.Generic;
using Project.Application;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Tiers
{
    [CreateAssetMenu(fileName = "tiered_group_", menuName = "Data/Tiers/TieredGroup", order = 0)]
    public class TieredDataGroup : ScriptableObject
    {
        [field: SerializeField] public List<TierInfo> Tiers { get; private set; } = new();
        [field: SerializeField] public int MaxCount { get; private set; } = -1;

        public TierInfo GetRandomTier(AnimationCurve rarityCurve, Vector2Int tierRange, float chanceIncrease)
        {
            var chance = Random.value + chanceIncrease;
            chance = rarityCurve.Evaluate(chance);
            var evaluatedIndex = Mathf.RoundToInt(Mathf.Lerp(tierRange.x, tierRange.y, chance));
            evaluatedIndex = Mathf.Clamp(evaluatedIndex, 0, Tiers.Count - 1);
            return Tiers[evaluatedIndex];
        }

        public bool CanUpgradeTier(BaseData baseData)
        {
            for (int i = 0; i < Tiers.Count - 1; i++)
            {
                if (Tiers[i].Data == baseData)
                {
                    return true;
                }
            }

            return false;
        }

        public TierInfo GetNextTier(BaseData baseData)
        {
            for (int i = 0; i < Tiers.Count - 1; i++)
            {
                if (Tiers[i].Data == baseData)
                {
                    return Tiers[i+1];
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class TierInfo
    {
        [field: SerializeField] public TierData Tier { get; set; }
        [field: SerializeField] public BaseData Data { get; set; }
    }

}