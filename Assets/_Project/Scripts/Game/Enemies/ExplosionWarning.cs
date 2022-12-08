using DG.Tweening;
using UnityEngine;

namespace Project.Game.Enemies
{
    public class ExplosionWarning : MonoBehaviour
    {
        [SerializeField] private Transform fillTransform;
        [SerializeField] private Transform outlineTransform;
        
        public void Initialize(float radius, float duration)
        {
            fillTransform.localScale = Vector3.zero;
            outlineTransform.localScale = Vector3.zero;
            outlineTransform.DOScale(radius, 0.1f);
            fillTransform.DOScale(radius, duration);
        }
    }
}