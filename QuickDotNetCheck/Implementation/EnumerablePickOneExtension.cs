using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickDotNetCheck.Implementation
{
    public static class EnumerablePickOneExtension
    {
        private static readonly Random random = new Random();

        public static T PickOne<T>(this IEnumerable<T> choices)
        {
            var count = choices.Count();
            if (count == 0)
                throw new InvalidOperationException("Can't choose something from an empty collection.");
            return choices.ElementAt(random.Next(0, count));
        }
    }
}