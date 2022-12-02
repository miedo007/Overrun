using DG.Tweening;
using Mtl.Injection;
using Project.Heroes;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class ShopCurrencyView : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public TextMeshProUGUI CurrencyText { get; private set; }
        [field: SerializeField] public RectTransform IconRect { get; private set; }
        
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
            IconRect.DOKill();
            IconRect.localScale = Vector3.one * 1.5f;
            IconRect.DOScale(1, 0.12f);
            CurrencyText.text = _heroRegistry.ActiveHero.GetShopCurrencyIntValue().ToString();
        }
    }
}