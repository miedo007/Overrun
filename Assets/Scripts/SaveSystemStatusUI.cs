using UnityEngine;
using UnityEngine.UI;
using CrazyGames;

namespace Project.Application
{
    public class SaveSystemStatusUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text statusText;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button checkStatusButton;

        private void Start()
        {
            if (loginButton != null)
            {
                loginButton.onClick.AddListener(OnLoginButtonClicked);
            }
            
            if (checkStatusButton != null)
            {
                checkStatusButton.onClick.AddListener(OnCheckStatusButtonClicked);
            }
            
            // Subscribe to login status changes
            SaveSystemIntegration.OnLoginStatusChanged += OnLoginStatusChanged;
            
            // Update UI immediately
            UpdateStatusUI();
        }

        private void OnDestroy()
        {
            SaveSystemIntegration.OnLoginStatusChanged -= OnLoginStatusChanged;
        }

        private void OnLoginStatusChanged(bool isLoggedIn)
        {
            UpdateStatusUI();
        }

        private void UpdateStatusUI()
        {
            if (statusText == null) return;

            var isLoggedIn = SaveSystemIntegration.IsUserLoggedIn;
            var sdkAvailable = false;
            
            try
            {
                sdkAvailable = CrazySDK.IsAvailable;
            }
            catch
            {
                // SDK not available
            }

            var status = $"Save System Status:\n";
            status += $"• Crazy Games SDK: {(sdkAvailable ? "Available" : "Not Available")}\n";
            status += $"• User Status: {(isLoggedIn ? "Logged In" : "Not Logged In")}\n";
            status += $"• Storage: {(isLoggedIn && sdkAvailable ? "Cloud (Crazy Games)" : "Local Files")}\n";

            statusText.text = status;

            // Update button state
            if (loginButton != null)
            {
                loginButton.interactable = sdkAvailable && !isLoggedIn;
                var buttonText = loginButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = isLoggedIn ? "Logged In" : "Login to Crazy Games";
                }
            }
        }

        private void OnLoginButtonClicked()
        {
            Debug.Log("Login button clicked");
            SaveSystemIntegration.ShowLoginDialog();
        }

        private void OnCheckStatusButtonClicked()
        {
            Debug.Log("Check status button clicked");
            SaveSystemIntegration.CheckUserLoginStatus();
            UpdateStatusUI();
        }
    }
}