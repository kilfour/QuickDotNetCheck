using System;
using System.Collections.Generic;
using System.Linq;
using QuickDotNetCheck.Implementation;

namespace QuickDotNetCheck.ShrinkingStrategies
{
    public static class Simple
    {
        private static readonly Dictionary<Type, object[]> simpleValues =
            new Dictionary<Type, object[]>
                {
                    {typeof(string), new object[]{""}},
                    {typeof(int), new object[]{-1, 0 ,1}},
                    {typeof(DateTime), new object[]{1.January(2010), 31.December(1970)}},
                };

        public static object[] Values<T>()
        {
            if (!simpleValues.ContainsKey(typeof(T)))
                throw new InvalidOperationException(string.Format("No simple values for Type : '{0}'.", typeof(T).Name));
            return simpleValues[typeof(T)].ToArray();
        }

        public static object[] AllValues()
        {
            return simpleValues.SelectMany(values => values.Value).ToArray();
        }
    }
}