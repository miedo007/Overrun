using System;
using Project.Application;
using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopInventoryItemView : MonoBehaviour
    {
        public event Action<ShopInventoryItemView> BuyButtonClicked;
        
        [field: SerializeField] public  Image Icon { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI NameField { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI DescriptionField { get; private set; }
        [field: SerializeField] public  Button BuyButton { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI CostText { get; private set; }
        
        public BaseData Data { get; private set; }

        public void Initialize(BaseData data, HeroInfo hero)
        {
            Data = data;
            Icon.sprite = data.Sprite;
            NameField.text = data.DisplayName;
            DescriptionField.text = data.GetDescriptionForHero(hero);
            
            BuyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        private void OnBuyButtonClicked()
        {
            BuyButtonClicked?.Invoke(this);
        }

        protected virtual void OnInitialize(BaseData data)
        {
        }
    }
}