using System.Collections;
using Lean.Pool;
using Project.Game.Projectiles;
using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "weapon_behaviour_multi_projectile_", menuName = "Data/WeaponBehaviours/MultiProjectile", order = 0)]
    public class WeaponBehaviourMultiProjectile : WeaponBehaviourBase
    {
        [field: SerializeField] public ProjectileData ProjectileData { get; private set; }
        [field: SerializeField] public int ProjectileCount { get; private set; }
        
        public override IEnumerator ActivationRoutine(WeaponController weaponController)
        {
            var barrel = weaponController.Barrel;
            var targetPosition = barrel.position + barrel.right;
            for (int i = 0; i < ProjectileCount; i++)
            {
                var randomPosition = targetPosition + (Vector3)Random.insideUnitCircle * 0.5f;
                var vectorToTarget = randomPosition - barrel.position;
                var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                var rotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
                var projectile = LeanPool.Spawn(ProjectileData.ProjectilePrefab, weaponController.Barrel.position, rotation);
                projectile.Initialize(ProjectileData);
            }
            
            yield break;
        }
    }
}