using System.Collections;
using UnityEngine;

namespace Project.Game.Weapons
{
    [CreateAssetMenu(fileName = "weapon_behaviour_melee_", menuName = "Data/WeaponBehaviours/Melee", order = 0)]
    public class WeaponBehaviourMelee : WeaponBehaviourBase
    {
        
        public override IEnumerator ActivationRoutine(WeaponController weapon)
        {
            weapon.Collider.enabled = true;
            yield return WaitForAnimationClip(weapon.Animation);
            weapon.Collider.enabled = false;
        }
    }
}