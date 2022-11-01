using System;
using System.Collections;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class SpawnWarning : MonoBehaviour
    {
        [field: SerializeField] public float ShowDuration { get; private set; }
        [field: SerializeField] public float WarningDuration { get; private set; }
        [field: SerializeField] public float HideDuration { get; private set; }

        private Tween _hideTween;
        private Tween _showTween;
        
        private void Awake()
        {
  
        }

        public IEnumerator ShowRoutine()
        {
            transform.localScale = Vector3.zero;
            yield return transform.DOScale(Vector3.one, ShowDuration).WaitForCompletion();

            var delay = WarningDuration;
            while (delay > 0)
            {
                yield return null;
                delay -= Time.deltaTime;
            }
        }
        
        public void Hide()
        {
            StopAllCoroutines();
            transform.DOKill();
            
            transform.DOScale(0, HideDuration)
                .OnComplete(()=>
                    LeanPool.Despawn(this)
                );
        }
    }
}