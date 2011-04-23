using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QuickDotNetCheck.Implementation;

namespace QuickDotNetCheck.ShrinkingStrategies
{
    public interface ISimpleValuesShrinkingStrategy : IShrinkingStrategy
    {
        void AddValues(object[] values);
        object OriginalValue();
    }

    public class SimpleValuesShrinkingStrategy<TEntity> : ISimpleValuesShrinkingStrategy
    {
        private readonly TEntity target;

        private readonly Func<TEntity, object> getter;
        private readonly Action<TEntity, object> setter;

        private readonly List<object> simpleValues = new List<object>();

        private readonly object originalValue;

        public SimpleValuesShrinkingStrategy(
            TEntity target,
            Func<TEntity, object> getter,
            Action<TEntity, object> setter)
        {
            this.target = target;
            this.getter = getter;
            this.setter = setter;
        }

        public SimpleValuesShrinkingStrategy(
            TEntity target,
            Expression<Func<TEntity, object>> expression)
        {
            this.target = target;

            var property = expression.AsPropertyInfo();

            getter = t => (object)property.GetValue(t, null);
            setter = (t, value) => property.SetValue(t, value, null);

            originalValue = getter(target);
        }

        public SimpleValuesShrinkingStrategy(
            TEntity target,
            PropertyInfo propertyInfo)
        {
            this.target = target;

            getter = t => (object)propertyInfo.GetValue(t, null);
            setter = (t, value) => propertyInfo.SetValue(t, value, null);

            originalValue = getter(target);
        }

        public void Shrink(Func<bool> runFunc)
        {
            shrunk = true;
            var lastValue = getter(target);
            foreach (var value in simpleValues)
            {
                setter(target, value);
                shrunk = shrunk && runFunc();
            }
            setter(target, lastValue);
        }

        private bool shrunk;
        public bool Shrunk()
        {
            return shrunk;
        }

        public void AddValues(object[] values)
        {
            simpleValues.AddRange(values.Cast<object>());
        }

        public object OriginalValue()
        {
            return originalValue;
        }
    }
}
