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
            SetSortingOrder(WeaponController.Slot.SortingOrder);
        }

        private void OnWeaponControllerActivated()
        {
            Animation.Play();
        }

        public void SetSortingOrder(int sortingOrder)
        {
            SpriteRenderer.sortingOrder = sortingOrder;
        }
    }
}