using System.Collections.Generic;
using Mtl.Injection;
using Project.Heroes;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Collectibles
{
    public class Collector : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public StatData PickupRangeModifier { get; private set; }
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
        [field: SerializeField] public float CollectSpeed { get; private set; } = 15;

        [Inject] private readonly HeroRegistry _heroRegistry;

        private StatInfo _pickupRangeModifierStat;
        
        private readonly List<Collectible> _collectibles = new();

        private static readonly Collider2D[] Results = new Collider2D[256];
        
        private void LateUpdate()
        {
            var resultCount = Physics2D.OverlapCircleNonAlloc(transform.position, Range * _pickupRangeModifierStat.GetFloatValue(), Results, LayerMask);
            for (var i = 0; i < resultCount; i++)
            {
                var collectible = Results[i].GetComponent<Collectible>();
                if (collectible != null)
                {
                    collectible.Precollect();
                    _collectibles.Add(collectible);
                }
            }

            var targetPosition = transform.position;
            for (var i = _collectibles.Count - 1; i >= 0; i--)
            {
                var collectible = _collectibles[i];
                var collectiblePosition = collectible.transform.position;
                collectible.transform.position = Vector3.MoveTowards(collectiblePosition, targetPosition, CollectSpeed * Time.deltaTime);
                
                if ((collectible.transform.position - targetPosition).sqrMagnitude < 0.2f)
                {
                    collectible.Collect();
                    _collectibles.RemoveAt(i);
                }
            }
        }

        public void CollectAll()
        {
            var resultCount = Physics2D.OverlapCircleNonAlloc(transform.position, 100, Results, LayerMask);
            for (var i = 0; i < resultCount; i++)
            {
                var collectible = Results[i].GetComponent<Collectible>();
                if (collectible != null)
                {
                    if (collectible.Data.AutoCollectOnWaveComplete)
                    {
                        collectible.Precollect();
                        _collectibles.Add(collectible);
                    }
                    else
                    {
                        collectible.Cleanup();
                    }
                }
            }
        }

        public void OnReady()
        {
            Debug.Log(_heroRegistry.ActiveHero);
            Debug.Log(_heroRegistry.ActiveHero.Data);
            _pickupRangeModifierStat = _heroRegistry.ActiveHero.GetStat(PickupRangeModifier);
        }
    }
}