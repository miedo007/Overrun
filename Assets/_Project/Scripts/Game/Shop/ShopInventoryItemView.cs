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

        private int _cost;
        
        public BaseData Data { get; private set; }

        private HeroInfo _heroInfo;

        private void Awake()
        {
            BuyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        public void Initialize(BaseData data, HeroInfo hero, int cost)
        {
            _cost = cost;
            _heroInfo = hero;
            Data = data;
            Icon.sprite = data.Sprite;
            NameField.text = data.DisplayName;
            DescriptionField.text = data.GetDescriptionForHero(hero);
            CostText.text = cost.ToString();

            hero.ShopCurrencyChanged += OnShopCurrencyChanged;
            OnShopCurrencyChanged();
        }

        private void OnDestroy()
        {
            if (_heroInfo != null)
            {
                _heroInfo.ShopCurrencyChanged -= OnShopCurrencyChanged;
            }
        }

        private void OnShopCurrencyChanged()
        {
            BuyButton.interactable = _cost <= _heroInfo.ShopCurrency;
        }

        private void OnBuyButtonClicked()
        {
            if (_heroInfo.ShopCurrency >= Data.BasePrice)
            {
                BuyButtonClicked?.Invoke(this);
            }
        }

        public void Purchase()
        {
            BuyButton.gameObject.SetActive(false);
            CanvasGroup.alpha = 0.25f;
            CanvasGroup.interactable = false;
        }

    }
}