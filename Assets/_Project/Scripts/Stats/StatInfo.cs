using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Project.Stats
{
    [System.Serializable]
    public class StatInfo
    {
        public event Action<StatInfo> Changed;
        
        [field: SerializeField] public StatData Data { get; private set; }
        [field: SerializeField] public float BaseValue { get; private set; }
        [field: SerializeField] public LevelScaling LevelScaling { get; private set; }
        [field: SerializeField] public RoundingType RoundingType { get; private set; }
        [field: SerializeField] public bool HasMinValue { get; private set; }
        [field: SerializeField, ShowIf("HasMinValue")] public float MinValue { get; private set; }
        [field: SerializeField] public bool HasMaxValue { get; private set; }
        [field: SerializeField] public bool AllowModifiersBeyondMaxValue { get; private set; }
        [field: SerializeField, ShowIf("HasMaxValue")] public float MaxValue { get; private set; }

        private int _roundingValue = 4;
        private float _modifiedValue;
        private bool _isDirty = true;
        public List<StatModifier> StatModifiers { get; private set; } = new();

        private StatInfo() { }

        public StatInfo GetLeveledStatInfo(int level)
        {
            var levelStatInfo = new StatInfo
            {
                Data = Data,
                BaseValue = GetBaseFloatValueForLevel(level),
                RoundingType = RoundingType,
                HasMinValue = HasMinValue,
                MinValue = MinValue,
                HasMaxValue = HasMaxValue,
                MaxValue = MaxValue,
                AllowModifiersBeyondMaxValue = AllowModifiersBeyondMaxValue
            };
            return levelStatInfo;
        }

        private float GetBaseFloatValueForLevel(int level)
        {
            var baseValue = LevelScaling.GetValueForLevel(BaseValue, level);
            if (HasMaxValue)
            {
                baseValue = Mathf.Min(baseValue, MaxValue);
            }
            return (float)Math.Round(baseValue, _roundingValue);
        }
       
        public float GetFloatValue()
        {
            if (_isDirty)
            {
                CalculateStat();
            }
            
            return _modifiedValue;
        }
        
        
        public string GetDisplayValue()
        {
            var value = Data.IsIntValue ? GetIntValue() : GetFloatValue();
            if (Data.DisplayAsPercent)
            {
                value *= 100f;
            }
            
            if (Mathf.Approximately(value - Mathf.Round(value),0))
            {
                return string.Format(Data.RoundNumberDisplayPattern, value);
            }

            return string.Format(Data.DisplayPattern, value);
        }


        public void CalculateStat()
        {
            _modifiedValue = BaseValue;

            var sumPercentAdd = 0f;

            for (var i = 0; i < StatModifiers.Count; i++)
            {
                var modifier = StatModifiers[i];
                switch (modifier.ModifierType)
                {
                    case StatModifierType.Flat:
                        _modifiedValue += modifier.Value;
                        break;
                    case StatModifierType.PercentAdd:
                        sumPercentAdd += modifier.Value;
                        if (i + 1 >= StatModifiers.Count || StatModifiers[i + 1].ModifierType != StatModifierType.PercentAdd)
                        {
                            _modifiedValue *=
                                1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                            sumPercentAdd = 0; // Reset the sum back to 0
                        }
                        break;
                    case StatModifierType.PercentMult:
                        _modifiedValue *= 1 + modifier.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (HasMinValue)
            {
                _modifiedValue = Mathf.Max(_modifiedValue, MinValue);
            }
            
            if (HasMaxValue && !AllowModifiersBeyondMaxValue)
            {
                _modifiedValue = Mathf.Min(_modifiedValue, MaxValue);
            }
            
            _modifiedValue = (float)Math.Round(_modifiedValue, _roundingValue);
        }

        public int GetIntValue()
        {
            var intValue = 0;
            var floatValue = GetFloatValue();
            switch (RoundingType)
            {
                case RoundingType.Round:
                    intValue = Mathf.RoundToInt(floatValue);
                    break;
                case RoundingType.Ceil:
                    intValue = Mathf.CeilToInt(floatValue);
                    break;
                case RoundingType.Floor:
                    intValue = Mathf.FloorToInt(floatValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return intValue;
        }
        
        public float GetIntBaseValue()
        {
            var intValue = 0;
            var floatValue = BaseValue;
            switch (RoundingType)
            {
                case RoundingType.Round:
                    intValue = Mathf.RoundToInt(floatValue);
                    break;
                case RoundingType.Ceil:
                    intValue = Mathf.CeilToInt(floatValue);
                    break;
                case RoundingType.Floor:
                    intValue = Mathf.FloorToInt(floatValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return intValue;
        }

        public void AddModifier(StatModifier statModifier)
        {
            StatModifiers.Add(statModifier);
            StatModifiers.Sort(CompareModifierOrder);
            CalculateStat();
            Changed?.Invoke(this);
        }
        
        public void RemoveModifier(StatModifier statModifier)
        {
            StatModifiers.Remove(statModifier);
            CalculateStat();
            Changed?.Invoke(this);
        }
        
        private int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            
            if (a.Order > b.Order)
                return 1;
            
            return 0;
        }
        
        
        /// <summary>
        /// Returns -1 if negatively modified, 0 if unmodified, or 1 if positively modified
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int GetModifiedDirection()
        {
            var value = GetFloatValue();
            
            if (Mathf.Approximately(value, BaseValue))
            {
                return 0;
            }
            
            if (value < BaseValue)
            {
                return -1;
            }

            return 1;
        }
    }

    [System.Serializable]
    public class LevelScaling
    {
        [field: SerializeField] public float Coefficient { get; private set; } = 1.09f;
        [field: SerializeField] public bool IsLinear { get; private set; } = false;
        [field: SerializeField] public float LinearIncrease { get; private set; } = .1f;

        public float GetValueForLevel(float baseValue, int level)
        {
            if (!IsLinear)
            {
                return baseValue * Mathf.Pow(Coefficient, level);
            }
            else
            {
                return baseValue + LinearIncrease * level;
            }
        }
    }

    public enum RoundingType
    {
        Round,
        Ceil,
        Floor
    }
}