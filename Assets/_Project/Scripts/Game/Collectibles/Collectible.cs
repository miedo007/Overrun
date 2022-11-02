using System;
using Lean.Pool;
using UnityEngine;

namespace Project.Game.Collectibles
{
    public class Collectible : MonoBehaviour
    {
        [SerializeField] private new Collider2D collider;

        private CollectibleData _data;
        
        public void Precollect()
        {
            collider.enabled = false;
        }
        
        public void Collect()
        {
            _data.Collect();
            LeanPool.Despawn(this);
        }

        public void Initialize(CollectibleData data)
        {
            _data = data;
            collider.enabled = true;
        }
    }
}