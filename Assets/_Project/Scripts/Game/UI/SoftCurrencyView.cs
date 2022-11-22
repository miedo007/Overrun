using Mtl.Injection;
using Project.Application;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class SoftCurrencyView : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private TextMeshProUGUI currencyText;

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
            currencyText.text = _playerInfo.PlayerSave.Currency.ToString();
        }
    }
}