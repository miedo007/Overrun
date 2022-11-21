using System;
using Project.Game.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class StatUpgradeView : MonoBehaviour
    {
        public event Action<ItemData> Selected;
        
        [SerializeField] private Image icon;
        [SerializeField] private Image tierBacker;
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private TextMeshProUGUI descriptionField;
        [SerializeField] private Button selectButton;
        
        private ItemData _data;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            Selected?.Invoke(_data);
        }

        public void Initialize(ItemData data)
        {
            _data = data;
            icon.sprite = _data.Sprite;
            tierBacker.color = _data.Tier.Color;

            var statModifier = data.StatModifiers[0];
            nameField.text = $"{_data.DisplayName} {_data.Tier.NamePostfix}";
            descriptionField.text = string.Format(_data.Description, statModifier.GetDisplayValue());
        }
    }
}