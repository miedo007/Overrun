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

        protected IEnumerator WaitForAnimationClip(Animation animation, float percentage = 1f)
        {
            if (percentage >= 1)
            {
                while (animation.IsPlaying(animation.clip.name))
                {
                    yield return null;
                }
                
            }
            else
            {
                var time = animation.clip.length * percentage;
                while (time > 0)
                {
                    yield return null;
                    time -= Time.deltaTime;
                }
            }
            
        }
    }
}