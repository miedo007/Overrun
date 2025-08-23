using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public interface IPool<T>
    {
        IReadOnlyList<T> Unpooled { get; }

        /// <summary>
        /// Retrieves all unpooled instances, runs the dispose action on all pooled instance,
        ///  and clears the pool
        /// </summary>
        void ClearPool();

        /// <summary>
        /// Gets a pooled instance if one exists, creates one otherwise
        /// </summary>
        T Get();

        /// <summary>
        /// Pools all objects that were created/unpooled from this pool
        /// </summary>
        void RetreiveAllUnpooled(bool reverseLoop = false);

        /// <summary>
        /// Returns an object to the pool for future use
        /// </summary>
        /// <param name="obj">The object to pool</param>
        void Pool([NotNull] T obj);
    }
}