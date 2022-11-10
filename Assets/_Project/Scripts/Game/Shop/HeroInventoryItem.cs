using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
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

        private BaseData _data;

        public void Initialize(BaseData data)
        {
            _data = data;
            Frame.color = _data.Tier.Color;
            Backer.color = _data.Tier.Color;
            Icon.sprite = _data.Sprite;
            Icon.enabled = true;
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