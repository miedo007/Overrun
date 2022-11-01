using Mtl.UiFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class ShopScreen : UIScreen
    {
        [field: SerializeField] public Button NextWaveButton { get; private set; }

        private void Awake()
        {
            NextWaveButton.onClick.AddListener(OnNextWaveButtonClicked);
        }

        private void OnNextWaveButtonClicked()
        {
            Close();
        }
    }
}