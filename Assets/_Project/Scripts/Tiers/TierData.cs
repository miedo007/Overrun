using Project.Application;
using UnityEngine;

namespace Project.Tiers
{
    [CreateAssetMenu(fileName = "data_tier_", menuName = "Data/Tiers/TierData", order = 0)]
    public class TierData : ScriptableObject
    {
        [field: SerializeField] public string NamePostfix = "I";
        [field: SerializeField] public Color Color;
    }
}