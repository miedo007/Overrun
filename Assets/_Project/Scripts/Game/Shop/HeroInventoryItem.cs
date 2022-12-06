using DG.Tweening;
using Mtl.Injection;
using Mtl.UiFramework;
using Project.Application;
using Project.Game.Tutorials;
using Project.Heroes;
using Project.Tiers;
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
        [field: SerializeField] public MergeableNotification MergeableNotif { get; private set; }

        [Inject] private readonly UIFrame _uiFrame;
        [Inject] private readonly HeroRegistry _heroRegistry;
        [Inject] private readonly TierDatabase _tierDatabase;
        
        private HeroInfo _heroInfo;
        private Sequence _mergeSequence;
        private BaseData _data;

        public void Initialize(BaseData data)
        {
            _data = data;

            if (_heroRegistry != null)
            {
                _heroInfo = _heroRegistry.ActiveHero;
            }
            
            if (_data != null && _heroInfo != null && _heroInfo.CanMerge(_data))
            {
                MergeableNotif.Activate(_tierDatabase.GetNextTier(_data.Tier).Color);
                
                if (!PrefKeys.HasCompletedWeaponMergeTutorial())
                {
                    _uiFrame.Open<WeaponMergeTutorialScreen>();
                }
            }
            else
            {
                MergeableNotif.Deactivate();
            }
            
            Frame.color = _data.Tier.Color;
            Backer.color = _data.Tier.Color;
            Backer.enabled = true;
            
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
            itemDetailsScreen.Initialize(_data, _heroInfo);
        }
    }
}