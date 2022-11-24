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
        [SerializeField] private Button sellButton;
        [SerializeField] private Button keepButton;
        [SerializeField] private RectTransform buttonGroup;
        [SerializeField] private TextMeshProUGUI sellText;

        [Inject] private readonly GameData _gameData;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject("items")] private readonly TieredGroupDatabase _itemDatabase;

        private BaseData _currentItem;
        private string _sellLabel;
        private int _waveIndex;

        private void Awake()
        {
            _sellLabel = sellText.text;
            sellButton.onClick.AddListener(OnSellButtonClicked);
            keepButton.onClick.AddListener(OnKeepButtonClicked);
            keepButton.interactable = true;
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
            buttonGroup.localScale = Vector3.zero;
            
            
            yield return backer.DOFade(1, 0.125f).WaitForCompletion();
            yield return StartCoroutine(RewardRoutine());

        }
        
        private IEnumerator RewardRoutine()
        {
            var tierRange = _gameData.GetItemTierRangeForWave(_waveIndex);
            
            yield return headerRect.DOScale(1, 0.12f).WaitForCompletion();
            
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
            sellText.text = string.Format(_sellLabel, GetSellValue(_currentItem));

            headerRect.DOScale(0, 0.125f);
            yield return itemDetailsRoot.DOScale(1, 0.12f).WaitForCompletion();
            yield return buttonGroup.DOScale(1, 0.125f).WaitForCompletion();
        }

        private IEnumerator CloseDetailsRoutine()
        {
            itemDetailsRoot.DOScale(0, 0.12f);
            yield return buttonGroup.DOScale(0, 0.125f).WaitForCompletion();
            
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