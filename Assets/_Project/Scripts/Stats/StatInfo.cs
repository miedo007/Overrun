using System;
using UnityEngine;

namespace Project.Stats
{
    [System.Serializable]
    public class StatInfo
    {
        [field: SerializeField] public StatData Data { get; private set; }
        [field: SerializeField] public float BaseValue { get; private set; }
        [field: SerializeField] public LevelScaling LevelScaling { get; private set; }
        [field: SerializeField] public RoundingType RoundingType { get; private set; }

        private StatInfo() { }

        public StatInfo GetLeveledStatInfo(int level)
        {
            var statInfo = new StatInfo();
            statInfo.Data = Data;
            statInfo.BaseValue = GetBaseFloatValueForLevel(level);
            statInfo.RoundingType = RoundingType;
            return statInfo;
        }

        private float GetBaseFloatValueForLevel(int level)
        {
            // apply modifiers
            return LevelScaling.GetValueForLevel(BaseValue, level);
        }
       
        public float GetFloatValue()
        {
            return BaseValue;
        }

        public int GetIntLevel()
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
    }

    [System.Serializable]
    public class LevelScaling
    {
        [field: SerializeField] public float Coefficient { get; private set; } = 1.09f;

        public float GetValueForLevel(float baseValue, int level)
        {
            return baseValue * Mathf.Pow(Coefficient, level);
        }
    }

    public enum RoundingType
    {
        Round,
        Ceil,
        Floor
    }
}