using UnityEngine;

namespace Project.Stats
{
    [CreateAssetMenu(fileName = "data_character_stats_", menuName = "Data/CharacterStats", order = 0)]
    public class CharacterStats : ScriptableObject
    {
        [field: SerializeField] public StatInfo[] Stats { get; private set; }
    }
}