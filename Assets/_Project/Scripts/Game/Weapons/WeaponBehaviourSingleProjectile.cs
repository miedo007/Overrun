using System.Collections;
using Lean.Pool;
using Project.Game.Projectiles;
using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "weapon_behaviour_single_projectile_", menuName = "Data/WeaponBehaviours/SingleProjectile", order = 0)]
    public class WeaponBehaviourSingleProjectile : WeaponBehaviourBase
    {
        [field: SerializeField] public ProjectileData ProjectileData { get; private set; }
        
        public override IEnumerator ActivationRoutine()
        {
            var projectile = LeanPool.Spawn(ProjectileData.ProjectilePrefab);
            yield break;
        }
    }
}