using System;
using Project.Application;
using Project.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class StatDeltaView : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI ValueText { get; private set; }

        private StatInfo _statInfo;
        
        public void Initialize(StatInfo currentStatInfo, StatInfo nextStatInfo)
        {
            Refresh(currentStatInfo, nextStatInfo);
        }
        
        private void Refresh(StatInfo currentStatInfo, StatInfo nextStatInfo)
        {
            var value = nextStatInfo.GetFloatValue() - currentStatInfo.GetFloatValue();
            if (Mathf.Approximately(value, 0))
            {
                ValueText.text = "-";
                ValueText.color = Colors.GetColor(Colors.White);
                return;
            }

            var valuePrefix = value > 0 ? "+" : "";
            var displayAsPercent = currentStatInfo.Data.DisplayAsPercent;
            var valuePostfix = displayAsPercent ? "%" : "";
            value = displayAsPercent ? value * 100f : value;
            ValueText.text = $"{valuePrefix}{value : 0.0}{valuePostfix}";

            var color = Color.white;
            if (value < 0)
            {
                color = Colors.GetColor(Colors.Negative);
            }
            else if (value > 0)
            {
                color = Colors.GetColor(Colors.Positive);
            }

            ValueText.color = color;
        }
    }
}