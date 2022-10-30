using System.Collections.Generic;
using Mtl.Injection;
using Project.Game.Targets;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerWeaponsController : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public WeaponData[] WeaponData { get; private set; }
        [field: SerializeField] public SlotConfigData[] WeaponSlotConfigs { get; private set; }

        [Inject] private readonly TargetManager _targetManager;
        [Inject] private readonly PlayerController _playerController;
        
        private readonly List<WeaponController> _weapons = new();

        public void OnReady()
        {
            PlaceWeapons();
        }

        private void PlaceWeapons()
        {
            var slotConfig = WeaponSlotConfigs[WeaponData.Length - 1];
            for (var i = 0; i < WeaponData.Length; i++)
            {
                var data = WeaponData[i];
                var weapon = Instantiate(data.Prefab, transform);
                weapon.LocalPosition = slotConfig.GetPosition(i);
                weapon.Initialize(data);
                _weapons.Add(weapon);
            }
        }

        private void Update()
        {
            var time = Time.time;
            foreach (var weapon in _weapons)
            {
                var target = _targetManager.GetClosestTarget(weapon.Barrel.position, weapon.Data.Range);
                weapon.UpdateTarget(target, time, _playerController.Character.HorizontalDirection);
                
                if (target != null && weapon.ShouldActivate(time))
                {
                    weapon.Activate(time, weapon);
                }
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (WeaponSlotConfigs.Length <= 0)
            {
                return;
            }

            var configIndex = WeaponData.Length - 1;
            for (var i = 0; i < WeaponSlotConfigs[configIndex].Count; i++)
            {
                var worldPos = transform.TransformPoint(WeaponSlotConfigs[configIndex].GetPosition(i));
                Gizmos.DrawWireSphere(worldPos, 0.1f);
                UnityEditor.Handles.Label(worldPos + (Vector3.up * 0.25f), $"{i}");
            }
        }
        #endif
    }
}