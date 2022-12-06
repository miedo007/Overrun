using System;
using DG.Tweening;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Tutorials;
using Project.Game.Weapons;
using Project.Heroes;
using Project.Tiers;
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
        [field: SerializeField] public  Image IconBacker { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI NameField { get; private set; }
        [field: SerializeField] public InfoViewWeapon WeaponInfoView;
        [field: SerializeField] public InfoViewItem ItemInfoView;
        [field: SerializeField] public  Button BuyButton { get; private set; }
        [field: SerializeField] public  TextMeshProUGUI CostText { get; private set; }
        [field: SerializeField] public  LayoutElement LayoutElement { get; private set; }
        [field: SerializeField] public  MergeableNotification MergeableNotification { get; private set; }

        [Inject] private readonly TierDatabase _tierDatabase;
        [Inject] private readonly UIFrame _uiFrame;

        private bool _affordable;
        private bool _purchased;
        private int _cost;
        
        public BaseData Data { get; private set; }

        private HeroInfo _heroInfo;

        private void Awake()
        {
            if (BuyButton != null)
            {
                BuyButton.onClick.AddListener(OnBuyButtonClicked);
            }
        }

        public void Initialize(BaseData data, HeroInfo hero, int cost, string buyButtonString = "")
        {
            if (MergeableNotification != null)
            {
                MergeableNotification.Deactivate();
            }
            
            _cost = cost;
            _heroInfo = hero;
            Data = data;
            Icon.sprite = data.Sprite;
            IconBacker.color = data.Tier.Color;
            NameField.text = $"{data.DisplayName} {data.Tier.NamePostfix}";

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

            if (BuyButton != null)
            {
                hero.ShopCurrencyChanged += OnShopCurrencyChanged;
                hero.WeaponsChanged += OnWeaponsChanged;
                OnShopCurrencyChanged();
                
                if (cost < 0)
                {
                    BuyButton.gameObject.SetActive(false);
                }
                else if (string.IsNullOrEmpty(buyButtonString))
                {
                    CostText.text = $"<sprite name=currency_ticket> {cost}";
                }
                else
                {
                    CostText.text = buyButtonString;
                }
            }
            
            RefreshMergeableNotification();
        }
        private void OnDestroy()
        {
            if (_heroInfo != null)
            {
                _heroInfo.ShopCurrencyChanged -= OnShopCurrencyChanged;
                _heroInfo.WeaponsChanged -= OnWeaponsChanged;
            }
        }

        private void OnShopCurrencyChanged()
        {
            _affordable = _cost <= _heroInfo.GetShopCurrencyIntValue();
            BuyButton.interactable = _affordable;
            RefreshMergeableNotification();
        }

        private void OnWeaponsChanged()
        {
            RefreshMergeableNotification();
        }

        private void RefreshMergeableNotification()
        {
            if (MergeableNotification == null)
            {
                return;
            }

            if (!_affordable || _purchased)
            {
                MergeableNotification.Deactivate();
                return;
            }

            var weaponData = Data as WeaponData;
            if (weaponData == null)
            {
                MergeableNotification.Deactivate();
                return;
            }
            
            if (!_heroInfo.HasFreeWeaponSlot() && _heroInfo.CanMergeWeapon(weaponData, 1))
            {
                MergeableNotification.Activate(_tierDatabase.GetNextTier(weaponData.Tier).Color);
                if (!PrefKeys.HasCompletedAutoMergeTutorial())
                {
                    _uiFrame.Open<AutoMergeTutorialScreen>();
                }
            }
            else
            {
                MergeableNotification.Deactivate();
            }
        }

        private void OnBuyButtonClicked()
        {
            if (_heroInfo.GetShopCurrencyIntValue() >= _cost)
            {
                BuyButtonClicked?.Invoke(this);
            }
        }

        public void Purchase()
        {
            _purchased = true;
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