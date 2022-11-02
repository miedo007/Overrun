using Mtl.UiFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopScreen : UIScreen
    {
        [field: SerializeField] public Button NextWaveButton { get; private set; }
        [field: SerializeField] public Button RerollButton { get; private set; }
        [field: SerializeField] public ShopInventoryView ShopInventory { get; private set; }

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
        
    }
}