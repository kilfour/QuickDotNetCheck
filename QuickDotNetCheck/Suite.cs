using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickDotNetCheck.Exceptions;
using QuickDotNetCheck.Implementation;
using QuickDotNetCheck.NotInTheRoot;
using QuickDotNetCheck.Reporting;

namespace QuickDotNetCheck
{
    public class Suite
    {
        public static Exception LastException { get; set; }

        public static IReporter Reporter = new ConsoleReporter();
        private readonly int numberOfTests;
        private readonly int numberOfFixtures;

        private bool shrink = true;

        private readonly List<Func<IFixture>> doFixtures = new List<Func<IFixture>>();
        private readonly List<Func<IFixture>> fixtureFuncs = new List<Func<IFixture>>();

        public Suite(int numberOfTests, int numberOfFixtures)
        {
            this.numberOfTests = numberOfTests;
            this.numberOfFixtures = numberOfFixtures;
        }

        public Suite DontShrink()
        {
            shrink = false;
            return this;
        }

        public Suite Do<TFixture>() where TFixture : IFixture
        {
            doFixtures.Add(() => (TFixture)Activator.CreateInstance(typeof(TFixture)));
            return this;
        }

        public Suite Do(Func<IFixture> fixtureFunc)
        {
            doFixtures.Add(fixtureFunc);
            return this;
        }

        public Suite Register<TFixture>()
            where TFixture : IFixture
        {
            fixtureFuncs.Add(() => (TFixture)Activator.CreateInstance(typeof(TFixture)));
            return this;
        }

        public Suite Register(Func<IFixture> fixtureFunc)
        {
            fixtureFuncs.Add(fixtureFunc);
            return this;
        }

        public TFixture GetLast<TFixture>()
        {
            return (TFixture) executedFixtures.Last(f => f.GetType() == typeof (TFixture));
        }

        private List<object> objects;
        private readonly List<Func<object>> objectFuncs = new List<Func<object>>();
        public Suite Using(Func<object> fixture)
        {
            objectFuncs.Add(fixture);
            return this;
        }

        private List<IDisposable> disposables;
        private readonly List<Func<IDisposable>> disposableFuncs = new List<Func<IDisposable>>();
        public Suite Using(Func<IDisposable> fixture)
        {
            disposableFuncs.Add(fixture);
            return this;
        }

        public T Get<T>()
        {
            var found = objects.SingleOrDefault(o => o.GetType() == typeof (T));
            if(found != null)
                return (T)found;
            return (T)disposables.Single(o => o.GetType() == typeof (T));
        }


        public RunReport Run()
        {
            objects = objectFuncs.Select(f => f()).ToList();
            disposables = disposableFuncs.Select(f => f()).ToList();

            var knownspecs =
                fixtureFuncs
                    .SelectMany(f => f().SpecNames())
                    .Union(doFixtures.SelectMany(f => f().SpecNames()))
                    .ToDictionary(s => s, s => 0);

            disposables.ForEach(d => d.Dispose());
            try
            {
                for (int testNumber = 0; testNumber < numberOfTests; testNumber++)
                {
                    executedFixtures = new List<IFixture>();

                    objects = objectFuncs.Select(f => f()).ToList();
                    disposables = disposableFuncs.Select(f => f()).ToList();

                    //execute Do Fixtures
                    foreach (var doFixture in doFixtures)
                    {
                        var fixture = doFixture();
                        executedFixtures.Add(fixture);
                        fixture.Arrange();
                        fixture.Execute();
                        var testedSpecs = fixture.Assert();
                        testedSpecs.ForEach(s => knownspecs[s]++);
                    }
                    for (int ix = 0; ix < numberOfFixtures; ix++)
                    {

                        var fixture = fixtureFuncs.Select(f => f()).Where(f => f.CanAct()).PickOne();
                        executedFixtures.Add(fixture);
                        fixture.Arrange();
                        try
                        {
                            fixture.Execute();
                        }
                        catch (Exception e)
                        {
                            LastException = e;
                        }
                        var testedSpecs = fixture.Assert();
                        testedSpecs.ForEach(s => knownspecs[s]++);
                        if (LastException != null)
                            throw LastException;
                    }
                }
                disposables.ForEach(d => d.Dispose());
            }
            catch (FalsifiableException failure)
            {
                disposables.ForEach(d => d.Dispose());
                if (shrink)
                    return new RunReport(false, failure, Shrink(executedFixtures, failure));
                return new RunReport(false, failure, null);
            }
            var untested = knownspecs.Where(s => s.Value == 0).ToList();
            if (untested.Count() > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Untested specs  : ");
                untested.ForEach(s => sb.AppendLine(s.Key));
                throw new ApplicationException(sb.ToString());
            }
            return new RunReport(true, null, null);
        }

