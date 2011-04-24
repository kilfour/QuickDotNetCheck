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
        
        private readonly List<PropertyInfo> propertiesToIgnore = new List<PropertyInfo>();

        private readonly Dictionary<Type, List<object>> simpleValues =
            new Dictionary<Type, List<object>>();

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
            return Register(propertyInfo);
        }

        private CompositeShrinkingStrategy<TEntity> Register(PropertyInfo propertyInfo)
        {
            if (!shrinkingStrategies.ContainsKey(propertyInfo))
                shrinkingStrategies.Add(propertyInfo, new SimpleValuesShrinkingStrategy<TEntity>(entity, propertyInfo));
            if (simpleValues.Keys.Any(key => propertyInfo.PropertyType.IsAssignableFrom(key)))
            {
                var shrinkingStrategy = shrinkingStrategies[propertyInfo];
                shrinkingStrategy.AddValues(simpleValues.First(pair => propertyInfo.PropertyType.IsAssignableFrom(pair.Key)).Value.ToArray());
            }
            return this;
        }
        public CompositeShrinkingStrategy<TEntity> Ignore<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            propertiesToIgnore.Add(propertyExpression.AsPropertyInfo());
            return this;
        }

        public CompositeShrinkingStrategy<TEntity> RegisterAll()
        {
            var properties = typeof(TEntity).GetProperties();
            foreach (var propertyInfo in properties)
            {
                if(!propertiesToIgnore.Contains(propertyInfo))
                    Register(propertyInfo);
            }
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

        public CompositeShrinkingStrategy<TEntity> AddNull<TProperty>()
        {
            if (!simpleValues.ContainsKey(typeof(TProperty)))
                simpleValues[typeof(TProperty)] = new List<object>();
            simpleValues[typeof(TProperty)].Add(null);
            return this;
        }

        public string Report()
        {
            var stream = new StringStream();
            if(Shrunk())
            {
                stream.Write("Any ");
                stream.Write(typeof(TEntity).Name);
                stream.Write(".");
            }
            else
            {
                stream.Write("A(n) ");
                stream.Write(typeof(TEntity).Name);
                stream.Write(" where :");
                stream.WriteLine();
                foreach (var shrinkingStrategy in shrinkingStrategies)
                {
                    if (shrinkingStrategy.Value.Shrunk())
                        continue;
                    stream.Write(shrinkingStrategy.Key.Name);
                    stream.Write(" == ");
                    stream.Write(shrinkingStrategy.Value.OriginalValue().ToString());
                    stream.WriteLine();
                }   
            }
            
            return stream.ToReader().ReadToEnd();
        }
    }
}