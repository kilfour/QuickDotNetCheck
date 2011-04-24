using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QuickDotNetCheck.Exceptions;
using QuickDotNetCheck.Implementation;
using QuickDotNetCheck.NotInTheRoot;

namespace QuickDotNetCheck
{
    public abstract class Fixture : IFixture
    {
        private readonly Dictionary<MethodInfo, FactInfo> testMethods;
        public IEnumerable<MethodInfo> factsToCheck;

        protected Fixture()
        {
            testMethods = new Dictionary<MethodInfo, FactInfo>();
            var specs = GetType()
                .GetMethods()
                .Where(mi => mi.HasAttribute<SpecAttribute>());
            foreach (var mi in specs)
            {
                testMethods[mi] = new FactInfo {Name = GetType().Name + "." + mi.Name};
            }
        }

        public IEnumerable<string> SpecNames()
        {
            return testMethods.Values.Select(v => v.Name);
        }

        public virtual void Arrange() { }
        
        public virtual bool CanAct()
        {
            return true;
        }

        public void Execute()
        {
            BeforeAct();
            FilterOutSpecsWithFailingPrecondition();
            Act();
            FilterOutSpecsWithFailingPostcondition();
        }

        public virtual void BeforeAct() { }

        protected abstract void Act();

        private void AssertSpec(MethodInfo info)
        {
            try
            {
                var action = (Action)Delegate.CreateDelegate(typeof (Action), this, info);
                action();
            }
            catch (FalsifiableException failure)
            {
                failure.Spec = info;
                throw;
            }
            catch (Exception)
            {
                Suite.Reporter.WriteLine(GetType().Name + " " + info.Name);
                throw;
            }
        }
        public IEnumerable<string> Assert()
        {
            foreach (var mi in factsToCheck)
            {
                testMethods[mi].TimesExecuted++;
                AssertSpec(mi);
            }

            return
                factsToCheck
                    .Select(mi => testMethods[mi].Name);
        }

        public virtual void Shrink(Func<bool> runFunc) { }

        private void FilterOutSpecsWithFailingPrecondition()
        {
            factsToCheck =
                testMethods.Keys.Where(mi => !mi.HasAttribute<IfAttribute>())
                    .Union(
                        testMethods.Keys
                            .Where(mi => mi.HasAttribute<IfAttribute>() && PreconditionIsSatisfied(mi)))
                    .ToList();
        }

        private bool PreconditionIsSatisfied(MethodInfo memberInfo)
        {
            var type = memberInfo.GetAttribute<IfAttribute>().PreconditionType;
            var precondition = (ICondition)Activator.CreateInstance(type);
            return precondition.Evaluate(this);
        }

        private void FilterOutSpecsWithFailingPostcondition()
        {
            factsToCheck =
                factsToCheck.Where(mi => !mi.HasAttribute<IfAfterAttribute>())
                    .Union(
                        factsToCheck
                            .Where(mi => mi.HasAttribute<IfAfterAttribute>() && PostconditionIsSatisfied(mi)))
                    .ToList();
        }

        private bool PostconditionIsSatisfied(MethodInfo memberInfo)
        {
            var type = memberInfo.GetAttribute<IfAfterAttribute>().PostconditionType;
            var postcondition = (ICondition)Activator.CreateInstance(type);
            return postcondition.Evaluate(this);
        }
    }
}
