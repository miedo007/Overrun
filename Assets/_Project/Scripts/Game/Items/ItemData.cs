using System.Collections.Generic;
using Project.Application;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "data_item_", menuName = "Data/Items/ItemData", order = 0)]
    public class ItemData : BaseData
    {
        [field: SerializeField] public List<StatModifier> StatModifiers { get; private set; }
    }
}