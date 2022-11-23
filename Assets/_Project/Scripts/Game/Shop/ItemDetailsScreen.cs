using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Items;
using Project.Game.Levels;
using Project.Game.Weapons;
using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ItemDetailsScreen : UIScreen, IPointerClickHandler
    {
        [SerializeField] private ShopInventoryItemView itemView;
        [SerializeField] private Button mergeButton;
        [SerializeField] private Button sellButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI sellText;

        [Inject] private readonly GameData _gameData;
        [Inject] private readonly LevelController _levelController;

        private HeroInfo _heroInfo;
        private string _sellLabel;
        private BaseData _data;

        private void Awake()
        {
            _sellLabel = sellText.text;
            closeButton.onClick.AddListener(Close);
            sellButton.onClick.AddListener(OnSellButtonClicked);
            mergeButton.onClick.AddListener(OnMergeButtonClicked);
        }

        public void Initialize(BaseData baseData, HeroInfo activeHero)
        {
            _heroInfo = activeHero;
            _data = baseData;
            
            sellText.text = string.Format(_sellLabel, _gameData.GetSellPrice(_data, _levelController.WaveIndex));

            var isWeapon = _data as WeaponData != null;
            mergeButton.gameObject.SetActive(isWeapon);
            sellButton.gameObject.SetActive(isWeapon);
            
            if (isWeapon)
            {
                mergeButton.interactable = _heroInfo.CanMergeWeapon(_data as WeaponData);
                sellButton.interactable = _heroInfo.CurrentWeapons.Count > 1;
            }
            
            itemView.Initialize(baseData, _heroInfo, -1);
        }

        private void OnSellButtonClicked()
        {
            _heroInfo.ShopCurrency += _gameData.GetSellPrice(_data, _levelController.WaveIndex);
            
            var weaponData = _data as WeaponData;
            if (weaponData != null)
            {
                SellWeapon(weaponData);
                Close();
                return;
            }
            
            var itemData = _data as ItemData;
            if (itemData != null)
            {
                SellItem(itemData);
                Close();
            }
        }

        private void OnMergeButtonClicked()
        {
            _heroInfo.MergeWeapon(_data as WeaponData);
            Close();
        }

        private void SellItem(ItemData itemData)
        {
            _heroInfo.RemoveItem(itemData);
        }

        private void SellWeapon(WeaponData weaponData)
        {
            _heroInfo.RemoveWeapon(weaponData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Close();
        }
    }
}