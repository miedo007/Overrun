using UnityEngine;

namespace Project.Stats
{
    [CreateAssetMenu(fileName = "data_stat_", menuName = "Data/StatData", order = 0)]
    public class StatData : ScriptableObject
    {
        [field: SerializeField] public string DisplayNameKey { get; private set; } = "STAT_NAME";
        [field: SerializeField] public string DescriptionKey { get; private set; } = "DESCRIPTION";
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public bool DisplayAsPercent { get; private set; }
        [field: SerializeField] public string DisplayPattern { get; private set; }
    }
}