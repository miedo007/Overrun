using System;
using Project.Game.Items;
using Project.Game.Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class StatUpgradeView : MonoBehaviour
    {
        public event Action<ItemData> Selected;
        
        [SerializeField] private Image icon;
        [SerializeField] private Image tierBacker;
        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private TextMeshProUGUI descriptionField;
        [SerializeField] private Button selectButton;
        
        [Header("Premium Upgrade (Ad Required)")]
        [SerializeField] private Button premiumButton; // Separate button for premium upgrades
        [SerializeField] private Image adIcon; // Ad icon overlay to show premium upgrade
        
        private ItemData _data;
        private bool _isPremium = false;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnRegularClicked);
            if (premiumButton != null)
            {
                premiumButton.onClick.AddListener(OnPremiumClicked);
            }
        }

        private void OnRegularClicked()
        {
            if (!_isPremium)
            {
                // Regular upgrade - direct selection
                Selected?.Invoke(_data);
            }
        }

        private void OnPremiumClicked()
        {
            if (_isPremium)
            {
                // Premium upgrade - show ad first
                Debug.Log("[StatUpgradeView] Premium upgrade clicked - showing rewarded ad");
                
                // Disable premium button during ad
                if (premiumButton != null) premiumButton.interactable = false;
                
                RewardedAds.ShowRewardedAd(
                    onAdFinished: () => {
                        Debug.Log("[StatUpgradeView] ✅ Rewarded ad completed - granting premium upgrade!");
                        Selected?.Invoke(_data);
                    },
                    onAdFailed: () => {
                        Debug.Log("[StatUpgradeView] ❌ Rewarded ad failed - restoring button state");
                        // Restore premium button state
                        if (premiumButton != null) premiumButton.interactable = true;
                    }
                );
            }
        }

        public void Initialize(ItemData data, bool isPremium = false)
        {
            _data = data;
            _isPremium = isPremium;
            
            icon.sprite = _data.Sprite;
            tierBacker.color = _data.Tier.Color;

            var statModifier = data.StatModifiers[0];
            nameField.text = $"{_data.DisplayName} {_data.Tier.NamePostfix}";
            descriptionField.text = string.Format(_data.Description, statModifier.GetDisplayValue());
            
            // Reset button states and show/hide appropriate elements
            SetupPremiumVisuals();
        }
        
        private void SetupPremiumVisuals()
        {
            if (_isPremium)
            {
                // Premium upgrade: show premium button and ad icon, hide regular button
                if (selectButton != null) selectButton.gameObject.SetActive(false);
                if (premiumButton != null) 
                {
                    premiumButton.gameObject.SetActive(true);
                    premiumButton.interactable = true; // Ensure button is always enabled initially
                }
                if (adIcon != null) 
                {
                    adIcon.gameObject.SetActive(true);
                    Debug.Log($"[StatUpgradeView] ⭐ Premium upgrade configured: {_data.DisplayName} - Premium button and ad icon shown");
                }
            }
            else
            {
                // Regular upgrade: show regular button, hide premium button and ad icon
                if (selectButton != null) 
                {
                    selectButton.gameObject.SetActive(true);
                    selectButton.interactable = true; // Ensure button is always enabled initially
                }
                if (premiumButton != null) premiumButton.gameObject.SetActive(false);
                if (adIcon != null) adIcon.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Force reset button states - useful for debugging button disable issues
        /// </summary>
        public void ForceResetButtonStates()
        {
            Debug.Log($"[StatUpgradeView] Force resetting button states for {(_data != null ? _data.DisplayName : "Unknown")}");
            if (selectButton != null) selectButton.interactable = true;
            if (premiumButton != null) premiumButton.interactable = true;
        }
    }
}