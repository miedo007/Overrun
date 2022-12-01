using JetBrains.Annotations;
using Mtl.Bonfire;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Feedback;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [Inject] private BeaconWrapper _beaconWrapper;
        [Inject] private UIFrame _uiFrame;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(OnResumeButtonClicked);
            emailSupportButton.onClick.AddListener(SendEmailToSupport);
        }
        
        public void OnReady()
        {
            playerIdText.text = $"Player ID: {_beaconWrapper.CustomerId}";
        }
        
        private void OnResumeButtonClicked()
        {
            _uiFrame.Close(GetType());
        }

        [UsedImplicitly]
        public void SendEmailToSupport()
        {
            UnityEngine.Application.OpenURL("mailto:privacy@darkmatterplay.com");
        }

        protected override void OnOpened()
        {
            playerIdText.text = $"Player ID: {_beaconWrapper.CustomerId}";
            hapticsToggle.Init(FeedbackController.GetHapticsEnabled(), OnHapticsToggled);
            soundToggle.Init(_feedbackController.Audio.GetSoundEffectsActive(), OnSoundToggled);
            musicToggle.Init(_feedbackController.Audio.GetMusicActive(), OnMusicToggled);
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
        }
    }
}