using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtl.Toolbox
{
    /// <summary>
    /// A Queue class in which each item is associated with a Double value
    /// representing the item's priority. 
    /// Dequeue and Peek functions return item with the best priority value.
    /// </summary>
    [PublicAPI]
    public class PriorityQueue<T> : IEnumerable<T>
    {
        private readonly List<Tuple<T, double>> _elements = new List<Tuple<T, double>>();

        private int _version;

        /// <summary>
        /// Return the total number of elements currently in the Queue.
        /// </summary>
        /// <returns>Total number of elements currently in Queue</returns>
        public int Count => _elements.Count;

        /// <summary>
        /// Add given item to Queue and assign item the given priority value.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        /// <param name="priorityValue">Item priority value as Double.</param>
        public void Enqueue(T item, double priorityValue)
        {
            var index = _elements.Count;
            for (; index > 0; --index)
            {
                var check = _elements[index - 1];
                if (check.Item2 < priorityValue)
                {
                    break;
                }
            }

            _elements.Insert(index, Tuple.Create(item, priorityValue));
            ++_version;
        }

        /// <summary>
        /// Return lowest priority value item and remove item from Queue.
        /// </summary>
        /// <returns>Queue item with lowest priority value.</returns>
        public T Dequeue()
        {
            if (_elements.Count <= 0)
            {
                throw new InvalidOperationException("The queue is empty");
            }

            var item = _elements[0].Item1;
            _elements.RemoveAt(0);
            ++_version;
            return item;
        }

        /// <summary>
        /// Return lowest priority value item without removing item from Queue.
        /// </summary>
        /// <returns>Queue item with lowest priority value.</returns>
        public T Peek() => _elements.Count <= 0 ? throw new InvalidOperationException("The queue is empty") : _elements[0].Item1;

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<T>
        {
            private readonly PriorityQueue<T> _queue;
            private readonly int _version;

            private int _index;

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public Enumerator(PriorityQueue<T> queue)
            {
                _queue = queue;
                _index = 0;
                _version = _queue._version;
                Current = default;
            }

            public bool MoveNext()
            {
                if (_version != _queue._version)
                {
                    throw new InvalidOperationException("The queue was modified during iteration");
                }

                if (_index >= _queue.Count)
                {
                    Current = default;
                    return false;
                }

                Current = _queue._elements[_index].Item1;
                ++_index;
                return true;
            }

            public void Reset()
            {
                if (_version != _queue._version)
                {
                    throw new InvalidOperationException("The queue was modified during iteration");
                }

                _index = 0;
                Current = default;
            }

            public void Dispose()
            {
            }
        }
    }
}