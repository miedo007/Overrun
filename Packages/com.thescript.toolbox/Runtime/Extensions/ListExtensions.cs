using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class ListExtensions
    {
        private static readonly Random Random = new Random();

        public static bool AddUniqueIfNotNull<T>([NotNull] this IList<T> list, T element) where T : class
        {
            if (element == null || list.Contains(element))
            {
                return false;
            }

            list.Add(element);
            return true;
        }

        public static bool AddUnique<T>([NotNull] this IList<T> list, T element) where T : class
        {
            if (list.Contains(element))
            {
                return false;
            }

            list.Add(element);
            return true;
        }

        public static bool AddIfNotNull<T>([NotNull] this IList<T> list, T element) where T : class
        {
            if (element == null)
            {
                return false;
            }

            list.Add(element);
            return true;
        }

        public static T GetRandomElement<T>([NotNull] this IList<T> list, Random random = null) => list[(random ?? Random).Next(list.Count)];

        public static void SwapElements<T>([NotNull] this IList<T> list, T fromElement, T toElement)
        {
            var fromIndex = list.IndexOf(fromElement);
            if (fromIndex < 0 || fromIndex >= list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            var toIndex = list.IndexOf(toElement);
            if (toIndex < 0 || toIndex >= list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            (list[fromIndex], list[toIndex]) = (list[toIndex], list[fromIndex]);
        }

        public static void SwapElementsAt<T>([NotNull] this IList<T> list, int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= list.Count ||
                toIndex < 0 || toIndex >= list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            (list[fromIndex], list[toIndex]) = (list[toIndex], list[fromIndex]);
        }
    }
}