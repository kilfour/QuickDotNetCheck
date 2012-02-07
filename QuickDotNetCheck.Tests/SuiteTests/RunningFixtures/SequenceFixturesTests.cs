using System.Collections.Generic;
using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests.SuiteTests.RunningFixtures
{
    public class SequenceFixturesTests
    {
        [Fact]
        public void Simple()
        {
            var spy = new FixtureSpy();
            new Suite()
                .Using(() => spy)
                .Do(2, opt => opt.Register(() => new TestObjectFixture { Id = 1 }))
                .Do(2, opt => opt.Register(() => new TestObjectFixture { Id = 2 }))
                .Run();

            Assert.Equal(4, spy.Ids.Count);

            Assert.Equal(1, spy.Ids[0]);
            Assert.Equal(1, spy.Ids[1]);
            Assert.Equal(2, spy.Ids[2]);
            Assert.Equal(2, spy.Ids[3]);
        }

        [Fact]
        public void Complex()
        {
            var spy = new FixtureSpy();
            new Suite()
                .Using(() => spy)
                .Do(() => new TestObjectFixture { Id = 1 })
                .Do(2, opt => opt.Register( 
                    () => new TestObjectFixture { Id = 2 },
                    () => new TestObjectFixture { Id = 2 }))
                .Do(() => new TestObjectFixture { Id = 3 })
                .Run();

            Assert.Equal(4, spy.Ids.Count);

            Assert.Equal(1, spy.Ids[0]);
            Assert.Equal(2, spy.Ids[1]);
            Assert.Equal(2, spy.Ids[2]);
            Assert.Equal(3, spy.Ids[3]);
        }

        public class FixtureSpy
        {
            public FixtureSpy()
            {
                Ids = new List<int>();
            }

            public List<int> Ids { get; set; }
            public void Add(int id)
            {
                Ids.Add(id);
            }
        }

        public class TestObjectFixture : Fixture, IUse<FixtureSpy>
        {
            private FixtureSpy fixtureSpy;
            public int Id { get; set; }

            public void Set(FixtureSpy state)
            {
                fixtureSpy = state;
            }

            protected override void Act()
            {
                fixtureSpy.Add(Id);
            }
        }
    }
}