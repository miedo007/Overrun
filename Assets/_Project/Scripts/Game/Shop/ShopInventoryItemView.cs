using System;
using DG.Tweening;
using Project.Application;
using Project.Game.Items;
using Project.Game.Weapons;
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
        [field: SerializeField] public InfoViewWeapon WeaponInfoView;
        [field: SerializeField] public InfoViewItem ItemInfoView;
        [field: SerializeField] public  Button BuyButton { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI CostText { get; private set; }
        [field: SerializeField] public  LayoutElement LayoutElement { get; private set; }

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

            var isWeapon = data as WeaponData != null;
            
            WeaponInfoView.gameObject.SetActive(isWeapon);
            ItemInfoView.gameObject.SetActive(!isWeapon);
            
            if (isWeapon)
            {
                WeaponInfoView.Initialize(data);
            }
            else
            {
                ItemInfoView.Initialize(data);
            }
            
            CostText.text = $"<sprite name=currency_ticket> {cost}";
            if (cost < 0)
            {
                BuyButton.gameObject.SetActive(false);
            }
            
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
            CanvasGroup.interactable = false;
            CanvasGroup.DOFade(0, 0.125f)
                .OnComplete(ScaleDown);
        }

        private void ScaleDown()
        {
            LayoutElement.DOPreferredSize(Vector2.zero, 0.1f)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}