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

        public TierInfo GetRandomTier(AnimationCurve rarityCurve = null, int minTier = -1, int maxTier = -1)
        {
            var curvedValue = Random.value;

            if (minTier < 0 && maxTier < 0)
            {
                if (rarityCurve == null)
                {
                    return Tiers[Random.Range(0, Tiers.Count)];
                }
                else
                {
                    curvedValue = rarityCurve.Evaluate(curvedValue);
                    var randomIndex = Mathf.RoundToInt(curvedValue * Tiers.Count);
                    return Tiers[randomIndex];
                }
            }

            if (rarityCurve == null)
            {
                var randomTier = Random.Range(minTier, maxTier);
                randomTier = Mathf.Clamp(randomTier, 0, Tiers.Count - 1);
                return Tiers[randomTier];
            }
            
            curvedValue = rarityCurve.Evaluate(curvedValue);
            var evaluatedIindex = Mathf.RoundToInt(Mathf.Lerp((float) minTier, (float) maxTier, curvedValue));
            evaluatedIindex = Mathf.Clamp(evaluatedIindex, 0, Tiers.Count - 1);
            return Tiers[evaluatedIindex];
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