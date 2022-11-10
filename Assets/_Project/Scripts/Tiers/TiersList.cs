using System.Collections.Generic;
using UnityEngine;

namespace Project.Tiers
{
    [CreateAssetMenu(fileName = "tiers_list", menuName = "Data/Tiers/TiersList", order = 0)]
    public class TiersList : ScriptableObject
    {
        [field: SerializeField] public List<TierData> Tiers { get; private set; }
    }
}