using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.Levels;
using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopScreen : UIScreen
    {
        [field: SerializeField] public Button NextWaveButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI NextWaveText { get; private set; }
        [field: SerializeField] public Button RerollButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI RerollButtonText { get; private set; }
        [field: SerializeField] public ShopInventoryView ShopInventory { get; private set; }

        [Inject] private readonly HeroInfo _heroInfo;
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly LevelController _levelController;


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

        public void Initialize()
        {
            _waveIndex = _levelController.CurrentWaveIndex;
            if (_levelController.IsFinalWave)
            {
                NextWaveText.text = "Begin\nFinal Wave!";
            }
            else
            {
                NextWaveText.text = $"Begin\nWave {_waveIndex + 1}!";
            }

            _rerollCount = 0;
            UpdateRerollCost();
            
            ShopInventory.Populate(waveIndex: _waveIndex);
        }

        private void UpdateRerollCost()
        {
            _rerollCost = _gameData.GetRerollCost(_waveIndex, _rerollCount);
            RerollButtonText.text = string.Format(_rerollButtonLabel, _rerollCost);
            OnShopCurrencyChanged();
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
            RerollButton.interactable = _heroInfo.GetShopCurrencyIntValue() >= _rerollCost;
        }
    }
}