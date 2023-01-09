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
        [field: SerializeField] public Vector3 offset { get; private set; } = new(0, 0.2f);

        [SerializeField, Range(0f, 180f)] private float _maxAngle = 180f;
        [SerializeField] private float _yScale = 1f;

        [SerializeField] private int _weaponPreviewIndex = -1;

        [Inject] private readonly TargetManager _targetManager;
        [Inject] private readonly PlayerController _playerController;
        [Inject] private readonly HeroRegistry _heroRegistry;

        private HeroInfo _heroInfo;
        private readonly List<WeaponController> _weapons = new();

        public void OnReady()
        {
            _heroRegistry.ActiveHeroChanged += RegisterHero;
            RegisterHero(_heroRegistry.ActiveHero);
        }

        private void RegisterHero(HeroInfo info)
        {
            if (_heroInfo != null)
            {
                _heroInfo.WeaponsChanged -= OnWeaponsChanged;
            }

            _heroInfo = info;
            _heroInfo.WeaponsChanged += OnWeaponsChanged;
        }

        private void OnDestroy()
        {
            if (_heroInfo != null)
            {
                _heroInfo.WeaponsChanged -= OnWeaponsChanged;
            }

            _heroRegistry.ActiveHeroChanged -= RegisterHero;
        }

        private void OnWeaponsChanged()
        {
            ClearWeapons();
            PlaceWeapons();
        }

        private void ClearWeapons()
        {
            foreach (var weapon in _weapons)
            {
                if (weapon != null)
                {
                    Destroy(weapon.gameObject);
                }
            }

            _weapons.Clear();
        }

        private void PlaceWeapons()
        {
            if (_heroRegistry.ActiveHero.CurrentWeapons.Count <= 0)
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

        private Vector3 GetWeaponPosition(int slotIndex) => GetWeaponPosition(slotIndex, _heroInfo.CurrentWeapons.Count);

        private Vector3 GetWeaponPosition(int slotIndex, int weaponCount)
        {
            float angle;
            if (weaponCount <= 1)
            {
                angle = 0f;
            }
            else
            {
                angle = (weaponCount / 2f - 0.5f) * (360f / weaponCount);
                // Don't offset if only one weapon
                if (angle > _maxAngle)
                {
                    angle = Mathf.Lerp(-_maxAngle, _maxAngle, slotIndex / (float)(weaponCount - 1));
                }
                else
                {
                    angle = Mathf.Lerp(-angle, angle, slotIndex / (float)(weaponCount - 1));
                }
            }

            var radiusFactor = RadiusCurve.Evaluate(weaponCount / 6f);
            var radius = Mathf.Lerp(RadiusRange.x, RadiusRange.y, radiusFactor);

            var position = Quaternion.Euler(0, 0, angle) * (Vector3.down * radius);
            position.y *= _yScale;
            position += offset;
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

        private void OnDrawGizmosSelected()
        {
            const int weaponCount = 8;

            if (_weaponPreviewIndex is > 0 and <= weaponCount)
            {
                DrawIndex(_weaponPreviewIndex, Color.green);
            }
            else
            {
                for (var i = 1; i <= weaponCount; ++i)
                {
                    DrawIndex(i, Color.HSVToRGB(i / (float)(weaponCount + 1), 1f, 1f));
                }
            }

            void DrawIndex(int index, Color color)
            {
                Gizmos.color = color;

                const float granularity = 1f / 360f;
                var radiusFactor = RadiusCurve.Evaluate(index / 6f);
                for (var f = 0f; f < 1f; f += granularity)
                {
                    var radius = Mathf.Lerp(RadiusRange.x, RadiusRange.y, radiusFactor);
                    var position = Quaternion.Euler(0, 0, Mathf.Lerp(-_maxAngle, _maxAngle, f)) * (Vector3.down * radius);
                    position.y *= _yScale;
                    position += offset;

                    var nextPosition = Quaternion.Euler(0, 0, Mathf.Lerp(-_maxAngle, _maxAngle, f + granularity)) * (Vector3.down * radius);
                    nextPosition.y *= _yScale;
                    nextPosition += offset;

                    Gizmos.DrawLine(position, nextPosition);
                }

                for (var j = 0; j < index; ++j)
                {
                    var pos = GetWeaponPosition(j, index);
                    Gizmos.DrawIcon(pos, "", true, Gizmos.color);
                }
            }
        }
    }
}