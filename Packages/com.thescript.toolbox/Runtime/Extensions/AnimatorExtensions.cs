using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class AnimatorExtensions
    {
        public static IEnumerator PlayClip(this Animator animator, AnimationClip clip)
        {
            var playable = AnimationPlayableUtilities.PlayClip(animator, clip, out var graph);
            graph.Play();

            while (playable.GetTime() < clip.length)
            {
                yield return null;
            }

            graph.Destroy();
        }

        public static IEnumerator WhileState(this Animator animator, string stateName) => WhileState(animator, 0, Animator.StringToHash(stateName));
        public static IEnumerator WhileState(this Animator animator, int layer, string stateName) => WhileState(animator, layer, Animator.StringToHash(stateName));
        public static IEnumerator WhileState(this Animator animator, int stateHash) => WhileState(animator, 0, stateHash);

        public static IEnumerator WhileState(this Animator animator, int layer, int stateHash)
        {
            while (true)
            {
                var state = animator.GetCurrentAnimatorStateInfo(layer);
                if (state.shortNameHash != stateHash)
                {
                    break;
                }

                yield return null;
            }
        }

        public static IEnumerator WhileNotState(this Animator animator, string stateName) => WhileNotState(animator, 0, Animator.StringToHash(stateName));
        public static IEnumerator WhileNotState(this Animator animator, int layer, string stateName) => WhileNotState(animator, layer, Animator.StringToHash(stateName));
        public static IEnumerator WhileNotState(this Animator animator, int stateHash) => WhileNotState(animator, 0, stateHash);

        public static IEnumerator WhileNotState(this Animator animator, int layer, int stateHash)
        {
            while (true)
            {
                var state = animator.GetCurrentAnimatorStateInfo(layer);
                if (state.shortNameHash == stateHash)
                {
                    break;
                }

                yield return null;
            }
        }
    }
}