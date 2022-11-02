using Lean.Pool;
using UnityEngine;

namespace Project.Game.Collectibles
{
    public class Collectible : MonoBehaviour
    {
        public void Collect()
        {
            LeanPool.Despawn(this);
        }
    }
}