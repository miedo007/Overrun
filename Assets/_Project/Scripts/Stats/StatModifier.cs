using UnityEngine;

namespace Project.Stats
{
    [System.Serializable]
    public class StatModifier
    {
        [field: SerializeField] public StatData StatData { get; private set; }
        [field: SerializeField] public float Value { get; private set; }
        [field: SerializeField] public StatModifierType ModifierType { get; private set; }

        public int Order => (int) ModifierType;
    }

    public enum StatModifierType
    {
        Flat,
        PercentAdd,
        PercentMult
    }
}