using DG.Tweening;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Heroes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class HeroInventoryItem : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public Image Frame { get; private set; }
        [field: SerializeField] public Image Backer { get; private set; }

        [Inject] private readonly UIFrame _uiFrame;
        [Inject] private readonly HeroInfo _heroInfo;

        private Sequence _mergeSequence;
        private BaseData _data;

        public void Initialize(BaseData data)
        {
            _data = data;
            
            if (_data != null && _heroInfo != null && _heroInfo.CanMerge(_data))
            {
                _mergeSequence = DOTween.Sequence();
                _mergeSequence.Append(transform.DOScale(1.0375f, 0.125f)
                        .SetLoops(2, LoopType.Yoyo))
                    .AppendInterval(2f)
                    .SetLoops(-1);

                _mergeSequence.Play();
            }
            
            Frame.color = _data.Tier.Color;
            Backer.color = _data.Tier.Color;
            Icon.sprite = _data.Sprite;
            Icon.enabled = true;
        }

        private void OnDestroy()
        {
            if (_mergeSequence != null)
            {
                _mergeSequence.Kill();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_data == null)
            {
                return;
            }
            
            var itemDetailsScreen = _uiFrame.Open<ItemDetailsScreen>();
            itemDetailsScreen.Initialize(_data);
        }
    }
}