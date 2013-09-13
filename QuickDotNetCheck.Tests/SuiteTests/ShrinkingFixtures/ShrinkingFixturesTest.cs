using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests.SuiteTests.ShrinkingFixtures
{
    public class ShrinkingFixturesTest
    {
        [Fact]
        public void Test()
        {
            var report = Assert.Throws<RunReport>(
                () =>
                new Suite(10)
                    .Using(() => new TestState())
                    .Do(100,
                        opt => opt.Register<ParentTestFixture>(),
                        opt => opt.Register<ChildTestFixture>(),
                        opt => opt.Register<FailChildTestFixture>())
                    .Run());

            var failcase = report.SimplestFailCase;
            Assert.Equal(4, failcase.Fixtures.Count);
        }

        [Fact]
        public void TestExplicit()
        {
            var report = Assert.Throws<RunReport>(
                () =>
                new Suite()
                    .Using(() => new TestState())
                    .Do<ParentTestFixture>()
                    .Do<ChildTestFixture>()
                    .Do<ParentTestFixture>() // should be shrunk out
                    .Do<ChildTestFixture>()
                    .Do<FailChildTestFixture>()
                    .Run());

            var failcase = report.SimplestFailCase;
            Assert.Equal(4, failcase.Fixtures.Count);
        }

        public class TestState
        {
            public int ParentRan { get; set; }
            public int ChildrenRan { get; set; }
        }

        public class ParentTestFixture : Fixture, IUse<TestState>
        {
            private TestState testState;

            public void Set(TestState state)
            {
                testState = state;
            }

            protected override void Act()
            {
                testState.ParentRan++;
            }
        }

        public class ChildTestFixture : Fixture, IUse<TestState>
        {
            private TestState testState;

            public void Set(TestState state)
            {
                testState = state;
            }
            
            public override bool CanAct()
            {
                return testState.ParentRan > 0;
            }

            protected override void Act()
            {
                testState.ChildrenRan++;
            }
        }

        public class FailChildTestFixture : Fixture, IUse<TestState>
        {
            private TestState testState;

            public void Set(TestState state)
            {
                testState = state;
            }

            public override bool CanAct()
            {
                return testState.ParentRan > 0;
            }

            protected override void Act()
            {
                testState.ChildrenRan++;
            }

            [Spec]
            public void Fail()
            {
                if (testState.ChildrenRan == 3)
                    Ensure.Fail();
            }
        }
    }
}
