using System;
using System.Collections;
using Mtl.Injection;
using Project.Game.Targets;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        public event Action Initialized;
        public event Action Activated;
        
        [field: SerializeField] public Transform Barrel { get; set; }
        
        private float _lastActivationTime;
        private Transform _transform;
        private float _attackDelay;

        [Inject] private readonly HeroInfo _heroInfo;

        public WeaponData Data { get; private set; }
        public WeaponSlot Slot { get; private set; }

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

            var heroAttackRateStatInfo = _heroInfo.GetStat(weaponData.AttackRateStat);
            _attackDelay = 1f / (Data.AttackRate * heroAttackRateStatInfo.GetFloatValue());
            Initialized?.Invoke();
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
                    ? new Vector3(1, -1, 1)
                    : new Vector3(1,1,1);
            }
        }
    }
}