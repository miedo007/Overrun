using System;
using Lean.Pool;
using Mtl.Injection;
using Project.Game.Projectiles;
using Project.Game.Rooms;
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
        [SerializeField] private new Rigidbody2D rigidbody;
        
        [Inject] private readonly PopupTextManager _popupTextManager;
        [Inject] private readonly RoomManager _roomManager;

        private Transform _transform;
        private int _facingDirection = 1;
        private float _lastAttackTime;
        private float _lastKnockbackTime;
        
        private const float KnockbackDuration = 0.1f;

        public EnemyData Data { get; private set; }
        public float CurrentHealth { get; private set; }
        public float CurrentMeleeDamage { get; private set; }

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

        public void Initialize(EnemyData enemyData, Vector3 playerPosition, int level, int wave)
        {
            _lastKnockbackTime = 0f;
            _lastAttackTime = 0f;
            
            selfTarget.Deactivate();
            var direction = (playerPosition - _transform.position).normalized;
            FacePlayer(direction);
            
            Data = enemyData;
            CurrentHealth = GetScaledValue(enemyData.BaseHealth, level, wave);
            CurrentMeleeDamage = GetScaledValue(enemyData.MeleeDamage, level, 0);
        }

        private float GetScaledValue(float baseValue, int level, int wave)
        {
            var levelScaled = baseValue * Mathf.Pow(Data.LevelScaling, level);
            return levelScaled * Mathf.Pow(Data.WaveScaling, wave);
        }

        public void Step(float dt, float time, Vector3 playerPosition)
        {
            if (time < _lastKnockbackTime + KnockbackDuration)
            {
                rigidbody.position = _roomManager.ClampToRoomRect(rigidbody.position, out var didClampX, out var didClampY);
                if (didClampX || didClampY)
                {
                    rigidbody.velocity = Vector2.zero;
                }

                return;
            }
            
            var currentPosition = _transform.position;
            var direction = (playerPosition - currentPosition).normalized;
            FacePlayer(direction);
            //rigidbody.AddForce(direction * Data.MoveSpeed * dt, ForceMode2D.Impulse);
            rigidbody.velocity = direction * Data.MoveSpeed;
            //_transform.position = currentPosition + direction * Data.MoveSpeed * dt;
        }

        private void FacePlayer(Vector3 directionToPlayer)
        {
            FacingDirection = directionToPlayer.x < 0 ? -1 : 1;
        }

        public bool ReactToProjectile(ProjectileController projectile, float damage, bool isCritical, Vector2 force)
        {
            if (selfTarget.IsActivated)
            {
                ApplyDamage(damage, isCritical, force);
                return true;
            }

            return false;
        }

        private void ApplyDamage(float damage, bool isCritical, Vector2 force)
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
            
            rigidbody.AddForce(force, ForceMode2D.Impulse);
            _lastKnockbackTime = Time.time;
            
            var damageInt = Mathf.CeilToInt(damage);
            if (damageInt <= 0)
            {
                return;
            }
            
            _popupTextManager.DisplayTextAtPosition($"{damageInt}", isCritical ? Color.yellow : Color.white, selfTarget.Position);
        }

        public void Kill()
        {
            Killed?.Invoke(this);
        }
        
        public void Cleanup()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            selfTarget.Deactivate();
            LeanPool.Despawn(this);
        }
        

        public bool ReceiveDamage(float damage, bool isCritical, Vector2 force)
        {
            if (selfTarget.IsActivated)
            {
                ApplyDamage(damage, isCritical, force);
                return true;
            }

            return false;
        }

        public void Activate()
        {
            selfTarget.Activate();
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            TryAttack(collision.collider);
        }

        public void OnCollisionStay2D(Collision2D collision)
        {
            TryAttack(collision.collider);
        }

        private void TryAttack(Collider2D other)
        {
            if (!selfTarget.IsActivated)
            {
                return;
            }
            
            if (Time.time >= _lastAttackTime + Data.MeleeAttackRate)
            {
                var damageReceiver = other.GetComponent<IDamageReceiver>();
                if (damageReceiver == null)
                {
                    return;
                }

                damageReceiver.ReceiveDamage(CurrentMeleeDamage, false, Vector2.zero);
                _lastAttackTime = Time.time;
            }
        }
    }
}