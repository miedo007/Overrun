using System;
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
        [field: SerializeField, Tooltip("Weapon distance from player from 1-6 weapons")] 
        public Vector2 RadiusRange { get; private set; } = new(0.375f, 0.75f);

        [field: SerializeField] public AnimationCurve RadiusCurve { get; private set; }
        

        [Inject] private readonly TargetManager _targetManager;
        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly HeroInfo _heroInfo;

        private readonly List<WeaponController> _weapons = new();
        
        public void OnReady()
        {
            _heroInfo.CurrentWeaponsChanged += OnCurrentWeaponsChanged;
        }

        private void OnDestroy()
        {
            _heroInfo.CurrentWeaponsChanged -= OnCurrentWeaponsChanged;
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
            
            for (var i = 0; i < _heroInfo.CurrentWeapons.Count; i++)
            {
                var data = _heroInfo.CurrentWeapons[i];
                var weapon = Instantiate(data.Prefab, transform);
                weapon.LocalPosition = GetWeaponPosition(i);
                weapon.Initialize(data);
                _weapons.Add(weapon);
            }
        }
        
        public Vector3 GetWeaponPosition(int slotIndex)
        {
            var weaponCount = _heroInfo.CurrentWeapons.Count;
            var angleBetween = 360f / weaponCount;
            
            // don't offset if only one weapon
            var angleOffset = weaponCount == 1 ? 0f : angleBetween * 0.5f;
            var angle = -angleOffset - (angleBetween * slotIndex);
            var radiusFactor = RadiusCurve.Evaluate(weaponCount / 6f);
            var radius = Mathf.Lerp(RadiusRange.x, RadiusRange.y, radiusFactor);
            
            var position = Quaternion.Euler(0, 0, angle) * (Vector3.down * radius);
            return position;
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            var time = Time.time;
            
            foreach (var weapon in _weapons)
            {
                var target = _targetManager.GetClosestTarget(weapon.Barrel.position, weapon.Data.Range);
                weapon.UpdateTarget(target, dt, _playerController.Character.HorizontalDirection);
                
                if (target != null && weapon.ShouldActivate(time))
                {
                    weapon.Activate(weapon);
                }
            }
        }
    }
}