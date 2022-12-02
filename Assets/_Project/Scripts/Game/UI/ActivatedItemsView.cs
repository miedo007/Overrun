using Lean.Pool;
using Mtl.Injection;
using Project.Game.Items;
using UnityEngine;

namespace Project.Game.UI
{
    public class ActivatedItemsView : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private ActivatedItemView itemPrefab;
        [SerializeField] private RectTransform parent;
        
        [Inject] private readonly ItemBehaviourManager _itemBehaviourManager;
        
        public void OnReady()
        {
            _itemBehaviourManager.ItemBehaviourPerformed += OnItemBehaviourPerformed;
        }

        private void OnItemBehaviourPerformed(BehaviourItemPair pair)
        {
            var item = LeanPool.Spawn(itemPrefab, parent);
            item.Initialize(pair.Item);
        }
    }
}