using System;
using UnityEngine;

namespace Project.Stats
{
    [System.Serializable]
    public class StatInfo
    {
        [field: SerializeField] public StatData Data { get; private set; }
        [field: SerializeField] public LevelScaling LevelScaling { get; private set; }
        [field: SerializeField] public RoundingType RoundingType { get; private set; }

        public int GetIntValueForLevel(int level)
        {
            var intValue = 0;
            var floatValue = GetFloatValue(level);
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

        public float GetFloatValue(int level)
        {
            // apply modifiers
            return LevelScaling.GetBaseValueForLevel(level);
        }
    }

    [System.Serializable]
    public class LevelScaling
    {
        [field: SerializeField] public float BaseValue { get; private set; }
        [field: SerializeField] public float Coefficient { get; private set; } = 1.09f;

        public float GetBaseValueForLevel(int level)
        {
            return BaseValue * Mathf.Pow(Coefficient, level);
        }
    }

    public enum RoundingType
    {
        Round,
        Ceil,
        Floor
    }
}