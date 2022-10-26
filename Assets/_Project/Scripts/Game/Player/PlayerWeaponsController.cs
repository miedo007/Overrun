using System;
using System.Collections.Generic;
using Mtl.Injection;
using Project.Game.Targets;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerWeaponsController : MonoBehaviour
    {
        [field: SerializeField] public WeaponData[] WeaponData { get; private set; }
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }

        [Inject] private readonly TargetManager _targetManager;

        private readonly List<WeaponSlot> _activeSlots = new List<WeaponSlot>();

        private void Start()
        {
            FillWeaponSlots();
        }

        private void FillWeaponSlots()
        {
            for (int i = 0; i < WeaponSlots.Length; i++)
            {
                if (i >= WeaponData.Length)
                {
                    break;
                }

                var slot = WeaponSlots[i];
                var data = WeaponData[i];
                slot.SetWeapon(data);
                _activeSlots.Add(slot);
            }
        }

        public void SetFlipped(bool flipX)
        {
            foreach (var slot in _activeSlots)
            {
                 slot.SetFlipped(flipX);
            }
        }

        private void Update()
        {
            var time = Time.time;
            foreach (var weaponSlot in _activeSlots)
            {
                var weapon = weaponSlot.Weapon;
                var target = _targetManager.GetClosestTarget(weapon.Barrel.position);
                weapon.UpdateTarget(target);
                
                if (weapon.ShouldActivate(time))
                {
                    weapon.Activate(time, weapon);
                }
            }
        }
    }
}