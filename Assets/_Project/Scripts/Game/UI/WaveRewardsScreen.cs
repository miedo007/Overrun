using System.Collections;
using DG.Tweening;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Items;
using Project.Game.Levels;
using Project.Game.Shop;
using Project.Heroes;
using Project.Tiers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class WaveRewardsScreen : UIScreen
    {
        [SerializeField] private ShopInventoryItemView itemDetailsView;
        [SerializeField] private RectTransform itemDetailsRoot;
        [SerializeField] private GameObject containerView;
        [SerializeField] private CanvasGroup backer;
        [SerializeField] private RectTransform headerRect;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Button _keepButton;
        [SerializeField] private RectTransform _buttonGroup;
        [SerializeField] private TextMeshProUGUI _sellText;

        [Inject] private readonly GameData _gameData;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly LevelController _levelController;
        [Inject] private readonly PlayerInfo _playerInfo;
        [Inject("items")] private readonly TieredGroupDatabase _itemDatabase;

        private BaseData _currentItem;
        private string _sellLabel;
        private int _waveIndex;

        private void Awake()
        {
            _sellLabel = _sellText.text;
            _sellButton.onClick.AddListener(OnSellButtonClicked);
            _keepButton.onClick.AddListener(OnKeepButtonClicked);
            _keepButton.interactable = true;
        }

        private void OnSellButtonClicked()
        {
            _heroRegistry.ActiveHero.ShopCurrency += GetSellValue(_currentItem);
            StartCoroutine(CloseDetailsRoutine());
        }

        private void OnKeepButtonClicked()
        {
            _heroRegistry.ActiveHero.AddItem(_currentItem as ItemData);
            StartCoroutine(CloseDetailsRoutine());
        }

        public void Initialize(int waveIndex)
        {
            _waveIndex = waveIndex;

            StartCoroutine(OpenRoutine());
        }

        private IEnumerator OpenRoutine()
        {
            backer.alpha = 0;
            headerRect.localScale = Vector3.zero;
            itemDetailsRoot.localScale = Vector3.zero;
            _buttonGroup.localScale = Vector3.zero;
            
            
            yield return backer.DOFade(1, 0.125f).WaitForCompletion();
            yield return headerRect.DOScale(1, 0.125f).WaitForCompletion();
            
            yield return StartCoroutine(RewardRoutine());

        }
        private IEnumerator RewardRoutine()
        {
            var tierRange = _gameData.GetItemTierRangeForWave(_waveIndex);

            containerView.SetActive(true);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            
            _heroRegistry.ActiveHero.WaveRewards--;
            containerView.SetActive(false);

            var chanceIncrease = _gameData.GetChanceIncreaseForWave(_waveIndex);

            _currentItem = _itemDatabase.GetRandom()
                .GetRandomTier(_gameData.RarityCurve,
                    tierRange,
                    chanceIncrease)
                .Data as ItemData;
                
            itemDetailsView.Initialize(_currentItem, _heroRegistry.ActiveHero, -1);
            _sellText.text = string.Format(_sellLabel, GetSellValue(_currentItem));

            yield return itemDetailsRoot.DOScale(1, 0.12f).WaitForCompletion();
            yield return _buttonGroup.DOScale(1, 0.125f).WaitForCompletion();
        }

        private IEnumerator CloseDetailsRoutine()
        {
            itemDetailsRoot.DOScale(0, 0.12f);
            yield return _buttonGroup.DOScale(0, 0.125f).WaitForCompletion();
            
            if (_heroRegistry.ActiveHero.WaveRewards > 0)
            {
                StartCoroutine(RewardRoutine());
            }
            else
            {
                StartCoroutine(CloseRoutine());
            }
        }

        private IEnumerator CloseRoutine() 
        {
            backer.DOFade(0, 0.125f);
            yield return headerRect.DOScale(0, 0.125f).WaitForCompletion();
            Close();
        }

        private int GetSellValue(BaseData data)
        {
            var waveIndex = _waveIndex;
            return _gameData.GetSellPrice(data, waveIndex);
        }
    }
}