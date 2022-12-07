using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class LockToggle : MonoBehaviour
    {
        public Action<bool> StateChanged;
        
        [SerializeField] private Image lockImage;
        [SerializeField] private Sprite unlockedSprite;
        [SerializeField] private Sprite lockedSprite;
        [SerializeField] private Image backerImage;
        [SerializeField] private Sprite unlockedBacker;
        [SerializeField] private Sprite lockedBacker;
        [SerializeField] private Button button;

        public bool IsOn { get; set; }
        
        private void Awake()
        {
            button.onClick.AddListener(Toggle);
        }

        private void Toggle()
        {
            SetState(!IsOn);
            StateChanged?.Invoke(this);
        }

        public void SetState(bool isOn)
        {
            IsOn = isOn;
            lockImage.sprite = IsOn ? lockedSprite : unlockedSprite;
            backerImage.sprite = IsOn ? lockedBacker : unlockedBacker;
        }
    }
}