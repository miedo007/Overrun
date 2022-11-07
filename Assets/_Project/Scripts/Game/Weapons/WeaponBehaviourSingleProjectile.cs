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
        
        public override IEnumerator ActivationRoutine(WeaponController weapon)
        {
            var projectile = LeanPool.Spawn(ProjectileData.ProjectilePrefab, weapon.Barrel.position, weapon.Barrel.rotation);
            projectile.Initialize(
                ProjectileData,
                weapon.GetDamageValue(false), 
                weapon.CriticalChanceStat.GetFloatValue(), 
                weapon.Data.CriticalDamageMultiplier,
                weapon.KnockbackStat.GetFloatValue()
            );
            
            yield return WaitForAnimationClip(weapon.Animation, 0.5f);
        }
    }
}