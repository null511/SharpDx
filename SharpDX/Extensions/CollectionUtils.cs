using System;
using System.Collections.Generic;

namespace SharpDX.Extensions
{
    static class CollectionUtils
    {
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action) {
            foreach (var i in collection) action(i);
        }
    }
}
