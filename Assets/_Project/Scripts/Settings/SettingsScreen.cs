using JetBrains.Annotations;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Feedback;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
        
        private bool _isOpenedFromGameplay = false;

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
            // Determine if we're in gameplay or main menu based on current scene
            _isOpenedFromGameplay = IsCurrentSceneGameplay();
            
            // Only stop gameplay when settings screen opens if we're actually in gameplay
            if (_isOpenedFromGameplay)
            {
                CrazySDK.Game.GameplayStop();
            }
            
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
            // Only resume gameplay when settings screen closes if we were in gameplay
            if (_isOpenedFromGameplay)
            {
                CrazySDK.Game.GameplayStart();
            }
        }
        
        private bool IsCurrentSceneGameplay()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            return currentSceneName == "game" || currentSceneName.ToLower().Contains("game");
        }
    }
}
