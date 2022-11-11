using Mtl.Injection;
using Project.Heroes;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class ShopCurrencyView : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public TextMeshProUGUI CurrencyText { get; private set; }
        
        [Inject] private readonly HeroInfo _heroInfo;
        
        public void OnReady()
        {
            _heroInfo.ShopCurrencyChanged += OnShopCurrencyChanged;
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
            CurrencyText.text = _heroInfo.GetShopCurrencyIntValue().ToString();
        }
    }
}