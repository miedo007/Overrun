using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "data_weapon_", menuName = "Data/WeaponData", order = 0)]
    public class WeaponData : ScriptableObject
    {
        [field: SerializeField] public WeaponController Prefab { get; private set; }
        [field: SerializeField] public WeaponTypeData Type { get; private set; }
        [field: SerializeField] public WeaponBehaviourBase BehaviourBase { get; set; }
        [field: SerializeField, Tooltip("Duration between attacks")] public float Cooldown { get; private set; } = 1f;
        [field: SerializeField, Tooltip("Multiplied by WeaponType Damage Stat")] public float DamageFactor { get; private set; } = 1f;
        [field: SerializeField] public float Range { get; set; } = 2.5f;
        [field: SerializeField] public float CriticalDamageMultiplier { get; set; } = 2f;
    }
}