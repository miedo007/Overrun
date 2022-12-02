using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class HudScreen : UIScreen, IInjectionReady
    {
        [field: SerializeField] public TextMeshProUGUI WaveIndexText { get; private set; }
        [field: SerializeField] public RectTransform CurrencyBar { get; private set; }
        [field: SerializeField] public WaveTimer Timer { get; private set; } 
        [field: SerializeField] public Button SettingsButton { get; private set; } 

        [Inject] private readonly LevelController _levelController;
        [Inject] private readonly UIFrame _uiFrame;

        private void Awake()
        {
            SettingsButton.onClick.AddListener(OpenSettings);
            WaveIndexText.enabled = false;
            HideCurrencyBar();
        }

        private void OpenSettings()
        {
            _uiFrame.Open<PauseScreen>();
        }

        public void OnReady()
        {
            _levelController.WaveStarted += OnWaveStarted;
            _levelController.WaveCompleted += OnWaveCompleted;
        }

        private void OnWaveStarted()
        {
            WaveIndexText.enabled = true;
            WaveIndexText.text = $"WAVE {_levelController.WaveIndex + 1}<alpha=#BB>/{_levelController.WaveCount}";
        }

        private void OnWaveCompleted()
        {
            WaveIndexText.enabled = false;
        }

        public void ShowCurrencyBar()
        {
            CurrencyBar.gameObject.SetActive(true);
        }
        
        public void HideCurrencyBar()
        {
            CurrencyBar.gameObject.SetActive(false);
        }
    }
}