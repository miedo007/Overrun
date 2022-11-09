using Project.Application;
using Project.Game.Items;
using Project.Game.UI;
using UnityEngine;

namespace Project.Game.Shop
{
    public class InfoViewItem : InfoViewBase
    {
        [SerializeField] private StatModifierView statModifierViewPrefab;
        [SerializeField] private RectTransform parent;

        public override void Initialize(BaseData data)
        {
            var itemData = data as ItemData;
            if (itemData == null)
            {
                Debug.LogError($"Data is not ItemData :: {data.name}");
                return;
            }
            
            foreach (var statModifier in itemData.StatModifiers)
            {
                var modifierView = Instantiate(statModifierViewPrefab, parent);
                modifierView.Initialize(statModifier);
            }
        }
    }
}