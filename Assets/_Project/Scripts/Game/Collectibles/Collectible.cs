using Lean.Pool;
using UnityEngine;

namespace Project.Game.Collectibles
{
    public class Collectible : MonoBehaviour
    {
        [field: SerializeField] public bool IsRotatable { get; private set; } = true;

        [SerializeField] private new Collider2D collider;

        private CollectibleData _data;
        
        public CollectibleData Data => _data;

        public void Precollect()
        {
            collider.enabled = false;
        }
        
        public void Collect()
        {
            _data.Collect(transform.position);
            Cleanup();
        }

        public void Initialize(CollectibleData data)
        {
            _data = data;
            collider.enabled = true;
        }

        public void Cleanup()
        {
            LeanPool.Despawn(this);
        }
    }
}