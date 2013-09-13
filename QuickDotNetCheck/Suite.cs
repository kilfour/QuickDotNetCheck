using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickDotNetCheck.Exceptions;
using QuickDotNetCheck.NotInTheRoot;
using QuickGenerate;

namespace QuickDotNetCheck
{
	public class Suite
    {
        public static Exception LastException { get; set; }

        private readonly int numberOfTests;
        private readonly List<Sequence> sequences = new List<Sequence>();

        // todo: make this a local var
        private List<IFixture> executedFixtures = new List<IFixture>();

        private List<object> objects;
        private readonly List<Func<object>> objectFuncs = new List<Func<object>>();
        private List<IDisposable> disposables;
        private readonly List<Func<IDisposable>> disposableFuncs = new List<Func<IDisposable>>();
        private bool shrink = true;

        public Suite() : this(1){ }
        public Suite(int numberOfTests) 
        {
            this.numberOfTests = numberOfTests;
        }

        public Suite DontShrink()
        {
            shrink = false;
            return this;
        }

        public Suite Do<TFixture>() where TFixture : IFixture
        {
            Do(()=> (TFixture)Activator.CreateInstance(typeof(TFixture)));
            return this;
        }

        public Suite Do(Func<IFixture> fixtureFunc)
        {
            Register(1, fixtureFunc);
            return this;
        }

        public Suite Do(params Func<IFixture>[] funcs)
        {
            foreach (var fixtureFunc in funcs)
            {
                Do(fixtureFunc);    
            }
            return this;
        }

        public Suite Do(int numberToRun, params Func<Sequence, Sequence>[] registrationFuncs)
        {
            var sequence =
                new Sequence
                {
                    NumberToRun = numberToRun,
                };
            foreach (var registrationFunc in registrationFuncs)
            {
                registrationFunc(sequence);    
            }
            sequences.Add(sequence);
            return this;
        }

        private void Register(int numberToRun, params Func<IFixture>[] funcs)
        {
            var sequence =
                new Sequence
                {
                    NumberToRun = numberToRun,
                    FixtureFuncs = funcs.ToList(),
                };
            sequences.Add(sequence);
        }

        public Suite Using(Func<object> fixture)
        {
            objectFuncs.Add(fixture);
            return this;
        }

        public Suite Using(Func<IDisposable> disposable)
        {
            disposableFuncs.Add(disposable);
            return this;
        }

        public Suite Using<TObject>()
        {
            if(typeof(TObject).GetInterface("IDisposable") != null)
                disposableFuncs.Add(() => (IDisposable)Activator.CreateInstance<TObject>());
            else
                objectFuncs.Add(() => Activator.CreateInstance<TObject>());
            return this;
        }

        public void Run()
        {
            objects = objectFuncs.Select(f => f()).ToList();
            disposables = disposableFuncs.Select(f => f()).ToList();

            var knownspecs =
                sequences.SelectMany(s => s.FixtureFuncs.SelectMany(f => f().SpecNames()))
                    .Distinct()
                    .ToDictionary(s => s, s => 0);

        	var futureSpecs = new List<Spec>();
            disposables.ForEach(d => d.Dispose());
            for (var testNumber = 0; testNumber < numberOfTests; testNumber++)
            {
                executedFixtures = new List<IFixture>();
                objects = objectFuncs.Select(f => f()).ToList();
                disposables = disposableFuncs.Select(f => f()).ToList();
				RunSequences(knownspecs, testNumber, futureSpecs);
                disposables.ForEach(d => d.Dispose());
            }
            
            var untested = knownspecs.Where(s => s.Value == 0).ToList();
            if (untested.Count() > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Untested specs  : ");
                untested.ForEach(s => sb.AppendLine(s.Key));
                throw new UntestedSpecsException(sb.ToString());
            }
        }

