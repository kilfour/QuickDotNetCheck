using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using QuickDotNetCheck.Implementation;

namespace QuickDotNetCheck
{
    public class ListShrinkingStrategy<TEntity, TProperty>
    {
        private readonly TEntity target;

        private readonly Func<TEntity, IList<TProperty>> getter;

        private readonly TProperty[] simpleValues;

        public ListShrinkingStrategy(
            TEntity target,
            Expression<Func<TEntity, IList<TProperty>>> expression,
            TProperty[] simpleValues)
        {
            this.target = target;

            var property = expression.AsPropertyInfo();

            getter = t => (IList<TProperty>)property.GetValue(t, null);

            this.simpleValues = simpleValues;
        }

        public IList<TProperty> Result { get; set; }

        public void Shrink(Func<bool> runFunc)
        {
            var theList = getter(target);
            int index = 0;
            while(index < theList.Count)
            {
                int ix = index;

                var strategy =
                    new SimpleValuesShrinkingStrategy<IList<TProperty>, TProperty>(
                        theList,
                        t => t[ix],
                        (t,i) => t[ix] = i,
                        simpleValues);

                strategy.Shrink(runFunc);

                if (strategy.Shrunk())
                    theList.RemoveAt(index);
                else
                    index++;
            }
            Result = theList;
        }
    }
}
