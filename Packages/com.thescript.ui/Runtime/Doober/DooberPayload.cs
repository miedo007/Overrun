using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.UI
{
    [PublicAPI]
    public class DooberPayload<TOrigin, TTarget> : DooberPayload
    {
        public DooberPayload(Sprite sprite, Vector2 size, TOrigin origin, TTarget target) :
            base(sprite, size, origin, target, typeof(TOrigin), typeof(TTarget)) { }

        public new DooberPayload<TOrigin, TTarget> SetOriginCamera(Camera camera)
        {
            base.SetOriginCamera(camera);
            return this;
        }

        public new DooberPayload<TOrigin, TTarget> SetTargetCamera(Camera camera)
        {
            base.SetTargetCamera(camera);
            return this;
        }

        public new DooberPayload<TOrigin, TTarget> SetPrefab(DooberElement prefab)
        {
            base.SetPrefab(prefab);
            return this;
        }
    }

    [PublicAPI]
    public class DooberPayload
    {
        /// <summary>
        /// The sprite to display in the doober
        /// </summary>
        public readonly Sprite Sprite;

        /// <summary>
        /// The UI Size of the doober
        /// </summary>
        public readonly Vector2 Size;

        /// <summary>
        /// The object from which to start the doober
        /// </summary>
        public readonly object Origin;

        /// <summary>
        /// The object to target with the doober
        /// </summary>
        public readonly object Target;

        /// <summary>
        /// The type of the origin object (used to get IDooberPositionProvider&lt;T&gt;)
        /// </summary>
        public readonly Type OriginType;

        /// <summary>
        /// The type of the target object (used to get IDooberPositionProvider&lt;T&gt;)
        /// </summary>
        public readonly Type TargetType;

        /// <summary>
        /// The camera used to get the position of the origin object
        /// </summary>
        public Camera OriginCamera { get; set; }

        /// <summary>
        /// The camera used to get the position of the target object
        /// </summary>
        public Camera TargetCamera { get; set; }

        /// <summary>
        /// If set, this prefab will be used instead of the default one
        /// </summary>
        public DooberElement Prefab { get; set; }

        /// <summary>
        /// Called when the doober reaches the target and provides the completion percentage [0-1]
        /// </summary>
        public Action<float> DooberReachedTarget { get; set; }

        /// <summary>
        /// Called when the doober has completed fully
        /// </summary>
        public Action DooberCompleted { get; set; }

        /// <summary>
        /// Called if the doober is cancelled while executing
        /// </summary>
        public Action DooberCancelled { get; set; }

        protected DooberPayload(Sprite sprite, Vector2 size, object origin, object target, Type originType, Type targetType)
        {
            Sprite = sprite;
            Size = size;
            Origin = origin;
            OriginType = originType;
            Target = target;
            TargetType = targetType;
        }

        public DooberPayload(Sprite sprite, Vector2 size, object origin, object target) :
            this(sprite, size, origin, target, origin.GetType(), target.GetType()) { }

        public DooberPayload SetOriginCamera(Camera camera)
        {
            OriginCamera = camera;
            return this;
        }

        public DooberPayload SetTargetCamera(Camera camera)
        {
            TargetCamera = camera;
            return this;
        }

        public DooberPayload SetPrefab(DooberElement prefab)
        {
            Prefab = prefab;
            return this;
        }
    }
}