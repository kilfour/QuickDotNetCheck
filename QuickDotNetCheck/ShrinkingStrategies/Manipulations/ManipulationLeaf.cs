using System;
using System.Linq.Expressions;
using System.Reflection;
using QuickDotNetCheck.Implementation;

namespace QuickDotNetCheck.ShrinkingStrategies.Manipulations
{
    public class ManipulationLeaf<TEntity> : IManipulation
    {
        private readonly TEntity target;
        private readonly object newValue;
        private readonly Func<TEntity, object> getter;
        private readonly Action<TEntity, object> setter;
        private readonly object originalValue;
        private readonly string name;

        public ManipulationLeaf(
            TEntity target,
            Expression<Func<TEntity, object>> expression,
            object newValue)
            : this(target, expression.AsPropertyInfo(), newValue) { }

        public ManipulationLeaf(
            TEntity target,
            PropertyInfo propertyInfo,
            object newValue)
        {
            this.target = target;
            this.newValue = newValue;
            getter = t => propertyInfo.GetValue(t, null);
            setter = (t, value) => propertyInfo.SetValue(t, value, null);
            originalValue = getter(target);
            name = string.Format("{0},{1}", target.GetType().Name, propertyInfo.Name);
        }

        public void Manipulate()
        {
            setter(target, newValue);
        }

        public void Reset()
        {
            setter(target, originalValue);
        }

        public string Report()
        {
            return name;
        }

        public string[] Keys()
        {
            return new[] { name };
        }
    }
}
