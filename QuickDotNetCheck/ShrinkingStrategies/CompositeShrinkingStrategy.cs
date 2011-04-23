using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QuickDotNetCheck.Implementation;
using QuickGenerate.Writing;

namespace QuickDotNetCheck.ShrinkingStrategies
{
    public class CompositeShrinkingStrategy<TEntity> : IShrinkingStrategy
    {
        private readonly TEntity entity;

        private readonly Dictionary<PropertyInfo, ISimpleValuesShrinkingStrategy> shrinkingStrategies =
            new Dictionary<PropertyInfo, ISimpleValuesShrinkingStrategy>();

        public CompositeShrinkingStrategy(TEntity entity)
        {
            this.entity = entity;
        }

        public void Shrink(Func<bool> runFunc)
        {
            foreach (var shrinkingStrategy in shrinkingStrategies.Values)
            {
                shrinkingStrategy.Shrink(runFunc);
            }
        }

        public bool Shrunk()
        {
            return shrinkingStrategies.Values.All(s => s.Shrunk());
        }

        public CompositeShrinkingStrategy<TEntity> Register<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var propertyInfo = propertyExpression.AsPropertyInfo();
            if (!shrinkingStrategies.ContainsKey(propertyInfo))
                shrinkingStrategies.Add(propertyInfo, new SimpleValuesShrinkingStrategy<TEntity, TProperty>(entity, propertyExpression));
            if (simpleValues.Keys.Any(key => typeof(TProperty).IsAssignableFrom(key)))
            {
                var shrinkingStrategy = shrinkingStrategies[propertyInfo];
                shrinkingStrategy.AddValues(simpleValues.First(pair => typeof(TProperty).IsAssignableFrom(pair.Key)).Value.ToArray());
            }
            return this;
        }

        public CompositeShrinkingStrategy<TEntity> RegisterAll()
        {
            return this;
        }

        public CompositeShrinkingStrategy<TEntity> AddValuesFor<TProperty>(
            Expression<Func<TEntity, TProperty>> propertyExpression, 
            params TProperty[] values)
        {
            Register(propertyExpression);
            var shrinkingStrategy = shrinkingStrategies[propertyExpression.AsPropertyInfo()];
            shrinkingStrategy.AddValues(values.Cast<object>().ToArray());
            return this;
        }

        public IShrinkingStrategy StrategyFor<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            return shrinkingStrategies[propertyExpression.AsPropertyInfo()];
        }

        private readonly Dictionary<Type, List<object>> simpleValues =
            new Dictionary<Type, List<object>>();

        public CompositeShrinkingStrategy<TEntity> Add(params object[] values)
        {
            foreach (var value in values)
            {
                if (!simpleValues.ContainsKey(value.GetType()))
                    simpleValues[value.GetType()] = new List<object>();
                simpleValues[value.GetType()].Add(value);
            }
            return this;
        }

        public string Report()
        {
            var stream = new StringStream();
            foreach (var shrinkingStrategy in shrinkingStrategies)
            {
                stream.Write(shrinkingStrategy.Key.Name);
                stream.Write(" : ");
                if (shrinkingStrategy.Value.Shrunk())
                    stream.Write("-");
                else
                    stream.Write(shrinkingStrategy.Value.OriginalValue().ToString());
                stream.Write(" .");
                stream.WriteLine();
            }
            return stream.ToReader().ReadToEnd();
        }
    }
}