using System;
using System.Linq.Expressions;
using QuickDotNetCheck.Implementation;

namespace QuickDotNetCheck
{
    public class SimpleValuesShrinkingStrategy<TEntity, TProperty>
    {
        private readonly TEntity target;

        private readonly Func<TEntity, TProperty> getter;
        private readonly Action<TEntity, TProperty> setter;

        private readonly TProperty[] simpleValues;

        public SimpleValuesShrinkingStrategy(
            TEntity target,
            Func<TEntity, TProperty> getter,
            Action<TEntity, TProperty> setter,
            TProperty[] simpleValues)
        {
            this.target = target;
            this.getter = getter;
            this.setter = setter;
            this.simpleValues = simpleValues;
        }

        public SimpleValuesShrinkingStrategy(
            TEntity target,
            Expression<Func<TEntity, TProperty>> expression, 
            TProperty[] simpleValues)
        {
            this.target = target;

            var property = expression.AsPropertyInfo();
            
            getter = t => (TProperty)property.GetValue(t, null);
            setter = (t, value) => property.SetValue(t, value, null);

            this.simpleValues = simpleValues;
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
    }
}
