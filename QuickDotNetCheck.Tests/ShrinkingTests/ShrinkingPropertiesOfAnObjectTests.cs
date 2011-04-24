using QuickDotNetCheck.ShrinkingStrategies;
using Xunit;

namespace QuickDotNetCheckTests.ShrinkingTests
{
    public class ShrinkingPropertiesOfAnObjectTests
    {
        [Fact]
        public void OneOutOfThree()
        {
            var something =
                new SomethingToShrink
                    {
                        PropertyOne = 42,
                        PropertyTwo = 42,
                        PropertyThree = 42,
                    };

            var composite =
                new ManipulationStrategy()
                    .Add(Simple.AllValues())
                    .RegisterAll(something);

            composite.Shrink(() => something.PropertyOne == 42);

            Assert.False(composite.Shrunk());
            Assert.False(composite.Shrunk(something, e => e.PropertyOne));
            Assert.True(composite.Shrunk(something, e => e.PropertyTwo));
            Assert.True(composite.Shrunk(something, e => e.PropertyThree));
        }

        [Fact]
        public void TwoOutOfThree()
        {
            var something =
                new SomethingToShrink
                    {
                        PropertyOne = 42,
                        PropertyTwo = 42,
                        PropertyThree = 42,
                    };
            var composite =
                new ManipulationStrategy()
                    .Add(Simple.AllValues())
                    .RegisterAll(something);

            composite.Shrink(() => something.PropertyOne == 42 && something.PropertyTwo == 42);

            Assert.False(composite.Shrunk());
            Assert.False(composite.Shrunk(something, e => e.PropertyOne));
            Assert.False(composite.Shrunk(something, e => e.PropertyTwo));
            Assert.True(composite.Shrunk(something, e => e.PropertyThree));
        }

        [Fact(Skip="Tricky")]
        public void TwoOutOfThreeTricky()
        {
            var something =
                new SomethingToShrink
                {
                    PropertyOne = 42,
                    PropertyTwo = 43,
                    PropertyThree = 42,
                };
            var composite =
                new ManipulationStrategy()
                    .Add(Simple.AllValues())
                    .Add(Get.From(something).AllValues())
                    .RegisterAll(something);

            composite.Shrink(() => something.PropertyOne != 0 && something.PropertyTwo != 42);

            Assert.False(composite.Shrunk());
            Assert.False(composite.Shrunk(something, e => e.PropertyOne));
            Assert.False(composite.Shrunk(something, e => e.PropertyTwo));
            Assert.True(composite.Shrunk(something, e => e.PropertyThree));
        }

        public class SomethingToShrink
        {
            public int PropertyOne { get; set; }
            public int PropertyTwo { get; set; }
            public int PropertyThree { get; set; }
        }
    }
}