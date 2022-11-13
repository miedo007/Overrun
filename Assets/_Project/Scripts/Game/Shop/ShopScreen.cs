using Mtl.Injection;
using Mtl.UiFramework;
using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopScreen : UIScreen
    {
        [field: SerializeField] public Button NextWaveButton { get; private set; }
        [field: SerializeField] public Button RerollButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI RerollButtonText { get; private set; }
        [field: SerializeField] public ShopInventoryView ShopInventory { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ShopCurrencyText { get; private set; }

        [Inject] private readonly HeroInfo _heroInfo;
        [Inject] private readonly GameData _gameData;


        private int _waveIndex;
        private int _rerollCount;
        private int _rerollCost;
        private string _rerollButtonLabel;

        private void Awake()
        {
            _rerollButtonLabel = RerollButtonText.text;
            NextWaveButton.onClick.AddListener(OnNextWaveButtonClicked);
            RerollButton.onClick.AddListener(OnRerollButtonClicked);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            
            _heroInfo.ShopCurrencyChanged += OnShopCurrencyChanged;
            OnShopCurrencyChanged();
        }
        
        protected override void OnClosed()
        {
            _heroInfo.ShopCurrencyChanged -= OnShopCurrencyChanged;
        }

        public void Initialize(int waveIndex)
        {
            _waveIndex = waveIndex;
            _rerollCount = 0;
            UpdateRerollCost();
            
            ShopInventory.Populate(waveIndex: waveIndex);
        }

        private void UpdateRerollCost()
        {
            _rerollCost = _gameData.GetRerollCost(_waveIndex, _rerollCount);
            RerollButtonText.text = string.Format(_rerollButtonLabel, _rerollCost);
            RerollButton.interactable = _heroInfo.ShopCurrency >= _rerollCost;
        }

        private void OnNextWaveButtonClicked()
        {
            Close();
        }

        private void OnRerollButtonClicked()
        {
            _rerollCount++;
            UpdateRerollCost();
            _heroInfo.ShopCurrency -= _rerollCost;
            ShopInventory.Populate(_waveIndex);
        }

        private void OnShopCurrencyChanged()
        {
            RerollButton.interactable = _heroInfo.ShopCurrency >= _rerollCost;
            ShopCurrencyText.text = $"{_heroInfo.ShopCurrency}";
        }
    }
}