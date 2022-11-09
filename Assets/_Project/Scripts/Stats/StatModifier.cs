using Project.Application;
using UnityEngine;

namespace Project.Stats
{
    [System.Serializable]
    public class StatModifier
    {
        [field: SerializeField] public StatData StatData { get; private set; }
        [field: SerializeField] public float Value { get; private set; }
        [field: SerializeField] public StatModifierType ModifierType { get; private set; }
        [field: SerializeField] public bool DisplayAsPercentage { get; set; }

        public int Order => (int) ModifierType;

        public string GetDisplayValue()
        {
            var valueString = "";
            var displayValue = Value;
            if (DisplayAsPercentage)
            {
                displayValue *= 100f;
            }
            
            var color = Value < 0 ? Colors.Negative : Colors.Positive;
            var prefix = Value < 0 ? "" : "+";
            var postfix = DisplayAsPercentage ? "%" : "";
            
            if (displayValue - Mathf.Round(displayValue) == 0)
            {
                valueString = $"{prefix}{displayValue:0}{postfix}";
            }
            else
            {
                valueString = $"{prefix}{displayValue:0.0}{postfix}";
            }
            
            valueString = $"<color={color}>{valueString} <sprite tint=1 name={StatData.Icon.name}>";
            return valueString;
        }

        public Color GetColor()
        {
            return Value < 0 ? Colors.GetColor(Colors.Negative) : Colors.GetColor(Colors.Positive);
        }
    }

    public enum StatModifierType
    {
        Flat,
        PercentAdd,
        PercentMult
    }
}