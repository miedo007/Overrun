using System.Collections;
using Lean.Pool;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponBehaviourLauncher : WeaponBehaviourBase
    {
        [SerializeField] private Launchable launchablePrefab;
        [SerializeField] private float launchForce;

        public override IEnumerator ActivationRoutine(WeaponController weapon)
        {
            var launchable = LeanPool.Spawn(launchablePrefab, weapon.Barrel.position, Quaternion.identity);
            launchable.Initialize( weapon.GetDamageValue(false), 
                weapon.CriticalChanceStat.GetFloatValue(), 
                weapon.Data.CriticalDamageMultiplier,
                weapon.KnockbackStat.GetFloatValue() * weapon.Data.KnockbackMultiplier,
                1.5f,
                weapon.Barrel.right * launchForce
            );
            yield break;
        }
    }
}