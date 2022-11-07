using System.Collections.Generic;
using UnityEngine;

namespace Project.Game.Collectibles
{
    public class Collector : MonoBehaviour
    {
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
        [field: SerializeField] public float CollectSpeed { get; private set; } = 15;

        private readonly List<Collectible> _collectibles = new();

        private static readonly Collider2D[] Results = new Collider2D[256];
        
        private void LateUpdate()
        {
            var resultCount = Physics2D.OverlapCircleNonAlloc(transform.position, Range, Results, LayerMask);
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
                    collectible.Precollect();
                    _collectibles.Add(collectible);
                }
            }
        }
    }
}