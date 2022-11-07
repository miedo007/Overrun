using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Project.Extensions
{
    public static class ListExtensions
    {
        /// Shuffle the list in place using the Fisher-Yates method.
        [PublicAPI] public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        /// Return a random item from the list.
        [PublicAPI] public static T RandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new IndexOutOfRangeException("Cannot select a random item from an empty list");
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        /// Removes a random item from the list, returning that item.
        [PublicAPI] public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new IndexOutOfRangeException("Cannot remove a random item from an empty list");
            }
            var index = UnityEngine.Random.Range(0, list.Count);
            var item = list[index];
            list.RemoveAt(index);
            return item;
        }
        
        [PublicAPI] public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }
    }
}