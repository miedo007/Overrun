using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class DictionaryExtensions
    {
        private static readonly Random Random = new Random();

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) =>
            dict.TryGetValue(key, out var val) ? val : default;

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> content)
        {
            foreach (var (key, value) in content)
            {
                dict.Add(key, value);
            }
        }
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<(TKey key, TValue value)> content)
        {
            foreach (var (key, value) in content)
            {
                dict.Add(key, value);
            }
        }

        public static KeyValuePair<TKey, TValue> GetRandomElement<TKey, TValue>(this IDictionary<TKey, TValue> dict, Random random = null) =>
            dict.ElementAt((random ?? Random).Next(dict.Count));

        public static TKey GetRandomKey<TKey, TValue>(this IDictionary<TKey, TValue> dict) =>
            dict.GetRandomElement().Key;

        public static TValue GetRandomValue<TKey, TValue>(this IDictionary<TKey, TValue> dict) =>
            dict.GetRandomElement().Value;

        public static KeyValuePair<TKey, double> GetWeightedRandomElement<TKey>(this IDictionary<TKey, double> dict, Random random = null)
        {
            using var tempList = ListPool.Get(dict);
            var total = tempList.Sum(kvp => kvp.Value);
            var val = (random ?? Random).NextDouble() * total;

            for (int i = 0, iMax = tempList.Count; i < iMax; ++i)
            {
                var kvp = tempList[i];
                if (val <= kvp.Value)
                {
                    return kvp;
                }

                val -= kvp.Value;
            }

            return tempList[tempList.Count - 1];
        }

        public static TKey GetWeightedRandomKey<TKey>(this IDictionary<TKey, double> dict) =>
            dict.GetWeightedRandomElement().Key;

        public static KeyValuePair<TKey, float> GetWeightedRandomElement<TKey>(this IDictionary<TKey, float> dict, Random random = null)
        {
            using var tempList = ListPool.Get(dict);
            var total = tempList.Sum(kvp => kvp.Value);
            var val = (random ?? Random).NextDouble() * total;

            for (int i = 0, iMax = tempList.Count; i < iMax; ++i)
            {
                var kvp = tempList[i];
                if (val <= kvp.Value)
                {
                    return kvp;
                }

                val -= kvp.Value;
            }

            return tempList[tempList.Count - 1];
        }

        public static TKey GetWeightedRandomKey<TKey>(this IDictionary<TKey, float> dict) =>
            dict.GetWeightedRandomElement().Key;

        public static KeyValuePair<TKey, int> GetWeightedRandomElement<TKey>(this IDictionary<TKey, int> dict, Random random = null)
        {
            using var tempList = ListPool.Get(dict);
            var total = tempList.Sum(kvp => kvp.Value);
            var val = (random ?? Random).NextDouble() * total;

            for (int i = 0, iMax = tempList.Count; i < iMax; ++i)
            {
                var kvp = tempList[i];
                if (val <= kvp.Value)
                {
                    return kvp;
                }

                val -= kvp.Value;
            }

            return tempList[tempList.Count - 1];
        }

        public static TKey GetWeightedRandomKey<TKey>(this IDictionary<TKey, int> dict) =>
            dict.GetWeightedRandomElement().Key;
    }
}