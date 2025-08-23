using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    public class GenericPool<T> : IPool<T>
    {
        /// <summary>
        /// Used to prevent null-checks internally
        /// </summary>
        private static readonly Action<T> DummyAction = delegate { };

        private readonly Func<T> _createFunc;
        private readonly Action<T> _disposeAction;

        private readonly Action<T> _poolAction;
        private readonly Action<T> _unpoolAction;

        private readonly Stack<T> _pooled;
        private readonly List<T> _unpooled;

        public IReadOnlyList<T> Unpooled => _unpooled;

        public GenericPool([NotNull] Func<T> createFunc, Action<T> disposeAction = null, Action<T> poolAction = null, Action<T> unpoolAction = null)
        {
            _createFunc = createFunc;
            _disposeAction = disposeAction ?? DummyAction;

            _poolAction = poolAction ?? DummyAction;
            _unpoolAction = unpoolAction ?? DummyAction;

            _pooled = new Stack<T>();
            _unpooled = new List<T>();
        }

        /// <summary>
        /// Retrieves all unpooled instances, runs the dispose action on all pooled instance,
        ///  and clears the pool
        /// </summary>
        public void ClearPool()
        {
            RetreiveAllUnpooled();
            foreach (var pooled in _pooled)
            {
                _disposeAction.Invoke(pooled);
            }

            _pooled.Clear();
        }

        /// <summary>
        /// Gets a pooled instance if one exists, creates one otherwise
        /// </summary>
        public T Get()
        {
            T instance;
            if (_pooled.Count > 0)
            {
                instance = _pooled.Pop();
                _unpoolAction.Invoke(instance);
            }
            else
            {
                instance = _createFunc.Invoke();
            }

            _unpooled.Add(instance);
            return instance;
        }

        /// <summary>
        /// Pools all objects that were created/unpooled from this pool
        /// </summary>
        public void RetreiveAllUnpooled(bool reverseLoop = false)
        {
            // A list copy is required otherwise it will be modifed during iteration
            using var unpooledCopy = ListPool.Get(_unpooled);
            if (reverseLoop)
            {
                for (var i = unpooledCopy.Count - 1; i >= 0; --i)
                {
                    Pool(unpooledCopy[i]);
                }
            }
            else
            {
                foreach (var unpooled in unpooledCopy)
                {
                    Pool(unpooled);
                }
            }
        }

        /// <summary>
        /// Returns an object to the pool for future use
        /// </summary>
        /// <param name="obj">The object to pool</param>
        public void Pool(T obj)
        {
            if (!_unpooled.Remove(obj))
            {
                Debug.LogError("Pooling an object that was not created from this pool");
            }

            _poolAction.Invoke(obj);
            _pooled.Push(obj);
        }
    }
}