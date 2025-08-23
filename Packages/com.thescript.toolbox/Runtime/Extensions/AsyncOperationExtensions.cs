using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class AsyncOperationExtensions
    {
        public static MultiAsyncOperation WaitForAll(this IEnumerable<AsyncOperation> operations) => new MultiAsyncOperation(operations);
    }
}