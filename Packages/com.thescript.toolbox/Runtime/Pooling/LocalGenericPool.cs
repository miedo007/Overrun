using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mtl.Toolbox
{
    /// <summary>
    /// Local version of a pool that fetches from a parent pool to populate itself
    /// </summary>
    public class LocalGenericPool<T> : IPool<T>
    {
        /// <summary>
        /// Used to prevent null-checks internally
        /// </summary>
        private static readonly Action<T> DummyAction = delegate { };
        
        private readonly IPool<T> _sourcePool;
        private readonly List<T> _unpooled;

        private readonly Action<T> _poolAction;
        private readonly Action<T> _unpoolAction;
        
        public IReadOnlyList<T> Unpooled => _unpooled;

        public LocalGenericPool(IPool<T> sourcePool, Action<T> poolAction = null, Action<T> unpoolAction = null)
        {
            _sourcePool = sourcePool;
            _unpooled = new List<T>();

            _poolAction = poolAction ?? DummyAction;
            _unpoolAction = unpoolAction ?? DummyAction;
        }

        /// <summary>
        /// Retrieves all unpooled instances, runs the dispose action on all pooled instance,
        ///  and clears the pool
        /// </summary>
        public void ClearPool() => RetreiveAllUnpooled();

        /// <summary>
        /// Gets a pooled instance if one exists, creates one otherwise
        /// </summary>
        public T Get()
        {
            var newInst = _sourcePool.Get();
            _unpooled.Add(newInst);
            _unpoolAction.Invoke(newInst);
            return newInst;
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
            _sourcePool.Pool(obj);
        }
    }
}