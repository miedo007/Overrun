using System.Collections.Generic;
using Project.Application;
using UnityEngine;

namespace Project.Tiers
{
    [CreateAssetMenu(fileName = "tiered_group_", menuName = "Data/TieredGroup", order = 0)]
    public class TieredDataGroup : ScriptableObject
    {
        [field: SerializeField] public List<TierInfo> Tiers { get; private set; } = new();
    }

    [System.Serializable]
    public class TierInfo
    {
        [field: SerializeField] public TierData Tier { get; set; }
        [field: SerializeField] public BaseData Data { get; set; }
    }

}