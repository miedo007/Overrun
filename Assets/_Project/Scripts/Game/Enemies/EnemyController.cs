using System;
using Lean.Pool;
using Mtl.Injection;
using Project.Game.Player;
using Project.Game.Projectiles;
using Project.Game.Rooms;
using Project.Game.Targets;
using Project.Game.Weapons;
using Project.PopupText;
using SRDebugger.Internal;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyController : MonoBehaviour, IProjectileReactor, IDamageReceiver
    {
        public event Action DamageTaken;
        public event Action<EnemyController> WillDie;
        public event Action<EnemyController> Initialized;
        public event Action<EnemyController> Killed;
        public event Action<int> FacingDirectionChanged;

        [SerializeField] private Target selfTarget;
        [SerializeField] private new Rigidbody2D rigidbody;
        
        [Inject] private readonly PopupTextManager _popupTextManager;
        [Inject] private readonly RoomManager _roomManager;
        [Inject] private readonly GameData _gameData;

        private Transform _transform;
        private int _facingDirection = 1;
        private float _lastContactAttackTime;
        private float _lastKnockbackTime;
        private float _lastActionTime;
        private Vector3 _wanderPosition;
        private bool _isDead;

        private const float KnockbackDuration = 0.2f;

        public EnemyData Data { get; private set; }
        public float CurrentHealth { get; private set; }
        public float CurrentMeleeDamage { get; private set; }
        public bool IsPerformingAction { get; private set; }
        public Vector2 Position => rigidbody.position;

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
            var currentPosition = _transform.position;
            _wanderPosition = currentPosition;
            _lastKnockbackTime = 0f;
            _lastContactAttackTime = 0f;
            _lastActionTime = Time.time;
            _isDead = false;
            
            selfTarget.Deactivate();
            var direction = (playerPosition - currentPosition).normalized;
            FacePlayer(direction);
            
            Data = enemyData;
            CurrentHealth = GetScaledValue(enemyData.BaseHealth,
                level,
                wave,
                _gameData.HealthScalingPerLevel,
                _gameData.HealthScalingPerWave);
            
            CurrentMeleeDamage = GetScaledValue(enemyData.MeleeDamage,
                level,
                wave,
                _gameData.DamageScalingPerLevel,
                _gameData.DamageScalingPerWave);
            
            Initialized?.Invoke(this);
        }

        private float GetScaledValue(float baseValue, int level, int wave, float levelScaling, float waveScaling)
        {
            var levelScaled = baseValue * ((level * levelScaling) + 1);
            return levelScaled * ((wave * waveScaling) + 1);
        }

        public void Step(float dt, float time, PlayerController playerController)
        {            
            if (time < _lastKnockbackTime + KnockbackDuration)
            {
                ClampToRoom();
                return;
            }
            
            if (_isDead)
            {
                Kill();
            }
            
            if (IsPerformingAction || !selfTarget.IsActivated)
            {
                ClampToRoom();
                return;
            }
            
            if (Data.Action != null && time >= _lastActionTime + Data.Action.Cooldown)
            {
                ClampToRoom();
                rigidbody.velocity = Vector2.zero;
                IsPerformingAction = true;
                StartCoroutine(Data.Action.ActionRoutine(this, playerController, time, () =>
                {
                    IsPerformingAction = false;
                    _lastActionTime = Time.time;
                }));
                
                return;
            }

            
            if (Data.MovementMode == MovementMode.Chase)
            {
                var playerPosition = playerController.Position;
                var currentPosition = _transform.position;
                var direction = (playerPosition - currentPosition).normalized;

                rigidbody.AddForce(direction, ForceMode2D.Impulse);
                rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, Data.MoveSpeed);
                
                FacePlayer(direction);
                ClampToRoom();
            }
            else if (Data.MovementMode == MovementMode.Wander)
            {
                var currentPosition = _transform.position;
                
                // are we at our wander target position?
                if ((_wanderPosition - currentPosition).sqrMagnitude < 0.25f)
                {
                    _wanderPosition = _roomManager.GetValidPositionInRadius(currentPosition, 8f);
                }
                
                var direction = (_wanderPosition - currentPosition).normalized;

                rigidbody.AddForce(direction, ForceMode2D.Impulse);
                rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, Data.MoveSpeed);
                
                FacePlayer(direction);
                ClampToRoom();
            }
            else if (Data.MovementMode == MovementMode.None)
            {
                ClampToRoom();
            }
        }

        private void ClampToRoom()
        {
            rigidbody.position = _roomManager.ClampToRoomRect(rigidbody.position, out var didClampX, out var didClampY);
            if (didClampX || didClampY)
            {
                rigidbody.velocity = Vector2.zero;
            }
        }

        private void FacePlayer(Vector3 directionToPlayer)
        {
            FacingDirection = directionToPlayer.x < 0 ? -1 : 1;
        }

        public bool ReactToProjectile(ProjectileController projectile, float damage, bool isCritical, Vector2 direction, float force)
        {
            if (selfTarget.IsActivated)
            {
                ApplyDamage(damage, isCritical, direction, force, projectile.gameObject);
                return true;
            }

            return false;
        }

        private void ApplyDamage(float damage, bool isCritical, Vector2 direction, float force, GameObject sender)
        {
            if (_isDead)
            {
                return;
            }
            
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                PreKill();
                Knockback(direction * 10f );
            }
            else
            {
                DamageTaken?.Invoke();
                Knockback(direction * force);
            }
            
            var damageInt = Mathf.CeilToInt(damage);
            if (damageInt <= 0)
            {
                return;
            }
            
            _popupTextManager.DisplayTextAtPosition($"{damageInt}",
                selfTarget.Position,
                isCritical ? _popupTextManager.CritDamagePrefab : _popupTextManager.DamagePrefab);
        }

        private void PreKill()
        {
            selfTarget.Deactivate();
            WillDie?.Invoke(this);
            _isDead = true;
        }

        public void Knockback(Vector2 force)
        {
            rigidbody.AddForce(force, ForceMode2D.Impulse);
            _lastKnockbackTime = Time.time;
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
        

        public bool ReceiveDamage(float damage, bool isCritical, Vector2 direction, float force, GameObject sender)
        {
            if (selfTarget.IsActivated)
            {
                ApplyDamage(damage, isCritical, direction, force, sender);
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
            if (_isDead)
            {
                return;
            }
            
            if (other.CompareTag("Enemy"))
            {
                return;
            }
            
            if (!selfTarget.IsActivated)
            {
                return;
            }
            
            if (Time.time >= _lastContactAttackTime + Data.MeleeAttackRate)
            {
                var damageReceiver = other.GetComponent<IDamageReceiver>();
                if (damageReceiver == null)
                {
                    return;
                }

                damageReceiver.ReceiveDamage(CurrentMeleeDamage, false, Vector2.zero, 0, gameObject);
                _lastContactAttackTime = Time.time;
            }
        }
    }
}