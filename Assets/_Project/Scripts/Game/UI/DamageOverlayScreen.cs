using DG.Tweening;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.Player;
using UnityEngine;

namespace Project.Game.UI
{
    public class DamageOverlayScreen : UIScreen, IInjectionReady
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [Inject] private readonly PlayerHealthController _healthController;

        public void OnReady()
        {
            _healthController.Changed += HealthControllerOnChanged;
            canvasGroup.alpha = 0;
        }

        private void HealthControllerOnChanged(float previous, float current)
        {
            if (previous > current)
            {
                canvasGroup.DOKill();
                canvasGroup.alpha = 0.75f;
                canvasGroup.DOFade(Mathf.Min(0.375f, 1 - current), .5f)
                    .SetEase(Ease.OutQuad);
            }
            else if (previous < current)
            {
                canvasGroup.DOKill();
                canvasGroup.DOFade(Mathf.Min(0.375f, 1 - current), .5f)
                    .SetEase(Ease.OutQuad);
            }
        }
    }
}