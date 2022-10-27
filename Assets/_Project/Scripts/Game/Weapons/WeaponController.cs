using System;
using System.Collections;
using Mtl.Injection;
using Project.Game.Targets;
using Project.Heroes;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        private static readonly Vector3 FlippedScale = new Vector3(1, -1, 1);
        
        public event Action Initialized;
        public event Action Activated;
        
        [field: SerializeField] public Transform Barrel { get; set; }
        
        private float _lastActivationTime;
        private Transform _transform;
        private float _attackDelay;

        [Inject] private readonly HeroInfo _heroInfo;

        public WeaponData Data { get; private set; }
        public WeaponSlot Slot { get; private set; }

        public StatInfo AttackRateStat { get; private set; }

        public StatInfo DamageStat { get; private set; }

        private void Awake()
        {
            _transform = transform;
        }

        public void Initialize(WeaponData weaponData, WeaponSlot weaponSlot)
        {
            Slot = weaponSlot;
            _transform.SetParent(weaponSlot.Transform, true);
            _transform.localPosition = Vector3.zero;
            Data = weaponData;

            AttackRateStat = _heroInfo.GetStat(weaponData.Type.AttackRateStat);
            AttackRateStat.Changed += OnHeroAttackRateChanged;
            OnHeroAttackRateChanged(AttackRateStat);
            
            DamageStat = _heroInfo.GetStat(weaponData.Type.DamageStat);
            DamageStat.Changed += OnDamageStatChanged;
            OnDamageStatChanged(DamageStat);
            
            Initialized?.Invoke();
        }

        private void OnDestroy()
        {
            AttackRateStat.Changed -= OnHeroAttackRateChanged;
            DamageStat.Changed -= OnDamageStatChanged;
        }

        private void OnDamageStatChanged(StatInfo obj)
        {
        }

        private void OnHeroAttackRateChanged(StatInfo statInfo)
        {
            _attackDelay = 1f / (Data.AttackRate * AttackRateStat.GetFloatValue());
        }

        public bool ShouldActivate(float time)
        {
            return time >= _lastActivationTime + _attackDelay;
        }

        public bool ShouldTarget(float time)
        {
            return time >= _lastActivationTime + (_attackDelay * 0.5f);
        }

        public void Activate(float time, WeaponController weapon)
        {
            _lastActivationTime = time;
            Activated?.Invoke();
            StartCoroutine(ActivationRoutine(weapon));
        }

        private IEnumerator ActivationRoutine(WeaponController weapon)
        {
            yield return Data.BehaviourBase.ActivationRoutine(weapon);
        }

        public void UpdateTarget(Target target, float time)
        {
            if (!ShouldTarget(time))
            {
                return;
            }
            
            if (target == null)
            {
                _transform.rotation = Quaternion.identity;
                _transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                // vector from this object towards the target location
                var vectorToTarget = (target.transform.position - _transform.position).normalized;
                // rotate that vector by 90 degrees around the Z axis
                var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                
                var targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1080 * Time.deltaTime);
                _transform.localScale = target.transform.position.x < _transform.position.x 
                    ? FlippedScale
                    : Vector3.one;
            }
        }
    }
}