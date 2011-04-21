using System;
using System.Collections.Generic;

namespace QuickDotNetCheck.Implementation
{
    public static class EnumerableForEachExtension
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
            return enumerable;
        }
    }
}