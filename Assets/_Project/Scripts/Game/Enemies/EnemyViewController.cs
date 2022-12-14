using System;
using DG.Tweening;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class EnemyViewController : MonoBehaviour
    {
        [field: SerializeField] public EnemyController EnemyController { get; private set; }
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        [field: SerializeField] public Animation Animation { get; private set; }

        [SerializeField] private bool _useRootScalingForDirection;

        private MaterialPropertyBlock _propertyBlock;
        private Tween _flashTween;
        private static readonly int FlashProperty = Shader.PropertyToID("_Flash");

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            ResetFlash();
            EnemyController.DamageTaken += OnDamageTaken;
            EnemyController.FacingDirectionChanged += OnFacingDirectionChanged;
            EnemyController.WillDie += OnWillDie;
            EnemyController.Initialized += OnInitialized;
        }

        private void OnDisable()
        {
            EnemyController.DamageTaken -= OnDamageTaken;
            EnemyController.FacingDirectionChanged -= OnFacingDirectionChanged;
            EnemyController.WillDie -= OnWillDie;
            EnemyController.Initialized -= OnInitialized;
        }

        private void OnInitialized(EnemyController obj)
        {
            Animation.Play(EnemyController.Data.WalkAnimation.name);
        }

        private void OnWillDie(EnemyController obj)
        {
            OnDamageTaken();
            Animation.Play(obj.Data.DeathAnimation.name);
        }

        private void OnFacingDirectionChanged(int direction)
        {
            if (_useRootScalingForDirection)
            {
                transform.localScale = new Vector3(direction, 1, 1);
            }
            else
            {
                SpriteRenderer.flipX = direction < 0;
            }
        }

        private void ResetFlash()
        {
            if (_flashTween != null)
            {
                _flashTween.Kill();
            }

            SpriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(FlashProperty, 0);
            SpriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void OnDamageTaken()
        {
            if (_flashTween != null)
            {
                _flashTween.Kill();
            }

            SpriteRenderer.GetPropertyBlock(_propertyBlock);
            // Assign our new value.
            _propertyBlock.SetFloat(FlashProperty, 1);
            // Apply the edited values to the renderer.
            SpriteRenderer.SetPropertyBlock(_propertyBlock);

            _flashTween = DOVirtual.Float(1f, 0f, 0.2f, v =>
                {
                    SpriteRenderer.GetPropertyBlock(_propertyBlock);
                    // Assign our new value.
                    _propertyBlock.SetFloat(FlashProperty, v);
                    // Apply the edited values to the renderer.
                    SpriteRenderer.SetPropertyBlock(_propertyBlock);
                })
                .SetDelay(0.1f)
                .SetEase(Ease.OutQuad);
        }
    }
}