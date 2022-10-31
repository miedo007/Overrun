using Mtl.Injection;
using Mtl.UiFramework;
using Project.Heroes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Project.Game.Weapons
{
    public class WeaponTestScreen : UIScreen
    {
        [field: SerializeField] public Button AddRandomButton { get; private set; }
        [field: SerializeField] public Button RemoveRandomButton { get; private set; }

        [Inject] private readonly HeroInfo _heroInfo;
        [Inject] private readonly WeaponDatabase _weaponDatabase;

        private void Awake()
        {
            AddRandomButton.onClick.AddListener(AddRandomWeapon);
            RemoveRandomButton.onClick.AddListener(RemoveRandomWeapon);
        }

        private void RefreshButtonState()
        {
            AddRandomButton.interactable = _heroInfo.CurrentWeapons.Count < 12;
            RemoveRandomButton.interactable = _heroInfo.CurrentWeapons.Count > 0;
        }

        private void AddRandomWeapon()
        {
            _heroInfo.AddWeapon(_weaponDatabase.GetRandomWeapon());
            RefreshButtonState();
        }

        private void RemoveRandomWeapon()
        {
            _heroInfo.RemoveWeaponAtIndex(Random.Range(0, _heroInfo.CurrentWeapons.Count));
            RefreshButtonState();
        }
    }
}