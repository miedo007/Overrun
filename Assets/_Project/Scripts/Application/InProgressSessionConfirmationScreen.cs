using System;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.Levels;
using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Application
{
    public class InProgressSessionConfirmationScreen : UIScreen
    {
        public event Action<bool> Confirmed;

        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image heroImage;

        [Inject] private readonly InProgressSessionInfo _inProgressSessionInfo;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly LevelDatabase _levelDatabase;
        
        private void Awake()
        {
            confirmButton.onClick.AddListener(ConfirmButtonClicked);
            cancelButton.onClick.AddListener(CancelButtonClicked);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            var save = _inProgressSessionInfo.InProgressSave;
            var heroData   = _heroRegistry.GetHeroData(save.HeroId);
            if (heroData != null)
            {
                heroImage.sprite = heroData.Sprite;
            }

            var levelData = _levelDatabase.GetLevel(save.LevelIndex);
            levelText.text = $"LEVEL {save.LevelIndex + 1}";
            progressText.text = $"WAVE {save.WaveIndex + 1} of {levelData.Waves.Length}";
        }


        private void CancelButtonClicked()
        {
            Confirmed?.Invoke(false);
        }

        private void ConfirmButtonClicked()
        {
            Confirmed?.Invoke(true);
        }
    }
}