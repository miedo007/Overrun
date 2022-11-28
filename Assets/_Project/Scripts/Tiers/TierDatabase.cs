using System.Collections.Generic;
using UnityEngine;

namespace Project.Tiers
{
    [CreateAssetMenu(fileName = "tiers_list", menuName = "Data/Tiers/TiersList", order = 0)]
    public class TierDatabase : ScriptableObject
    {
        [field: SerializeField] public List<TierData> Tiers { get; private set; }

        public TierData GetNextTier(TierData fromTier)
        {
            var nextTierIndex = Tiers.IndexOf(fromTier) + 1;
            if (nextTierIndex >= Tiers.Count)
            {
                return null;
            }

            return Tiers[nextTierIndex];
        }
    }
}