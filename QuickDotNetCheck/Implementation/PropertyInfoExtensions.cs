using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QuickDotNetCheck.Implementation
{
    public static class PropertyInfoExtensions
    {
        public static MemberExpression AsMemberExpression<TTarget, TExpression>(this Expression<Func<TTarget, TExpression>> expression)
        {
            if (expression.Body is UnaryExpression)
            {
                // WHY: expressions that target value types store the MemberExpression in another place than reference types
                return ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return expression.Body as MemberExpression;
        }

        public static PropertyInfo AsPropertyInfo<TTarget, TExpression>(this Expression<Func<TTarget, TExpression>> expression)
        {
            return expression.AsMemberExpression().Member as PropertyInfo;
        }

        public static Func<T, TProperty> GetValueGetter<T, TProperty>(this PropertyInfo propertyInfo)
        {
            if (typeof(T) != propertyInfo.DeclaringType)
            {
                throw new ArgumentException();
            }

            var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var property = Expression.Property(instance, propertyInfo);
            var convert = Expression.TypeAs(property, typeof(object));
            return (Func<T, TProperty>)Expression.Lambda(convert, instance).Compile();
        }

        public static Action<T, TProperty> GetValueSetter<T, TProperty>(this PropertyInfo propertyInfo)
        {
            if (typeof(T) != propertyInfo.DeclaringType)
            {
                throw new ArgumentException();
            }

            var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(
                instance,
                propertyInfo.GetSetMethod(),
                Expression.Convert(argument, propertyInfo.PropertyType));
            return (Action<T, TProperty>)Expression.Lambda(setterCall, instance, argument).Compile();
        }
    }
}