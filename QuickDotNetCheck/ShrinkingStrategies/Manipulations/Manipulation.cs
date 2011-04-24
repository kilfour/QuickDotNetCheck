using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QuickDotNetCheck.ShrinkingStrategies.Manipulations
{
    public class Manipulation : IManipulation
    {
        private readonly List<IManipulation> manipulations =
            new List<IManipulation>();

        public Manipulation Add(IManipulation manipulation)
        {
            manipulations.Add(manipulation);
            return this;
        }

        public Manipulation Add<TEntity>(
            TEntity target,
            Expression<Func<TEntity, object>> expression,
            object newValue)
        {
            manipulations.Add(new ManipulationLeaf<TEntity>(target, expression, newValue));
            return this;
        }

        public void Manipulate()
        {
            foreach (var manipulation in manipulations)
            {
                manipulation.Manipulate();
            }
        }

        public void Reset()
        {
            foreach (var manipulation in manipulations)
            {
                manipulation.Reset();
            }
        }

        public string Report()
        {
            var sb = new StringBuilder();
            foreach (var manipulation in manipulations)
            {
                sb.AppendLine(manipulation.Report());
            }
            return sb.ToString();
        }

        public string[] Keys()
        {
            return manipulations.SelectMany(manipulation => manipulation.Keys()).ToArray();
        }
    }
}