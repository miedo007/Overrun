using System;
using Project.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopInventoryItemView : MonoBehaviour
    {
        [field: SerializeField] public  Image Icon { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI NameField { get; private set; }

        public void Initialize(BaseData data)
        {
            Icon.sprite = data.Sprite;
            NameField.text = data.DisplayName;
            OnInitialize(data);
        }

        protected virtual void OnInitialize(BaseData data)
        {
        }
    }
}