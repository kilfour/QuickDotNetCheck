using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QuickDotNetCheck.Implementation;
using QuickDotNetCheck.ShrinkingStrategies.Manipulations;
using QuickGenerate.Writing;

namespace QuickDotNetCheck.ShrinkingStrategies
{
    public class ShrinkingStrategy : IShrinkingStrategy
    {
        private readonly Dictionary<Type, List<object>> simpleValues =
            new Dictionary<Type, List<object>>();

        private readonly List<IManipulation> manipulations = 
            new List<IManipulation>();

        private readonly List<PropertyInfo> propertiesToIgnore = new List<PropertyInfo>();

        private Dictionary<string, bool> shrunk;

        public bool Shrunk<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var key = string.Format("{0},{1}", entity.GetType().Name, propertyExpression.AsPropertyInfo().Name);
            return shrunk[key];
        }

        public ShrinkingStrategy Add(params object[] values)
        {
            foreach (var value in values)
            {
                if (!simpleValues.ContainsKey(value.GetType()))
                    simpleValues[value.GetType()] = new List<object>();
                simpleValues[value.GetType()].Add(value);
            }
            return this;
        }

        public ShrinkingStrategy AddNull<TProperty>()
        {
            if (!simpleValues.ContainsKey(typeof(TProperty)))
                simpleValues[typeof(TProperty)] = new List<object>();
            simpleValues[typeof(TProperty)].Add(null);
            return this;
        }

        public void Shrink(Func<bool> runFunc)
        {
            shrunk = manipulations.SelectMany(m => m.Keys()).Distinct().ToDictionary(k => k, k => false);
            foreach (var manipulation in manipulations)
            {
                manipulation.Manipulate();
                if(runFunc())
                {
                    foreach (var key in manipulation.Keys())
                    {
                        shrunk[key] = true;
                    }
                }
                manipulation.Reset();
            }
        }

        public ShrinkingStrategy Add(IManipulation manipulation)
        {
            manipulations.Add(manipulation);
            return this;
        }

        public ShrinkingStrategy Register<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var propertyInfo = propertyExpression.AsPropertyInfo();
            return Register(entity, propertyInfo);
        }

        private ShrinkingStrategy Register<TEntity>(TEntity entity, PropertyInfo propertyInfo)
        {
            if (simpleValues.Keys.Any(key => propertyInfo.PropertyType.IsAssignableFrom(key)))
            {
                foreach (var simpleValue in simpleValues.First(pair => propertyInfo.PropertyType.IsAssignableFrom(pair.Key)).Value.ToArray())
                {
                    var manipulation = new ManipulationLeaf<TEntity>(entity, propertyInfo, simpleValue);
                    manipulations.Add(manipulation);
                }
            }
            return this;
        }

        public ShrinkingStrategy Ignore<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            propertiesToIgnore.Add(propertyExpression.AsPropertyInfo());
            return this;
        }

        public ShrinkingStrategy RegisterAll<TEntity>(TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (!propertiesToIgnore.Contains(propertyInfo))
                    Register(entity, propertyInfo);
            }
            return this;
        }

        public bool Shrunk()
        {
            return shrunk.All(kv => kv.Value);
        }

        public string Report()
        {
            var stream = new StringStream();
            if (Shrunk())
            {
                stream.Write("Any input. ");
            }
            else
            {
                foreach (var key in shrunk.Where(kv => !kv.Value).Select(kv => kv.Key))
                {
                    stream.Write(key);
                    stream.WriteLine();
                }

                stream.Write("Shrunken :");
                stream.WriteLine();
                foreach (var key in shrunk.Where(kv => kv.Value).Select(kv => kv.Key))
                {
                    stream.Write(key);
                    stream.WriteLine();
                }
            }

            return stream.ToReader().ReadToEnd();
        }

        
    }
}