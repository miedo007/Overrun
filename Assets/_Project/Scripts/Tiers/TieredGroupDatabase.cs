using System.Collections.Generic;
using UnityEngine;

namespace Project.Tiers
{
    [CreateAssetMenu(fileName = "database_tiered_groups_", menuName = "Data/Tiers/TieredGroupGDatabase", order = 0)]
    public class TieredGroupDatabase : ScriptableObject
    {
        [field: SerializeField] public List<TieredDataGroup> TieredGroups { get; private set; }
        

        public TieredDataGroup GetRandom()
        {
            return TieredGroups[Random.Range(0, TieredGroups.Count)];
        }
        
    }
}