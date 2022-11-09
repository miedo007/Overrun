using System;
using DG.Tweening;
using Mtl.Injection;
using Project.Game.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class PlayerHealthMeter : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private Image meter;
        [SerializeField] private Image previousValueMeter;
        [SerializeField] private TextMeshProUGUI text;

        [Inject] private readonly PlayerHealthController _healthController;

        private Color _meterColor;

        public void OnReady()
        {
            _healthController.Changed += OnChanged;
            _healthController.Initialized += HealthControllerInitialized;

            _meterColor = meter.color;
        }

        private void Start()
        {
            RefreshImmediate();
        }

        private void OnEnable()
        {
            if (_healthController != null)
            {
                RefreshImmediate();
            }
        }

        private void RefreshImmediate()
        {
            meter.fillAmount = _healthController.CurrentPercentage;
        }

        private void HealthControllerInitialized()
        {
            OnChanged(1f,1f);
        }

        private void OnDestroy()
        {
            previousValueMeter.DOKill();
            meter.DOKill();

            if (_healthController != null)
            {
                _healthController.Changed -= OnChanged;
                _healthController.Initialized -= HealthControllerInitialized;
            }
        }

        private void OnChanged(float previousPercentage, float currentPercentage)
        {
            text.text = _healthController.GetHealthString();
            
            previousValueMeter.DOKill();
            meter.DOKill();
            
            meter.color = Color.white;
            meter.DOColor(_meterColor, 0.2f)
                .SetDelay(0.1f)
                .SetEase(Ease.OutQuad);
            
            previousValueMeter.fillAmount = previousPercentage;
            previousValueMeter.DOFillAmount(currentPercentage, 0.125f)
                .SetDelay(0.2f)
                .SetEase(Ease.OutQuad);
        
            meter.fillAmount = currentPercentage;
        }
    }
}