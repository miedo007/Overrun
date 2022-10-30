using System.Collections.Generic;
using Mtl.Injection;
using Project.Game.Targets;
using Project.Game.Weapons;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerWeaponsController : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public SlotConfigData[] WeaponSlotConfigs { get; private set; }

        [Inject] private readonly TargetManager _targetManager;
        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly HeroInfo _heroInfo;
        
        
        private readonly List<WeaponController> _weapons = new();
        
        public void OnReady()
        {
            _heroInfo.CurrentWeaponsChanged += OnCurrentWeaponsChanged;
        }

        private void OnCurrentWeaponsChanged()
        {
            ClearWeapons();
            PlaceWeapons();
        }

        private void ClearWeapons()
        {
            foreach (var weapon in _weapons)
            {
                Destroy(weapon.gameObject);
            }
            
            _weapons.Clear();
        }

        private void PlaceWeapons()
        {
            if (_heroInfo.CurrentWeapons.Count <= 0)
            {
                return;
            }
            
            var slotConfig = WeaponSlotConfigs[_heroInfo.CurrentWeapons.Count - 1];
            
            for (var i = 0; i < _heroInfo.CurrentWeapons.Count; i++)
            {
                var data = _heroInfo.CurrentWeapons[i];
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
    }
}