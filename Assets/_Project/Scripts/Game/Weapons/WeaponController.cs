using System;
using System.Collections;
using Mtl.Injection;
using Project.Game.Targets;
using Project.Heroes;
using Project.Stats;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public StatInfo CooldownReductionStat { get; private set; }
        public StatInfo DamageStat { get; private set; }
        public StatInfo DamagePercentStat { get; private set; }
        public StatInfo CriticalChanceStat { get; private set; }
        public StatInfo KnockbackStat { get; private set; }

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

            CooldownReductionStat = _heroInfo.GetStat(weaponData.Type.CooldownReductionStat);
            CooldownReductionStat.Changed += OnHeroCooldownReductionChanged;
            OnHeroCooldownReductionChanged(CooldownReductionStat);
            
            DamageStat = _heroInfo.GetStat(weaponData.Type.DamageStat);
            DamageStat.Changed += OnDamageStatChanged;
            OnDamageStatChanged(DamageStat);
            
            DamagePercentStat = _heroInfo.GetStat(weaponData.Type.DamagePercentStat);
            DamagePercentStat.Changed += OnDamagePercentStatChanged;
            OnDamagePercentStatChanged(DamagePercentStat);

            CriticalChanceStat = _heroInfo.GetStat(weaponData.Type.CriticalChanceStat);
            KnockbackStat = _heroInfo.GetStat(weaponData.Type.KnockbackStat);

            Initialized?.Invoke();
        }

        private void OnDestroy()
        {
            CooldownReductionStat.Changed -= OnHeroCooldownReductionChanged;
            DamageStat.Changed -= OnDamageStatChanged;
            DamagePercentStat.Changed -= OnDamagePercentStatChanged;
        }

        private void OnDamageStatChanged(StatInfo statInfo)
        {
        }

        private void OnHeroCooldownReductionChanged(StatInfo statInfo)
        {
            _attackDelay = Data.Cooldown * (1f - CooldownReductionStat.GetFloatValue());
        }
        
        private void OnDamagePercentStatChanged(StatInfo statInfo)
        {
        }

        public bool ShouldActivate(float time)
        {
            return !IsActivated && time >= _lastActivationTime + _attackDelay;
        }


        public void Activate(WeaponController weapon)
        {
            if (CurrentTarget == null || !CurrentTarget.IsActivated)
            {
                return;
            }

            PointToTarget();
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

        public void UpdateTarget(Target target, float dt, float directionIfNoTarget)
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
                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.identity, 1080 * dt);
            }
            else
            {
                // reset any scale flipping
                _transform.localScale = Vector3.one;
                
                // vector from this object towards the target location
                var vectorToTarget = (CurrentTarget.Position - _transform.position).normalized;
                // rotate that vector by 90 degrees around the Z axis
                var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                
                _targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, _targetRotation, 1080 * dt);

                _transform.right = vectorToTarget;
                
                if (FlipScale)
                {
                    _transform.localScale = CurrentTarget.Position.x < _transform.position.x 
                        ? FlippedScale
                        : Vector3.one;
                }
            }
        }

        private void PointToTarget()
        {
            var vectorToTarget = (CurrentTarget.Position - _transform.position).normalized;
            _transform.right = vectorToTarget;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var damageReceiver = col.GetComponent<IDamageReceiver>();
            if (damageReceiver != null)
            {
                var direction = (Vector2)(col.transform.position - _transform.position).normalized;
                var force = direction.normalized * KnockbackStat.GetFloatValue() * Data.KnockbackMultiplier;
                var isCritical = Random.value <= CriticalChanceStat.GetFloatValue();
                damageReceiver.ReceiveDamage(GetDamageValue(isCritical), isCritical, force, gameObject);
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

        public float GetDamageValue(bool isCritical)
        {
            var damageStat = DamageStat.GetFloatValue();
            var damagePercentStat = 1f + DamagePercentStat.GetFloatValue();
            var weaponDamageFactor = Data.DamageFactor;
            var total = damageStat * damagePercentStat * weaponDamageFactor;

            if (isCritical)
            {
                total *= Data.CriticalDamageMultiplier;
            }
            /*
             Debug.Log($"DamageStat :: {damageStat} \n" +
                      $"DamagePercentStat :: {damagePercentStat} \n" +
                      $"Weapon.DamageFactor :: {weaponDamageFactor} \n" +
                      $"Total :: {total}");
            */
            
            return total;
        }
    }
}