using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Settings
{
    public class SettingsToggle : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject on;
        [SerializeField] private GameObject off;

        private bool _isOn;

        private Action _toggleCallback;

        private void Start()
        {
            button.onClick.AddListener(Toggle);
            Refresh();
        }

        public void Init(bool isOn, Action toggleCallback)
        {
            _isOn = isOn;
            _toggleCallback = toggleCallback;
            Refresh();
        }

        private void Toggle()
        {
            _isOn = !_isOn;
            _toggleCallback?.Invoke();
            Refresh();
        }

        public bool IsOn()
        {
            return _isOn;
        }

        private void Refresh()
        {
            on.SetActive(_isOn);
            off.SetActive(!_isOn);
        }
    }
}