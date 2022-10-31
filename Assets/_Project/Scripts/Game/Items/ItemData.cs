using System.Collections.Generic;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "data_item_", menuName = "Data/Items/ItemData", order = 0)]
    public class ItemData : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; } = "DISPLAY_NAME";
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public List<StatModifier> StatModifiers { get; private set; }
    }
}