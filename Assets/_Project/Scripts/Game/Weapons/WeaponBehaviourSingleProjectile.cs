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
        
        public override IEnumerator ActivationRoutine(WeaponController weaponController)
        {
            var projectile = LeanPool.Spawn(ProjectileData.ProjectilePrefab, weaponController.Barrel.position, weaponController.Barrel.rotation);
            projectile.Initialize(ProjectileData);
            
            yield break;
        }
    }
}