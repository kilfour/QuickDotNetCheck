using System.Collections.Generic;
using QuickDotNetCheck;
using QuickGenerate;
using Xunit;

namespace QuickDotNetCheckTests.SuiteTests.RunningFixtures
{
    public class RunningFixturesWithDoTests
    {
        private static List<int> fixturesExecuted;

        public RunningFixturesWithDoTests()
        {
            fixturesExecuted = new List<int>();
        }

        [Fact]
        public void ZeroTestsWhatEverTransitions()
        {
            var suite =
                new Suite(0)
                    .Do<SomeFixtureToRun>()
                    .Do<SomeOtherFixtureToRun>()
                    .Do(new[] {1, 100}.FromRange(), opt => opt.Register<YetAnotherFixtureToRun>());

            suite.Run();

            Assert.Equal(0, fixturesExecuted.Count);
        }

        [Fact]
        public void OneTestZeroTransitions()
        {
            var suite =
                new Suite(1)
                    .Do<SomeFixtureToRun>()
                    .Do<SomeOtherFixtureToRun>()
                    .Do(0, opt => opt.Register<YetAnotherFixtureToRun>());

            suite.Run();

            Assert.Equal(2, fixturesExecuted.Count);
            Assert.Equal(1, fixturesExecuted[0]);
            Assert.Equal(2, fixturesExecuted[1]);
        }

        [Fact]
        public void OneTestWhateverTransitions()
        {
            var numberOfTransitions = new[] {1, 100}.FromRange();
            var suite =
                new Suite(1)
                    .Do<SomeFixtureToRun>()
                    .Do<SomeOtherFixtureToRun>()
                    .Do(numberOfTransitions, opt => opt.Register<YetAnotherFixtureToRun>());

            suite.Run();

            Assert.Equal(numberOfTransitions + 2, fixturesExecuted.Count);
            Assert.Equal(1, fixturesExecuted[0]);
            Assert.Equal(2, fixturesExecuted[1]);
            for (int i = 2; i < numberOfTransitions + 2; i++)
            {
                Assert.Equal(3, fixturesExecuted[i]);    
            }
        }

        public class SomeFixtureToRun : Fixture
        {
            protected override void Act() { fixturesExecuted.Add(1);}
        }

        public class SomeOtherFixtureToRun : Fixture
        {
            protected override void Act() { fixturesExecuted.Add(2); }
        }

        public class YetAnotherFixtureToRun : Fixture
        {
            protected override void Act() { fixturesExecuted.Add(3); }
        }
    }
}