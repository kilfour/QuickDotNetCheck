using System;
using System.Collections.Generic;
using System.Linq;
using QuickDotNetCheck.Exceptions;
using QuickDotNetCheck.Implementation;
using QuickDotNetCheck.NotInTheRoot;

namespace QuickDotNetCheck
{
    public abstract class Fixture : IFixture
    {
        private readonly Dictionary<Spec, FactInfo> testMethods;
        private IEnumerable<Spec> factsToCheck;

        protected Fixture()
        {
            testMethods = new Dictionary<Spec, FactInfo>();
            var specs = GetType()
                .GetMethods()
                .Where(mi => mi.HasAttribute<SpecAttribute>());

            foreach (var mi in specs)
            {
                var action = (Action)Delegate.CreateDelegate(typeof (Action), this, mi);
                var spec = new Spec(action) { Name = GetType().Name + "." + mi.Name};
                testMethods[spec] = new FactInfo { Name = spec.Name };
            }

            specs = GetType()
                .GetMethods()
                .Where(mi => mi.ReturnType == typeof(Spec));

            foreach (var mi in specs)
            {
                var action = (Func<Spec>)Delegate.CreateDelegate(typeof(Func<Spec>), this, mi);
                var spec = action();
                spec.Name = GetType().Name + "." + mi.Name;
                testMethods[spec] = new FactInfo { Name = spec.Name };
            }
        }

        public IEnumerable<string> SpecNames()
        {
            return testMethods.Values.Select(v => v.Name);
        }

        public virtual void Arrange() { }
        public virtual bool CanAct() { return true; }
        public virtual void BeforeAct() { }
        public virtual void AfterAct() { }
        protected virtual void Act() { }

        public void Execute()
        {
            BeforeAct();
            FilterOutSpecsWithFailingPrecondition();
            Act();
            FilterOutSpecsWithFailingPostcondition();
            AfterAct();
        }

        public IDictionary<string, int> AssertSpecs()
        {
            var result = new Dictionary<string, int>();
            foreach (var spec in factsToCheck)
            {
                try
                {
                    result.Add(spec.Name, spec.Verify());
                    testMethods[spec].TimesExecuted++;
                }
                catch (FalsifiableException ex)
                {
                    ex.Spec = spec;
                    throw;
                }
            }

            return result;
        }

        public virtual void Shrink(Func<bool> runFunc) { }

        private void FilterOutSpecsWithFailingPrecondition()
        {
            factsToCheck = testMethods.Keys.Where(spec => spec.VerifyPrecondition()).ToList();
        }

        private void FilterOutSpecsWithFailingPostcondition()
        {
            factsToCheck = factsToCheck.Where(spec => spec.VerifyPostcondition()).ToList();
        }
    }

    public interface IUse { }
    public interface IUse<T> : IUse
    {
        void Set(T state);
    }
}
