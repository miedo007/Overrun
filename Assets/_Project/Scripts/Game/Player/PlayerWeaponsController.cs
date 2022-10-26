using System;
using System.Collections.Generic;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerWeaponsController : MonoBehaviour
    {
        [field: SerializeField] public WeaponData[] WeaponData { get; private set; }
        
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }

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
    }
}