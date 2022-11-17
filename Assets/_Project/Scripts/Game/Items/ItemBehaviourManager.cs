using System;
using System.Collections.Generic;
using Mtl.Injection;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.Items
{
    public class ItemBehaviourManager : MonoBehaviour, IInjectionReady
    {
        public event Action<ItemBehaviour> ItemBehaviourPerformed;
        
        [SerializeField] private ItemBehaviourTrigger[] behaviourTriggers;
        
        [Inject] private readonly HeroRegistry _heroRegistry;

        private readonly Dictionary<ItemBehaviourTrigger, List<ItemBehaviour>> _triggerBehavioursDict = new();

        public void OnReady()
        {
            _heroRegistry.ActiveHero.ItemsWillChange += OnItemsWillChange;
            _heroRegistry.ActiveHero.ItemsChanged += OnItemsChanged;

            foreach (var trigger in behaviourTriggers)
            {
                trigger.Triggered += OnTriggered;
            }
        }

        private void OnDestroy()
        {
            _heroRegistry.ActiveHero.ItemsWillChange -= OnItemsWillChange;
            _heroRegistry.ActiveHero.ItemsChanged -= OnItemsChanged;
            
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
                if (behaviour.Perform(position))
                {
                    ItemBehaviourPerformed?.Invoke(behaviour);
                }
            }
        }

        private void OnItemsWillChange()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            foreach (var kvp in _triggerBehavioursDict)
            {
                foreach (var behaviour in kvp.Value)
                {
                    behaviour.Cleanup();
                }
            }
            
            _triggerBehavioursDict.Clear();
        }

        private void OnItemsChanged()
        {
            foreach (var item in _heroRegistry.ActiveHero.Items)
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