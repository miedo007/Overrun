using System;
using UnityEngine;

namespace Project.Game.Projectiles
{
    public class ProjectileController : MonoBehaviour
    {
        public ProjectileData Data { get; private set; }
        
        public void Initialize(ProjectileData data)
        {
            Data = data;
        }
        
        private void Update()
        {
            transform.position += transform.right * Data.Speed * Time.deltaTime;
        }
    }
}