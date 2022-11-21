using Project.Application;
using Project.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class StatView : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI NameText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ValueText { get; private set; }

        private StatInfo _statInfo;
        
        public void Initialize(StatInfo statInfo)
        {
            NameText.text = $"<sprite name={statInfo.Data.Icon.name}> {statInfo.Data.DisplayNameKey}";
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
            
            ValueText.text = statInfo.GetDisplayValue();

            var color = Color.white;
            if (Mathf.Approximately(value, baseValue))
            {
                color = Colors.GetColor(Colors.White);
            }
            if (value < baseValue)
            {
                color = Colors.GetColor(Colors.Negative);
            }
            else if (value > baseValue)
            {
                color = Colors.GetColor(Colors.Positive);
            }

            ValueText.color = color;
        }
    }
}