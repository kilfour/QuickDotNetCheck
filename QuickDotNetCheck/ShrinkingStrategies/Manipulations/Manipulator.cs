using System;
using System.Linq.Expressions;

namespace QuickDotNetCheck.ShrinkingStrategies.Manipulations
{
    public class Manipulator<TEntity> : IManipulation
    {
        private readonly Manipulation manipulation;
        private readonly TEntity target;

        public Manipulator(TEntity target)
        {
            this.target = target;
            manipulation = new Manipulation();
        }

        public Manipulator<TEntity> Change(
            Expression<Func<TEntity, object>> expression,
            params object[] newValues)
        {
            manipulation.Add(target, expression, newValues);
            return this;
        }

        public void Manipulate()
        {
            manipulation.Manipulate();
        }

        public void Reset()
        {
            manipulation.Reset();
        }

        public string Report()
        {
            return manipulation.Report();
        }

        public string[] Keys()
        {
            return manipulation.Keys();
        }
    }
}