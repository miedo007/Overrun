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
        [field: SerializeField] public Collider2D Collider { get; set; }
        [field: SerializeField] public Animation Animation { get; set; }
        [field: SerializeField] public bool FlipScale { get; set; } = true;

        private Vector3 _localPosition;
        private Quaternion _targetRotation;
        private float _lastActivationTime;
        private Transform _transform;
        private float _attackDelay;

        [Inject] private readonly HeroInfo _heroInfo;

        public WeaponData Data { get; private set; }
        public bool IsActivated { get; private set; } = false;
        public Target CurrentTarget { get; private set; }

        public StatInfo AttackRateStat { get; private set; }
        public StatInfo DamageStat { get; private set; }

        public Vector3 LocalPosition
        {
            get => _localPosition;
            set
            {
                _localPosition = value;
                _transform.localPosition = _localPosition;
            }
        }

        private void Awake()
        {
            _transform = transform;
            if (Collider != null)
            {
                Collider.enabled = false;
            }
        }

        public void Initialize(WeaponData weaponData)
        {
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
            return !IsActivated && time >= _lastActivationTime + _attackDelay;
        }


        public void Activate(float time, WeaponController weapon)
        {
            if (CurrentTarget == null || !CurrentTarget.IsActivated)
            {
                return;
            }
            
            var vectorToTarget = (CurrentTarget.Position - _transform.position).normalized;
            _transform.right = vectorToTarget;
            StartCoroutine(ActivationRoutine(weapon));
        }

        private IEnumerator ActivationRoutine(WeaponController weapon)
        {
            IsActivated = true;
            Activated?.Invoke();
            yield return Data.BehaviourBase.ActivationRoutine(weapon);
            _lastActivationTime = Time.time;
            IsActivated = false;
        }

        public void UpdateTarget(Target target, float time, float directionIfNoTarget)
        {
            if (IsActivated)
            {
                return;
            }

            CurrentTarget = target;
            
            if (CurrentTarget == null || !CurrentTarget.IsActivated)
            {
                _transform.localScale = new Vector3(directionIfNoTarget, 1, 1);
                _transform.right = Vector3.right;
                //_transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.identity, 1080 * Time.deltaTime);
            }
            else
            {
                _transform.localScale = new Vector3(1, 1, 1);
                // vector from this object towards the target location
                var vectorToTarget = (CurrentTarget.Position - _transform.position).normalized;
                // rotate that vector by 90 degrees around the Z axis
                /*
                var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                
                _targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, _targetRotation, 1080 * Time.deltaTime);
                */

                _transform.right = vectorToTarget;
                
                if (FlipScale)
                {
                    _transform.localScale = CurrentTarget.Position.x < _transform.position.x 
                        ? FlippedScale
                        : Vector3.one;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var damageReceiver = col.GetComponent<IDamageReceiver>();
            if (damageReceiver != null)
            {
                damageReceiver.ReceiveDamage(DamageStat.GetFloatValue() * Data.DamageFactor);
            }
        }

        private void OnDrawGizmos()
        {
            if (Data != null)
            {
                Gizmos.DrawWireSphere(transform.position, Data.Range);
            }

            if (CurrentTarget != null)
            {
                Gizmos.DrawLine(transform.position, CurrentTarget.Position);
            }
            
        }
    }
}