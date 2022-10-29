using DG.Tweening;
using Mtl.Injection;
using Project.Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class PlayerHealthMeter : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private Image meter;
        [SerializeField] private Image previousValueMeter;

        [Inject] private readonly PlayerHealthController _healthController;

        private Color _meterColor;

        public void OnReady()
        {
            _healthController.Changed += HealthControllerOnChanged;
            _healthController.Initialized += HealthControllerInitialized;

            _meterColor = meter.color;
        }

        private void Start()
        {
            HealthControllerOnChanged(1f,1f);
        }

        private void HealthControllerInitialized()
        {
            HealthControllerOnChanged(1f,1f);
        }

        private void OnDestroy()
        {
            previousValueMeter.DOKill();
            meter.DOKill();
            _healthController.Changed -= HealthControllerOnChanged;
            _healthController.Initialized -= HealthControllerInitialized;
        }

        private void HealthControllerOnChanged(float previousPercentage, float currentPercentage)
        {
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