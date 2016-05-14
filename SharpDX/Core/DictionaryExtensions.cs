using System;
using System.Collections.Generic;

namespace SharpDX.Core
{
    static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue)) {
            TValue value;
            if (dictionary.TryGetValue(key, out value)) return value;
            return defaultValue;
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> createEvent) {
            TValue value;
            if (dictionary.TryGetValue(key, out value)) return value;

            value = createEvent(key);
            dictionary.Add(key, value);
            return value;
        }
    }
}
