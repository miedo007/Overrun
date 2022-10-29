using System;
using Mtl.Injection;
using Project.Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class PlayerHealthMeter : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private Image meter;

        [Inject] private readonly PlayerHealthController _healthController;


        public void OnReady()
        {
            _healthController.Changed += HealthControllerOnChanged;
            _healthController.Initialized += HealthControllerInitialized;
        }

        private void Start()
        {
            HealthControllerOnChanged(0);
        }

        private void HealthControllerInitialized()
        {
            HealthControllerOnChanged(0);
        }

        private void HealthControllerOnChanged(float delta)
        {
            meter.fillAmount = _healthController.CurrentHealth / _healthController.MaxHealth;
        }
    }
}