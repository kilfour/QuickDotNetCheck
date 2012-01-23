using System.Collections.Generic;
using QuickDotNetCheck;
using Xunit;

namespace QuickDotNetCheckTests.SuiteTests.RunningFixtures
{
    public class DoFixturesTests
    {
        [Fact]
        public void Simple()
        {
            var spy = new FixtureSpy();
            new Suite(1, 0)
                .Using(() => spy)
                .Do(() => new TestObjectFixture { Id = 1 })
                .Do(() => new TestObjectFixture { Id = 2 })
                .Do(() => new TestObjectFixture { Id = 3 })
                .Run();

            Assert.Equal(3, spy.Ids.Count);

            Assert.Equal(1, spy.Ids[0]);
            Assert.Equal(2, spy.Ids[1]);
            Assert.Equal(3, spy.Ids[2]);
        }

        [Fact]
        public void Params()
        {
            var spy = new FixtureSpy();
            new Suite(1, 0)
                .Using(() => spy)
                .Do(() => new TestObjectFixture { Id = 1 }, 
                    () => new TestObjectFixture { Id = 2 },
                    () => new TestObjectFixture { Id = 3 })
                .Run();

            Assert.Equal(3, spy.Ids.Count);

            Assert.Equal(1, spy.Ids[0]);
            Assert.Equal(2, spy.Ids[1]);
            Assert.Equal(3, spy.Ids[2]);
        }

        [Fact]
        public void ParamsTwice()
        {
            var spy = new FixtureSpy();
            new Suite(2, 0)
                .Using(() => spy)
                .Do(() => new TestObjectFixture { Id = 1 },
                    () => new TestObjectFixture { Id = 2 },
                    () => new TestObjectFixture { Id = 3 })
                .Run();

            Assert.Equal(6, spy.Ids.Count);

            Assert.Equal(1, spy.Ids[0]);
            Assert.Equal(2, spy.Ids[1]);
            Assert.Equal(3, spy.Ids[2]);
            Assert.Equal(1, spy.Ids[3]);
            Assert.Equal(2, spy.Ids[4]);
            Assert.Equal(3, spy.Ids[5]);
        }

        [Fact]
        public void Complex()
        {
            var spy = new FixtureSpy();
            new Suite(2, 1)
                .Using(() => spy)
                .Do(() => new TestObjectFixture { Id = 1 },
                    () => new TestObjectFixture { Id = 2 },
                    () => new TestObjectFixture { Id = 3 })
                .Register(() => new TestObjectFixture {Id = 4})
                .Run();

            Assert.Equal(8, spy.Ids.Count);

            Assert.Equal(1, spy.Ids[0]);
            Assert.Equal(2, spy.Ids[1]);
            Assert.Equal(3, spy.Ids[2]);
            Assert.Equal(4, spy.Ids[3]);
            Assert.Equal(1, spy.Ids[4]);
            Assert.Equal(2, spy.Ids[5]);
            Assert.Equal(3, spy.Ids[6]);
            Assert.Equal(4, spy.Ids[7]);
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