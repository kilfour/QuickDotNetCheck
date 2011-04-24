using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using QuickDotNetCheck.Implementation;
using QuickGenerate.Writing;

namespace QuickDotNetCheck.ShrinkingStrategies
{
    public interface IManipulation
    {
        void Manipulate();
        void Reset();
        string Report();
        string[] Keys();
    }

    public static class Manipulate
    {
        public static Manipulator<TEntity> This<TEntity>(TEntity target)
        {
            return new Manipulator<TEntity>(target);
        }
    }

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
            object newValue
            )
        {
            manipulation.Add(target, expression, newValue);
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
            manipulations.Add(new Manipulation<TEntity>(target, expression, newValue));
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

    public class Manipulation<TEntity> : IManipulation
    { 
        private readonly TEntity target;
        private readonly object newValue;
        private readonly Func<TEntity, object> getter;
        private readonly Action<TEntity, object> setter;
        private readonly object originalValue;
        private readonly string name;

        public Manipulation(
            TEntity target,
            Expression<Func<TEntity, object>> expression,
            object newValue)
            : this(target, expression.AsPropertyInfo(), newValue) { }

        public Manipulation(
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
            return new[] {name};
        }
    }

    public class ManipulationStrategy : IShrinkingStrategy
    {
        private readonly Dictionary<Type, List<object>> simpleValues =
            new Dictionary<Type, List<object>>();

        private readonly List<IManipulation> manipulations = 
            new List<IManipulation>();

        private readonly List<PropertyInfo> propertiesToIgnore = new List<PropertyInfo>();

        private Dictionary<string, bool> shrunk;

        public ManipulationStrategy Add(params object[] values)
        {
            foreach (var value in values)
            {
                if (!simpleValues.ContainsKey(value.GetType()))
                    simpleValues[value.GetType()] = new List<object>();
                simpleValues[value.GetType()].Add(value);
            }
            return this;
        }

        public ManipulationStrategy AddNull<TProperty>()
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

        public ManipulationStrategy Add(IManipulation manipulation)
        {
            manipulations.Add(manipulation);
            return this;
        }

        public ManipulationStrategy Register<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var propertyInfo = propertyExpression.AsPropertyInfo();
            return Register(entity, propertyInfo);
        }

        private ManipulationStrategy Register<TEntity>(TEntity entity, PropertyInfo propertyInfo)
        {
            if (simpleValues.Keys.Any(key => propertyInfo.PropertyType.IsAssignableFrom(key)))
            {
                foreach (var simpleValue in simpleValues.First(pair => propertyInfo.PropertyType.IsAssignableFrom(pair.Key)).Value.ToArray())
                {
                    var manipulation = new Manipulation<TEntity>(entity, propertyInfo, simpleValue);
                    manipulations.Add(manipulation);
                }
            }
            return this;
        }

        public ManipulationStrategy Ignore<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            propertiesToIgnore.Add(propertyExpression.AsPropertyInfo());
            return this;
        }

        public ManipulationStrategy RegisterAll<TEntity>(TEntity entity)
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

            getter = t => property.GetValue(t, null);
            setter = (t, value) => property.SetValue(t, value, null);

            originalValue = getter(target);
        }

        public SimpleValuesShrinkingStrategy(
            TEntity target,
            PropertyInfo propertyInfo)
        {
            this.target = target;

            getter = t => propertyInfo.GetValue(t, null);
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
            simpleValues.AddRange(values);
        }

        public object OriginalValue()
        {
            return originalValue;
        }
    }
}
