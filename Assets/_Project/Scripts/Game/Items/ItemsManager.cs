using System.Collections.Generic;
using Mtl.Injection;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.Items
{
    public class ItemsManager : MonoBehaviour, IInjectionReady
    {
        [SerializeField] private ItemBehaviourTrigger[] behaviourTriggers;
        
        [Inject] private readonly HeroInfo _heroInfo;

        private readonly Dictionary<ItemBehaviourTrigger, List<ItemBehaviour>> _triggerBehavioursDict = new();

        public void OnReady()
        {
            _heroInfo.ItemsWillChange += OnItemsWillChange;
            _heroInfo.ItemsChanged += OnItemsChanged;

            foreach (var trigger in behaviourTriggers)
            {
                trigger.Triggered += OnTriggered;
            }
        }

        private void OnDestroy()
        {
            foreach (var trigger in behaviourTriggers)
            {
                trigger.Triggered -= OnTriggered;
            }
        }

        private void OnTriggered(ItemBehaviourTrigger trigger, Vector3 position)
        {
            if (!_triggerBehavioursDict.ContainsKey(trigger))
            {
                return;
            }

            var behaviours = _triggerBehavioursDict[trigger];
            foreach (var behaviour in behaviours)
            {
                behaviour.Perform();
            }
        }

        private void OnItemsWillChange()
        {
            _triggerBehavioursDict.Clear();
        }

        private void OnItemsChanged()
        {
            foreach (var item in _heroInfo.Items)
            {
                foreach (var pair in item.Behaviours)
                {
                    if (!_triggerBehavioursDict.ContainsKey(pair.Trigger))
                    {
                        _triggerBehavioursDict.Add(pair.Trigger, new List<ItemBehaviour>());
                    }
                    
                    _triggerBehavioursDict[pair.Trigger].Add(pair.Behaviour);
                }
            }
        }
        
        
        
    }

    [System.Serializable]
    public class BehaviourTriggerPair
    {
        [field: SerializeField] public ItemBehaviourTrigger Trigger { get; private set; }
        [field: SerializeField] public ItemBehaviour Behaviour { get; private set; }
    }
}