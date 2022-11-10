using System.Collections.Generic;
using Project.Application;
using UnityEngine;

namespace Project.Tiers
{
    [CreateAssetMenu(fileName = "tiered_group_", menuName = "Data/Tiers/TieredGroup", order = 0)]
    public class TieredDataGroup : ScriptableObject
    {
        [field: SerializeField] public List<TierInfo> Tiers { get; private set; } = new();

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

            minTier = Mathf.Clamp(minTier, 0, Tiers.Count);
            maxTier = Mathf.Clamp(maxTier, minTier, Tiers.Count);
            
            
            if (rarityCurve == null)
            {
                return Tiers[Random.Range(minTier, maxTier)];
            }
            
            curvedValue = rarityCurve.Evaluate(curvedValue);
            var lerpedIndex = Mathf.RoundToInt(Mathf.Lerp((float) minTier, (float) maxTier, curvedValue));
            return Tiers[lerpedIndex];
        }
    }

    [System.Serializable]
    public class TierInfo
    {
        [field: SerializeField] public TierData Tier { get; set; }
        [field: SerializeField] public BaseData Data { get; set; }
    }

}