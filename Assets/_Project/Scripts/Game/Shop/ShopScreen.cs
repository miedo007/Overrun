using System;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopScreen : UIScreen, IInjectionReady
    {
        [field: SerializeField] public Button NextWaveButton { get; private set; }
        [field: SerializeField] public Button RerollButton { get; private set; }
        [field: SerializeField] public ShopInventoryView ShopInventory { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ShopCurrencyText { get; private set; }

        [Inject] private readonly HeroInfo _heroInfo;

        private void Awake()
        {
            NextWaveButton.onClick.AddListener(OnNextWaveButtonClicked);
            RerollButton.onClick.AddListener(OnRerollButtonClicked);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            ShopInventory.Populate();
        }

        private void OnNextWaveButtonClicked()
        {
            Close();
        }

        private void OnRerollButtonClicked()
        {
            ShopInventory.Populate();
        }

        public void OnReady()
        {
            _heroInfo.ShopCurrencyChanged += OnShopCurrencyChanged;
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
            ShopCurrencyText.text = _heroInfo.ShopCurrency.ToString();
        }
    }
}