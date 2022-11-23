using System;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.Levels;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class HudScreen : UIScreen, IInjectionReady
    {
        [field: SerializeField] public TextMeshProUGUI WaveIndexText { get; private set; }

        [Inject] private readonly LevelController _levelController;

        private void Awake()
        {
            WaveIndexText.enabled = false;
        }

        public void OnReady()
        {
            _levelController.WaveStarted += OnWaveStarted;
            _levelController.WaveCompleted += OnWaveCompleted;
        }

        private void OnWaveStarted()
        {
            WaveIndexText.enabled = true;
            WaveIndexText.text = $"WAVE {_levelController.WaveIndex + 1}";
        }

        private void OnWaveCompleted()
        {
            WaveIndexText.enabled = false;
        }
    }
}