using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mtl.Toolbox
{
    /// <summary>
    /// Custom Yield Instruction that wraps multiple Async Operations together
    /// </summary>
    public class MultiAsyncOperation : CustomYieldInstruction
    {
        private event Action OnCompletedInternal;

        public event Action OnCompleted
        {
            add
            {
                if (_completed)
                {
                    value?.Invoke();
                }
                else
                {
                    OnCompletedInternal += value;
                }
            }
            remove => OnCompletedInternal -= value;
        }

        private readonly List<AsyncOperation> _remainingOperations;
        private bool _completed;

        public override bool keepWaiting => !_completed;

        public MultiAsyncOperation(IEnumerable<AsyncOperation> operations)
        {
            _remainingOperations = new List<AsyncOperation>(operations);
            if (_remainingOperations.Count <= 0)
            {
                _completed = true;
                return;
            }

            foreach (var op in _remainingOperations)
            {
                op.completed += OperationComplete;
            }
        }

        private void OperationComplete(AsyncOperation op)
        {
            if (_remainingOperations.Remove(op) && _remainingOperations.Count <= 0)
            {
                _completed = true;
                OnCompletedInternal?.Invoke();
                OnCompletedInternal = null;
            }
        }
    }
}