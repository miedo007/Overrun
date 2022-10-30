using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "data_slot_config_", menuName = "Data/SlotConfigData", order = 0)]
    public class SlotConfigData : ScriptableObject
    {
        [field: SerializeField] public float Radius { get; private set; }
        
        [field: SerializeField] public int Count { get; private set; }
        [field: SerializeField] public float AngleOffset { get; private set; }

        public Vector3 GetPosition(int slotIndex)
        {
            var angle = -AngleOffset - ((360f / Count) * slotIndex);
            var position = Quaternion.Euler(0, 0, angle) * (Vector3.down * Radius);
            return position;
        }
    }
}