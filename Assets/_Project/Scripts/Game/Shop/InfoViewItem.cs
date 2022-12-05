using Mtl.Toolbox;
using Project.Application;
using Project.Game.Items;
using Project.Game.UI;
using TMPro;
using UnityEngine;

namespace Project.Game.Shop
{
    public class InfoViewItem : InfoViewBase
    {
        [SerializeField] private StatModifierView statModifierViewPrefab;
        [SerializeField] private ItemBehaviourView itemBehaviourViewPrefab;
        [SerializeField] private bool showDescription;
        [SerializeField] private TextMeshProUGUI descriptionPrefab;
        [SerializeField] private RectTransform parent;

        public override void Initialize(BaseData data)
        {
            parent.RemoveAllChildren();
            
            var itemData = data as ItemData;
            if (itemData == null)
            {
                Debug.LogError($"Data is not ItemData :: {data.name}");
                return;
            }

            if (itemData.ShowDescription && showDescription)
            {
                var descriptionTextField = Instantiate(descriptionPrefab, parent);
                descriptionTextField.text = itemData.Description;
            }
            
            foreach (var statModifier in itemData.StatModifiers)
            {
                var modifierView = Instantiate(statModifierViewPrefab, parent);
                modifierView.Initialize(statModifier);
            }
            
            foreach (var behaviourPair in itemData.Behaviours)
            {
                var behaviourView = Instantiate(itemBehaviourViewPrefab, parent);
                behaviourView.Initialize(behaviourPair);
            }
        }
    }
}