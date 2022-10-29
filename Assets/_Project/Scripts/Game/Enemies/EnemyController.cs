using System;
using Mtl.Injection;
using Project.Game.Projectiles;
using Project.Game.Targets;
using Project.Game.Weapons;
using Project.PopupText;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyController : MonoBehaviour, IProjectileReactor, IDamageReceiver
    {
        public event Action DamageTaken;
        public event Action<EnemyController> Killed;
        public event Action<int> FacingDirectionChanged;

        [SerializeField] private Target selfTarget;

        [Inject] private readonly PopupTextManager _popupTextManager;

        private Transform _transform;
        private int _facingDirection = 1;
        private float _lastAttackTime;
        
        public EnemyData Data { get; private set; }
        public float CurrentHealth { get; private set; }

        public int FacingDirection
        {
            get => _facingDirection;
            set
            {
                if (_facingDirection == value)
                {
                    return;
                }
                
                _facingDirection = value;
                FacingDirectionChanged?.Invoke(_facingDirection);
            }
        }

        private void Awake()
        {
            _transform = transform;
        }

        public void Initialize(EnemyData enemyData, Vector3 playerPosition)
        {
            selfTarget.Enabled = false;
            var direction = (playerPosition - _transform.position).normalized;
            FacePlayer(direction);
            CurrentHealth = enemyData.BaseHealth;
            Data = enemyData;
        }

        public void Step(float dt, float time, Vector3 playerPosition)
        {
            var currentPosition = _transform.position;
            var direction = (playerPosition - currentPosition).normalized;
            FacePlayer(direction);
            _transform.position = currentPosition + direction * Data.MoveSpeed * dt;
        }

        private void FacePlayer(Vector3 directionToPlayer)
        {
            FacingDirection = directionToPlayer.x < 0 ? -1 : 1;
        }

        public bool ReactToProjectile(ProjectileController projectile, float damage)
        {
            if (selfTarget.Enabled)
            {
                ApplyDamage(damage);
                return true;
            }

            return false;
        }

        private void ApplyDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                Kill();
            }
            else
            {
                DamageTaken?.Invoke();
            }
                
            _popupTextManager.DisplayTextAtPosition($"{Mathf.RoundToInt(damage)}", Color.white, selfTarget.transform.position);
        }

        public void Kill()
        {
            selfTarget.Enabled = false;
            Killed?.Invoke(this);
        }

        public bool ReceiveDamage(float damage)
        {
            if (selfTarget.Enabled)
            {
                ApplyDamage(damage);
                return true;
            }

            return false;
        }

        public void Activate()
        {
            selfTarget.Enabled = true;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            TryAttack(other);
        }

        private void TryAttack(Collider2D other)
        {
            if (Time.time >= _lastAttackTime + Data.MeleeAttackRate)
            {
                
                var damageReceiver = other.GetComponent<IDamageReceiver>();
                if (damageReceiver == null)
                {
                    return;
                }

                damageReceiver.ReceiveDamage(Data.MeleeDamage);
                _lastAttackTime = Time.time;
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            TryAttack(other);
        }
        
        
    }
}