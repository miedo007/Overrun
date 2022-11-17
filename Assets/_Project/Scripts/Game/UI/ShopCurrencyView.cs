using Mtl.Injection;
using Project.Heroes;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class ShopCurrencyView : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public TextMeshProUGUI CurrencyText { get; private set; }
        
        [Inject] private readonly HeroRegistry _heroRegistry;
        
        public void OnReady()
        {
            _heroRegistry.ActiveHero.ShopCurrencyChanged += OnShopCurrencyChanged;
        }

        private void OnDestroy()
        {
            if (_heroRegistry.ActiveHero != null)
            {
                _heroRegistry.ActiveHero.ShopCurrencyChanged -= OnShopCurrencyChanged;
            }
        }

        private void OnShopCurrencyChanged()
        {
            CurrencyText.text = _heroRegistry.ActiveHero.GetShopCurrencyIntValue().ToString();
        }
    }
}