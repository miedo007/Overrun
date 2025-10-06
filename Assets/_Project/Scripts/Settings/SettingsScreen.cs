using JetBrains.Annotations;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Feedback;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrazyGames;

namespace Project.Settings
{
    public class SettingsScreen : UIScreen, IInjectionReady
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private SettingsToggle hapticsToggle;
        [SerializeField] private SettingsToggle soundToggle;
        [SerializeField] private SettingsToggle musicToggle;
        [SerializeField] private SettingsToggle screenshakeToggle;
        [SerializeField] private TextMeshProUGUI playerIdText;
        [SerializeField] private Button emailSupportButton;

        [Inject] private FeedbackController _feedbackController;
        [Inject] private UIFrame _uiFrame;

        private void Awake()
        {
            if (closeButton != null) closeButton.onClick.AddListener(OnResumeButtonClicked);
            if (emailSupportButton != null) emailSupportButton.onClick.AddListener(SendEmailToSupport);
        }

        public void OnReady()
        {
            if (playerIdText != null) playerIdText.text = "Player ID: —";
        }

        private void OnResumeButtonClicked()
        {
            _uiFrame.Close(GetType());
        }

        [UsedImplicitly]
public void SendEmailToSupport()
{
    UnityEngine.Application.OpenURL("mailto:mehdi.elmoussali@gmail.com");
}

        protected override void OnOpened()
        {
            // Stop gameplay when settings screen opens
            CrazySDK.Game.GameplayStop();
            
            if (playerIdText != null) playerIdText.text = "Player ID: —";

            if (hapticsToggle != null)
                hapticsToggle.Init(FeedbackController.GetHapticsEnabled(), OnHapticsToggled);

            if (soundToggle != null)
                soundToggle.Init(_feedbackController.Audio.GetSoundEffectsActive(), OnSoundToggled);

            if (musicToggle != null)
                musicToggle.Init(_feedbackController.Audio.GetMusicActive(), OnMusicToggled);

            if (screenshakeToggle != null)
                screenshakeToggle.Init(FeedbackController.GetScreenShakeActive(), OnScreenShakeToggled);
        }

        private void OnSoundToggled()
        {
            _feedbackController.Audio.SetSoundEffectsActive(soundToggle.IsOn());
        }

        private void OnMusicToggled()
        {
            _feedbackController.Audio.SetMusicActive(musicToggle.IsOn());
        }

        private void OnHapticsToggled()
        {
            FeedbackController.EnableHaptics(hapticsToggle.IsOn());
        }

        private void OnScreenShakeToggled()
        {
            FeedbackController.EnableScreenShake(screenshakeToggle.IsOn());
        }

        protected override void OnClosed()
        {
            // Resume gameplay when settings screen closes
            CrazySDK.Game.GameplayStart();
        }
    }
}
