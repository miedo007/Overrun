using System;
using Mtl.Injection;
using Project.Game.Projectiles;
using Project.Game.Targets;
using Project.PopupText;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyController : MonoBehaviour, IProjectileReactor
    {
        public event Action DamageTaken;
        public event Action<EnemyController> Killed;
        public event Action<int> FacingDirectionChanged;

        [SerializeField] private Target selfTarget;

        [Inject] private readonly PopupTextManager _popupTextManager;

        private Transform _transform;
        private int _facingDirection = 1;
        
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

        public void Initialize(EnemyData enemyData)
        {
            CurrentHealth = enemyData.BaseHealth;
            Data = enemyData;
            selfTarget.Enabled = true;
        }

        public void Step(float dt, float time, Vector3 playerPosition)
        {
            var currentPosition = _transform.position;
            var direction = (playerPosition - currentPosition).normalized;
            FacingDirection = direction.x < 0 ? -1 : 1;
            _transform.position = currentPosition + direction * Data.MoveSpeed * dt;
        }

        public bool ReactToProjectile(ProjectileController projectile, float damage)
        {
            if (selfTarget.Enabled)
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
                return true;
            }

            return false;
        }

        public void Kill()
        {
            selfTarget.Enabled = false;
            Killed?.Invoke(this);
        }
    }
}