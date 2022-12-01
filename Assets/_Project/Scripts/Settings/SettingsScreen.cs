using Mtl.Injection;
using Mtl.UiFramework;
using Project.Feedback;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Settings
{
    public class SettingsScreen : UIScreen
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private SettingsToggle hapticsToggle;
        [SerializeField] private SettingsToggle soundToggle;
        [SerializeField] private SettingsToggle musicToggle;
        [SerializeField] private SettingsToggle screenshakeToggle;

        [Inject] private FeedbackController _feedbackController;
        
        [Inject] private UIFrame _uiFrame;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(OnResumeButtonClicked);
        }
        
        private void OnResumeButtonClicked()
        {
            _uiFrame.Close(GetType());
        }

        protected override void OnOpened()
        {
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