using DG.Tweening;
using Mtl.Injection;
using Project.Application;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class SoftCurrencyView : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private RectTransform iconRect;

        [Inject] private readonly PlayerInfo _playerInfo;
        
        public void OnReady()
        {
            _playerInfo.OnCurrencyChanged += OnCurrencyChanged;
            OnCurrencyChanged();
        }

        private void OnDestroy()
        {
            _playerInfo.OnCurrencyChanged -= OnCurrencyChanged;
        }

        private void OnCurrencyChanged()
        {
            iconRect.DOKill();
            iconRect.localScale = Vector3.one * 1.5f;
            iconRect.DOScale(1, 0.12f);
            currencyText.text = _playerInfo.PlayerSave.Currency.ToString();
        }
    }
}