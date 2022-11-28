using System;
using System.Collections.Generic;
using System.Linq;
using Project.Application;
using Project.Game.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public TieredDataGroup GetGroupForBaseData(BaseData baseData)
        {
            return TieredGroups.FirstOrDefault(x =>
            {
                foreach (var tier in x.Tiers)
                {
                    if (tier.Data == baseData)
                    {
                        return true;
                    }
                }
                
                return false;
            });

        }

        public BaseData GetItemWithId(string id)
        {
            foreach (var tieredGroup in TieredGroups)
            {
                foreach (var tier in tieredGroup.Tiers)
                {
                    if (string.Equals(tier.Data.Id, id))
                    {
                        return tier.Data;
                    }
                }
            }

            return null;
        }
    }
}