using System.Collections.Generic;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "database_items", menuName = "Data/Items/ItemDatabase", order = 0)]
    public class ItemDatabase : ScriptableObject
    {
        [field: SerializeField] public List<ItemData> Items { get; private set; }

        public ItemData GetRandom()
        {
            return Items[Random.Range(0, Items.Count)];
        }
    }
}