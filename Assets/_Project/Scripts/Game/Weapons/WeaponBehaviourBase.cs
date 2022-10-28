using System.Collections;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponBehaviourBase : ScriptableObject
    {
        public virtual IEnumerator ActivationRoutine(WeaponController weapon)
        {
            yield break;
        }

        protected IEnumerator WaitForAnimationClip(Animation animation)
        {
            while (animation.IsPlaying(animation.clip.name))
            {
                yield return null;
            }
        }
    }
}