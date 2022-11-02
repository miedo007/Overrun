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
        
        [field: SerializeField] public  CanvasGroup CanvasGroup { get; private set; }
        [field: SerializeField] public  Image Icon { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI NameField { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI DescriptionField { get; private set; }
        [field: SerializeField] public  Button BuyButton { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI CostText { get; private set; }
        
        public BaseData Data { get; private set; }

        private HeroInfo _heroInfo;

        public void Initialize(BaseData data, HeroInfo hero)
        {
            _heroInfo = hero;
            Data = data;
            Icon.sprite = data.Sprite;
            NameField.text = data.DisplayName;
            DescriptionField.text = data.GetDescriptionForHero(hero);
            CostText.text = data.BasePrice.ToString();
            BuyButton.onClick.AddListener(OnBuyButtonClicked);

            hero.ShopCurrencyChanged += OnShopCurrencyChanged;
            OnShopCurrencyChanged();
        }

        private void OnShopCurrencyChanged()
        {
            BuyButton.interactable = Data.BasePrice <= _heroInfo.ShopCurrency;
        }

        private void OnBuyButtonClicked()
        {
            if (_heroInfo.ShopCurrency >= Data.BasePrice)
            {
                CanvasGroup.alpha = 0.25f;
                CanvasGroup.interactable = false;
                BuyButton.gameObject.SetActive(false);
                BuyButtonClicked?.Invoke(this);
            }
        }

        protected virtual void OnInitialize(BaseData data)
        {
        }
    }
}