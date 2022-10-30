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
        [field: SerializeField] public WeaponData[] AllWeapons { get; private set; }

        [Inject] private readonly HeroInfo _heroInfo;

        private void Awake()
        {
            AddRandomButton.onClick.AddListener(AddRandomWeapon);
            RemoveRandomButton.onClick.AddListener(RemoveRandomWeapon);
        }

        private void RemoveRandomWeapon()
        {
            _heroInfo.RemoveWeaponAtIndex(Random.Range(0, _heroInfo.CurrentWeapons.Count));
        }

        private void AddRandomWeapon()
        {
            if (_heroInfo.CurrentWeapons.Count > 0)
            {
                _heroInfo.AddWeapon(AllWeapons[Random.Range(0, AllWeapons.Length)]);
            }
        }
    }
}