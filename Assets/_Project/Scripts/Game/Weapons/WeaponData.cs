using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "data_weapon_", menuName = "Data/WeaponData", order = 0)]
    public class WeaponData : ScriptableObject
    {
        [field: SerializeField] public WeaponController Prefab { get; private set; }
        [field: SerializeField] public WeaponBehaviourBase BehaviourBase { get; set; }
        [field: SerializeField] public float ActivationRate { get; private set; } = 1f;
    }
}