        private bool Fails(IEnumerable<IFixture> actions, FalsifiableException previousFailure)
        {
            var actionsCopy = actions.ToList();
            var oldReporter = Reporter;
            Reporter = new NullReporter();
            objects = objectFuncs.Select(f => f()).ToList();
            disposables = disposableFuncs.Select(f => f()).ToList();
            for (int ix = 0; ix < actionsCopy.Count(); ix++)
            {
                try
                {
                    var fixture = actionsCopy.ElementAt(ix);
                    fixture.Execute();
                    fixture.Assert();
                }
                catch (FalsifiableException failure)
                {
                    if (failure.Message == previousFailure.Message)
                    {
                        disposables.ForEach(d => d.Dispose());
                        Reporter = oldReporter;
                        return true;
                    }
                    break;
                }
                catch(Exception ex)
                {
                    if (LastException != null && ex.Message == LastException.Message)
                    {
                        disposables.ForEach(d => d.Dispose());
                        Reporter = oldReporter;
                        return true;
                    }
                    break;
                }
            }
            disposables.ForEach(d => d.Dispose());
            Reporter = oldReporter;
            return false;
        }

        private List<IFixture> executedFixtures = new List<IFixture>();

        private SimplestFailCase Shrink(List<IFixture> fixtures, FalsifiableException previousFailure)
        {
            try
            {
                var simplestFailcase = ShrinkTransitionsList(fixtures, previousFailure);
                ShrinkFixtures(simplestFailcase, previousFailure);
                WriteSimplestFailCaseToReporter(simplestFailcase);
                return simplestFailcase;
            }
            catch (Exception e)
            {
                Reporter.WriteLine("----------------------------------------------------------");
                Reporter.WriteLine("Unexpected exception in QuickNet during Shrinking : ");
                Reporter.WriteLine(e.ToString());
                Reporter.WriteLine("----------------------------------------------------------");
                throw;
            }
        }

        private void ShrinkFixtures(SimplestFailCase simplestFailcase, FalsifiableException previousFailure)
        {
            simplestFailcase.Fixtures.ForEach(f => f.Shrink(() => Fails(simplestFailcase.Fixtures, previousFailure)));
        }

        private SimplestFailCase ShrinkTransitionsList(List<IFixture> fixtures, FalsifiableException previousFailure)
        {
            var simplestFailCase = new SimplestFailCase(fixtures);

            int ix = 0;
            while (ix < simplestFailCase.Fixtures.Count)
            {
                var lessActions = new List<IFixture>(simplestFailCase.Fixtures);
                lessActions.RemoveAt(ix);
                if (Fails(lessActions, previousFailure))
                {
                    simplestFailCase.Fixtures = lessActions;
                }
                else
                {
                    ix++;
                }
            }
            return simplestFailCase;
        }

        private void WriteSimplestFailCaseToReporter(SimplestFailCase simplestFailcase)
        {
            Reporter.WriteLine(simplestFailcase.Report());
        }
    }
}