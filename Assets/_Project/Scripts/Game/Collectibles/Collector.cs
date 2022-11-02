using UnityEngine;

namespace Project.Game.Collectibles
{
    public class Collector : MonoBehaviour
    {
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public LayerMask LayerMask { get; private set; }

        private static readonly Collider2D[] Results = new Collider2D[16];
        
        private void FixedUpdate()
        {
            var resultCount =Physics2D.OverlapCircleNonAlloc(transform.position, Range, Results, LayerMask);
            for (int i = 0; i < resultCount; i++)
            {
                var collectible = Results[i].GetComponent<Collectible>();
                if (collectible != null)
                {
                    collectible.Collect();
                }
            }
        }
    }
}