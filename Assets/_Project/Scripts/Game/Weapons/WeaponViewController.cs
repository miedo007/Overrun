using System;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponViewController : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void SetSlot(WeaponSlot weaponSlot)
        {
            _transform.SetParent(weaponSlot.Transform, true);
            _transform.localPosition = Vector3.zero;
            SetSortingOrder(weaponSlot.SortingOrder);
        }

        public void SetSortingOrder(int sortingOrder)
        {
            SpriteRenderer.sortingOrder = sortingOrder;
        }

        public void SetFlipped(bool flipX)
        {/*
            transform.localRotation = flipX ? Quaternion.Euler(0,180,0) : Quaternion.identity;*/
        }
    }
}