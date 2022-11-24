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
        [field: SerializeField] public Transform FeedbackRoot { get; private set; }
        [field: SerializeField] public ParticleSystem ActivateFx { get; private set; }
        

        private WeaponData _weaponData;
        
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
            _weaponData = WeaponController.Data;
        }

        private void OnWeaponControllerActivated()
        {
            if (_weaponData.ActivationFeedback != null)
            {
                _weaponData.ActivationFeedback.Play(FeedbackRoot.position, FeedbackRoot.rotation);
            }

            if (ActivateFx != null)
            {
                ActivateFx.Play();
            }
            
            Animation.Play();
        }

        private void LateUpdate()
        {
            var position = WeaponController.LocalPosition;
            var isFlipped = transform.localScale.x < 0;
            if (position.y <= 0.25f) // Front guns
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

    }
}