        private void ExecuteFixture(int fixtureNumber, int testNumber, Dictionary<string, int> knownspecs, IFixture fixture, List<Spec> futureSpecs)
        {
            try
            {
                try
                {
                    LastException = null;
                    fixture.Execute();
                }
                catch (Exception e)
                {
                    LastException = e;
                }
            	
                var testedSpecs = fixture.AssertSpecs();
                foreach (var testedSpec in testedSpecs)
                {
                    knownspecs[testedSpec.Key] += testedSpec.Value;
                }

				futureSpecs.AddRange(fixture.GetFutureSpecs());
				foreach (var spec in futureSpecs.Where(s => s.ForFixture == fixture.GetType()))
				{
					try
					{
						knownspecs[spec.Name] += spec.Verify();
					}
					catch (FalsifiableException ex)
					{
						ex.Spec = spec;
						throw;
					}
				}
                if (LastException != null)
                    throw new UnexpectedException(LastException);


            }
            catch (FalsifiableException)
            {
                try
                {
                    if (LastException != null)
                        throw new UnexpectedException(LastException);
                    throw;
                }
                catch (FalsifiableException failure)
                {
                    disposables.ForEach(d => d.Dispose());
                    if(shrink)
                        throw new RunReport(testNumber + 1, fixtureNumber + 1, failure, Shrink(executedFixtures, failure));
                    throw new RunReport(testNumber + 1, fixtureNumber + 1, failure, null);
                }
            }
        }

        private void RunSequences(Dictionary<string, int> knownspecs, int testNumber, List<Spec> futureSpecs)
        {
            var fixtureNumber = 0;
            foreach (var sequence in sequences)
            {
                for (int toRun = 0; toRun < sequence.NumberToRun; toRun++)
                {
                    var fixture = sequence.FixtureFuncs.Select(f => SetUsedState(f())).Where(f => f.CanAct()).PickOne();
                    executedFixtures.Add(fixture);
                    fixture.Arrange();
					ExecuteFixture(fixtureNumber, testNumber, knownspecs, fixture, futureSpecs);
                    fixtureNumber++;
                }
            }
        }

        private object Get(Type type)
        {
            var found = objects.SingleOrDefault(o => o.GetType() == type);
            if (found != null)
                return found;
            found = disposables.SingleOrDefault(o => o.GetType() == type);
            if (found != null)
                return found;
            throw new InvalidOperationException(string.Format("Fixture uses an object of type {0}, but none is registered.", type.Name));
        }

        private IFixture SetUsedState(IFixture fixture)
        {
            var interfaces = 
                fixture
                    .GetType()
                    .GetInterfaces()
                    .Where(t => typeof (IUse).IsAssignableFrom(t) && t != typeof(IUse));
            foreach (var @interface in interfaces)
            {
                var genericType = @interface.GetGenericArguments()[0];
                var mi = fixture.GetType().GetMethod("Set", new[]{genericType});
                mi.Invoke(fixture, new[] {Get(genericType)});
            }
            return fixture;
        }

        private bool Fails(IEnumerable<IFixture> actions, FalsifiableException previousFailure)
        {
            var actionsCopy = actions.ToList();
            objects = objectFuncs.Select(f => f()).ToList();
            disposables = disposableFuncs.Select(f => f()).ToList();
            for (int ix = 0; ix < actionsCopy.Count(); ix++)
            {
                try
                {
                    var fixture = actionsCopy.ElementAt(ix);
                    SetUsedState(fixture);
                    if (fixture.CanAct())
                    {
                        LastException = null;
                        try
                        {
                            fixture.Execute();
                        }
                        catch (Exception e)
                        {
                            LastException = e;
                        }
                        
                        if (ix == actionsCopy.Count - 1)
                        {
                           
                            if (previousFailure.Spec != null)
                                previousFailure.Spec.Verify();
                            
                            if(previousFailure.InnerException != null)
                            {
                                if (LastException == null)
                                    return false;
                                return LastException.GetType() == previousFailure.InnerException.GetType();
                            }
                        }
                    }
                }
                catch (FalsifiableException)
                {
                    disposables.ForEach(d => d.Dispose());
                    return true;
                }
            }
            disposables.ForEach(d => d.Dispose());
            return false;
        }

        private SimplestFailCase Shrink(List<IFixture> fixtures, FalsifiableException previousFailure)
        {
            var simplestFailcase = ShrinkTransitionsList(fixtures, previousFailure);
            ShrinkFixtures(simplestFailcase, previousFailure);
            return simplestFailcase;
        }

        private void ShrinkFixtures(SimplestFailCase simplestFailcase, FalsifiableException previousFailure)
        {
            simplestFailcase
                .Fixtures
                .ForEach(
                    f => f.Shrink(
                        () => Fails(simplestFailcase.Fixtures, previousFailure)));
        }

        private SimplestFailCase ShrinkTransitionsList(List<IFixture> fixtures, FalsifiableException previousFailure)
        {
            var simplestFailCase = new SimplestFailCase(fixtures);
            var ix = 0;
            while (ix < simplestFailCase.Fixtures.Count - 1)
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
    }
}