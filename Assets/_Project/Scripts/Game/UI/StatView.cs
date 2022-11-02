using System;
using Project.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class StatView : MonoBehaviour
    {
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ValueText { get; private set; }

        private StatInfo _statInfo;
        
        public void Initialize(StatInfo statInfo)
        {
            Icon.sprite = statInfo.Data.Icon;
            _statInfo = statInfo;
            _statInfo.Changed += OnStatChanged;
            Refresh(_statInfo);
        }

        private void OnDestroy()
        { 
            if (_statInfo != null)
            {
                _statInfo.Changed -= OnStatChanged;
            }
        }

        private void OnStatChanged(StatInfo statInfo)
        {
            Refresh(statInfo);
        }

        private void Refresh(StatInfo statInfo)
        {
            var value = statInfo.GetFloatValue();
            var baseValue = statInfo.BaseValue;
            ValueText.text = $"{value:0.00}";

            var color = Color.white;
            if (value < baseValue)
            {
                color = Color.red;
            }
            else if (value > baseValue)
            {
                color = Color.green;
            }

            ValueText.color = color;
        }
    }
}