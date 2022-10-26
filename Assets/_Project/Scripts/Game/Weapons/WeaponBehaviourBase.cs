using System.Collections;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponBehaviourBase : ScriptableObject
    {
        public virtual IEnumerator ActivationRoutine()
        {
            yield break;
        }
    }
}