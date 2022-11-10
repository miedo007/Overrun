using System.Collections.Generic;
using Project.Tiers;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "database_items", menuName = "Data/Items/ItemDatabase", order = 0)]
    public class ItemDatabase : ScriptableObject
    {
        [field: SerializeField] public List<ItemData> Items { get; private set; }
        [field: SerializeField] public List<TieredDataGroup> ItemGroups { get; private set; }

        public TieredDataGroup GetRandomGroup()
        {
            return ItemGroups[Random.Range(0, ItemGroups.Count)];
        }
        
        public ItemData GetRandom()
        {
            return Items[Random.Range(0, Items.Count)];
        }
    }
}