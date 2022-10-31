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
        [field: SerializeField] public float DelayBetweenProjectiles { get; private set; } = -1f;
        [field: SerializeField] public Vector2 ProjectileSpread { get; private set; } = new(-30, 30);
        
        public override IEnumerator ActivationRoutine(WeaponController weapon)
        {
            var barrel = weapon.Barrel;
            var targetPosition = barrel.position + barrel.right;
            for (var i = 0; i < ProjectileCount; i++)
            {
                var vectorToTarget = (targetPosition - barrel.position).normalized;
                var spreadAngle = Random.Range(ProjectileSpread.x, ProjectileSpread.y);
                var spreadRotation = Quaternion.AngleAxis(spreadAngle, Vector3.forward);
                var randomOffsetVector = spreadRotation * vectorToTarget;
                var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * randomOffsetVector;
                var rotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
                
                var projectile = LeanPool.Spawn(ProjectileData.ProjectilePrefab, weapon.Barrel.position, rotation);
                projectile.Initialize(ProjectileData, weapon.DamageStat.GetFloatValue() * weapon.Data.DamageFactor);

                if (DelayBetweenProjectiles > 0f)
                {
                    var time = DelayBetweenProjectiles;
                    while (time > 0)
                    {
                        yield return null;
                        time -= Time.deltaTime;
                    }
                }
            }
            
            yield return WaitForAnimationClip(weapon.Animation, 0.5f);
        }
    }
}