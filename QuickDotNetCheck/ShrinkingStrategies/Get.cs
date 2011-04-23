using System;
using System.Linq;

namespace QuickDotNetCheck.ShrinkingStrategies
{
    public static class Get
    {
        public static Getter<T> From<T>(T target)
        {
            return new Getter<T>(target);
        }
    }

    public class Getter<T>
    {
        private readonly T target;

        public Getter(T target)
        {
            this.target = target;
        }

        public TProperty[] All<TProperty>()
        {
            var properties =
                typeof(T).GetProperties()
                    .Where(p => p.PropertyType == typeof(TProperty));
            return
                properties
                    .Select(propertyInfo => (TProperty)propertyInfo.GetValue(target, null))
                    .ToArray();
        }

        public object[] AllValues()
        {
            var properties =
                typeof(T).GetProperties();

            return
                properties
                    .Select(propertyInfo => propertyInfo.GetValue(target, null))
                    .ToArray();
        }
    }
}