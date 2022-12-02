using DG.Tweening;
using Lean.Pool;
using Project.Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.UI
{
    public class ActivatedItemView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Image backer;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public void Initialize(ItemData item)
        {
            image.sprite = item.Sprite;
            backer.color = item.Tier.Color;
            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0, 0.25f)
                .SetDelay(1.25f)
                .OnComplete(() => LeanPool.Despawn(this));
        }
    }
}