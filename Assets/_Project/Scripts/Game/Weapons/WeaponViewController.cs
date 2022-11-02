using System;
using NaughtyAttributes;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponViewController : MonoBehaviour
    {
        [field: SerializeField] public WeaponController WeaponController { get; private set; }
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        [field: SerializeField] public Animation Animation { get; private set; }
        
        private void Awake()
        {
            WeaponController.Initialized += OnWeaponControllerInitialized;
            WeaponController.Activated += OnWeaponControllerActivated;
        }

        private void OnDestroy()
        {
            WeaponController.Initialized -= OnWeaponControllerInitialized;
            WeaponController.Activated -= OnWeaponControllerActivated;
        }

        private void OnWeaponControllerInitialized()
        {
            //SetSortingOrder(WeaponController.LocalPosition.y <= 0 ? WeaponController.FlippedHorizontal ? : : -2);
        }

        private void OnWeaponControllerActivated()
        {
            Animation.Play();
        }

        private void LateUpdate()
        {
            var position = WeaponController.LocalPosition;
            var isFlipped = transform.localScale.x < 0;
            if (position.y <= .25f) // Front guns
            {
                if (isFlipped)
                {
                    SetSortingOrder(position.x <= 0 ? 2 : 3);
                }
                else
                {
                    SetSortingOrder(position.x <= 0 ? 3 : 2);
                }
            }
            else
            {
                if (isFlipped)
                {
                    SetSortingOrder(position.x <= 0 ? -3 : -2);
                }
                else
                {
                    SetSortingOrder(position.x <= 0 ? -2 : -3);
                }
            }
        }

        public void SetSortingOrder(int sortingOrder)
        {
            SpriteRenderer.sortingOrder = sortingOrder;
        }

        [Button()]
        public void Test()
        {
            
        }
    }
}