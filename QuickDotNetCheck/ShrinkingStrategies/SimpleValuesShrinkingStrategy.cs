using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using QuickDotNetCheck.Implementation;

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
