using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class ListPool
    {
        public static CollectionPools.List<T> Get<T>() => CollectionPools.GetList<T>();
        public static CollectionPools.List<T> Get<T>(IEnumerable<T> content) => CollectionPools.GetList(content);
    }

    [PublicAPI]
    public static class HashSetPool
    {
        public static CollectionPools.HashSet<T> Get<T>() => CollectionPools.GetHashSet<T>();
        public static CollectionPools.HashSet<T> Get<T>(IEnumerable<T> content) => CollectionPools.GetHashSet(content);
    }

    [PublicAPI]
    public static class StackPool
    {
        public static CollectionPools.Stack<T> Get<T>() => CollectionPools.GetStack<T>();
    }

    [PublicAPI]
    public static class QueuePool
    {
        public static CollectionPools.Queue<T> Get<T>() => CollectionPools.GetQueue<T>();
    }

    [PublicAPI]
    public static class DictionaryPool
    {
        public static CollectionPools.Dictionary<TKey, TValue> Get<TKey, TValue>() => CollectionPools.GetDictionary<TKey, TValue>();
        public static CollectionPools.Dictionary<TKey, TValue> Get<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> content) => CollectionPools.GetDictionary(content);
        public static CollectionPools.Dictionary<TKey, TValue> Get<TKey, TValue>(IEnumerable<(TKey key, TValue value)> content) => CollectionPools.GetDictionary(content);
    }

    [PublicAPI]
    public static class PriorityQueuePool
    {
        public static CollectionPools.PriotityQueue<T> Get<T>() => CollectionPools.GetPriorityQueue<T>();
    }

    [PublicAPI]
    public static class CollectionPools
    {
        private static readonly System.Collections.Generic.Dictionary<Type, Func<object>> TypeCtor;
        private static readonly System.Collections.Generic.Dictionary<Type, System.Collections.Generic.Stack<object>> TypePool;

        static CollectionPools()
        {
            TypeCtor = new System.Collections.Generic.Dictionary<Type, Func<object>>();
            TypePool = new System.Collections.Generic.Dictionary<Type, System.Collections.Generic.Stack<object>>();
        }

        private static object GetCollection<T>()
        {
            var type = typeof(T);
            if (!TypePool.TryGetValue(type, out var pool))
            {
                pool = new System.Collections.Generic.Stack<object>(8);
                TypePool.Add(type, pool);
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            return pool.Count <= 0 ? TypeCtor[type].Invoke() : pool.Pop();
        }

        #region List

        [UnityEngine.Scripting.Preserve]
        public class List<T> : System.Collections.Generic.List<T>, IDisposable
        {
            internal bool Pooled { get; set; }

            static List() => TypeCtor.Add(typeof(List<T>), () => new List<T>());

            private List()
            {
            }

            public void Dispose() => ReturnList(this);
        }

        public static List<T> GetList<T>(IEnumerable<T> content)
        {
            var list = GetList<T>();
            list.AddRange(content);
            return list;
        }

        public static List<T> GetList<T>()
        {
            var list = (List<T>)GetCollection<List<T>>();
            list.Pooled = false;
            return list;
        }

        private static void ReturnList<T>(List<T> list)
        {
            if (list.Pooled)
            {
                return;
            }

            list.Pooled = true;
            list.Clear();
            TypePool[typeof(List<T>)].Push(list);
        }

        #endregion

        #region Hashset

        [UnityEngine.Scripting.Preserve]
        public class HashSet<T> : System.Collections.Generic.HashSet<T>, IDisposable
        {
            internal bool Pooled { get; set; }

            static HashSet() => TypeCtor.Add(typeof(HashSet<T>), () => new HashSet<T>());

            private HashSet()
            {
            }

            public void Dispose() => ReturnHashSet(this);
        }

        public static HashSet<T> GetHashSet<T>(IEnumerable<T> content)
        {
            var hashSet = GetHashSet<T>();
            foreach (var e in content)
            {
                hashSet.Add(e);
            }

            return hashSet;
        }

        public static HashSet<T> GetHashSet<T>()
        {
            var hashSet = (HashSet<T>)GetCollection<HashSet<T>>();
            hashSet.Pooled = false;
            return hashSet;
        }

        private static void ReturnHashSet<T>(HashSet<T> hashSet)
        {
            if (hashSet.Pooled)
            {
                return;
            }

            hashSet.Pooled = true;
            hashSet.Clear();
            TypePool[typeof(HashSet<T>)].Push(hashSet);
        }

        #endregion

        #region Stack

        [UnityEngine.Scripting.Preserve]
        public class Stack<T> : System.Collections.Generic.Stack<T>, IDisposable
        {
            internal bool Pooled { get; set; }

            static Stack() => TypeCtor.Add(typeof(Stack<T>), () => new Stack<T>());

            private Stack()
            {
            }

            public void Dispose() => ReturnStack(this);
        }

        public static Stack<T> GetStack<T>()
        {
            var stack = (Stack<T>)GetCollection<Stack<T>>();
            stack.Pooled = false;
            return stack;
        }

        private static void ReturnStack<T>(Stack<T> stack)
        {
            if (stack.Pooled)
            {
                return;
            }

            stack.Pooled = true;
            stack.Clear();
            TypePool[typeof(Stack<T>)].Push(stack);
        }

        #endregion

        #region Queue

        [UnityEngine.Scripting.Preserve]
        public class Queue<T> : System.Collections.Generic.Queue<T>, IDisposable
        {
            internal bool Pooled { get; set; }

            static Queue() => TypeCtor.Add(typeof(Queue<T>), () => new Queue<T>());

            private Queue()
            {
            }

            public void Dispose() => ReturnQueue(this);
        }

        public static Queue<T> GetQueue<T>()
        {
            var queue = (Queue<T>)GetCollection<Queue<T>>();
            queue.Pooled = false;
            return queue;
        }

        private static void ReturnQueue<T>(Queue<T> queue)
        {
            if (queue.Pooled)
            {
                return;
            }

            queue.Pooled = true;
            queue.Clear();
            TypePool[typeof(Queue<T>)].Push(queue);
        }

        #endregion

        #region Dictionary

        [UnityEngine.Scripting.Preserve]
        public class Dictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>, IDisposable
        {
            internal bool Pooled { get; set; }

            static Dictionary() => TypeCtor.Add(typeof(Dictionary<TKey, TValue>), () => new Dictionary<TKey, TValue>());

            private Dictionary()
            {
            }

            public void Dispose() => ReturnDictionary(this);
        }

        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> content)
        {
            var dict = GetDictionary<TKey, TValue>();
            dict.AddRange(content);
            return dict;
        }

        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(IEnumerable<(TKey key, TValue value)> content)
        {
            var dict = GetDictionary<TKey, TValue>();
            dict.AddRange(content);
            return dict;
        }

        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>()
        {
            var dict = (Dictionary<TKey, TValue>)GetCollection<Dictionary<TKey, TValue>>();
            dict.Pooled = false;
            return dict;
        }

        private static void ReturnDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict.Pooled)
            {
                return;
            }

            dict.Pooled = true;
            dict.Clear();
            TypePool[typeof(Dictionary<TKey, TValue>)].Push(dict);
        }

        #endregion

        #region PriorityQueue

        [UnityEngine.Scripting.Preserve]
        public class PriotityQueue<T> : System.Collections.Generic.Queue<T>, IDisposable
        {
            internal bool Pooled { get; set; }

            static PriotityQueue() => TypeCtor.Add(typeof(PriotityQueue<T>), () => new PriotityQueue<T>());

            private PriotityQueue()
            {
            }

            public void Dispose() => ReturnPriorityQueue(this);
        }

        public static PriotityQueue<T> GetPriorityQueue<T>()
        {
            var queue = (PriotityQueue<T>)GetCollection<PriotityQueue<T>>();
            queue.Pooled = false;
            return queue;
        }

        private static void ReturnPriorityQueue<T>(PriotityQueue<T> queue)
        {
            if (queue.Pooled)
            {
                return;
            }

            queue.Pooled = true;
            queue.Clear();
            TypePool[typeof(PriotityQueue<T>)].Push(queue);
        }

        #endregion
    }
}