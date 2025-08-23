using System;
using System.Collections.Generic;
using Mtl.Toolbox;
using UnityEngine;
using UnityEngine.UI;

namespace Mtl.UI
{
    [Serializable]
    [RequireComponent(typeof(Animator))]
    public class DooberElement : MonoBehaviour, IAnimationClipSource
    {
        [field: SerializeField]
        [field: Tooltip("Animation clip to play when spawning the doober")]
        public AnimationClip AnimIn { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Animation clip to play before despawning the doober")]
        public AnimationClip AnimOut { get; private set; }

        [SerializeField]
        private Image[] _images;

        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = (RectTransform) transform;
        }

        public void SetSprite(Sprite sprite)
        {
            foreach (var image in _images)
            {
                image.sprite = sprite;
            }
        }

        public static DooberElement Create(Transform parent)
        {
            var newGo = new GameObject("Doober", typeof(RectTransform));
            newGo.transform.SetParent(parent, false);
            var element = newGo.AddComponent<DooberElement>();
            var image = new GameObject("Image").AddComponent<Image>();
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.sizeDelta = Vector2.zero;
            image.transform.SetParent(newGo.transform, false);
            element._images = new[] {image};
            newGo.SetActive(false);
            return element;
        }

        public void GetAnimationClips(List<AnimationClip> results)
        {
            results.AddIfNotNull(AnimIn);
            results.AddIfNotNull(AnimOut);
        }
    }
}