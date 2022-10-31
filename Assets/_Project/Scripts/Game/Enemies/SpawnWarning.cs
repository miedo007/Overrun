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
        
        
        public IEnumerator ShowRoutine()
        {
            transform.localScale = Vector3.zero;
            yield return transform.DOScale(Vector3.one, ShowDuration).WaitForCompletion();
            yield return new WaitForSeconds(WarningDuration);
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