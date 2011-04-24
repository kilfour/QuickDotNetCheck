using System;
using QuickDotNetCheck;
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
                ShrinkingStrategy
                    .For(something)
                    .Add(Simple.AllValues())
                    .RegisterAll();

            composite.Shrink(() => something.PropertyOne == 42);

            Assert.False(composite.Shrunk());
            Assert.False(composite.StrategyFor(e => e.PropertyOne).Shrunk());
            Assert.True(composite.StrategyFor(e => e.PropertyTwo).Shrunk());
            Assert.True(composite.StrategyFor(e => e.PropertyThree).Shrunk());
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
                ShrinkingStrategy
                    .For(something)
                    .Add(Simple.AllValues())
                    .RegisterAll();

            composite.Shrink(() => something.PropertyOne == 42 && something.PropertyTwo == 42);

            Assert.False(composite.Shrunk());
            Assert.False(composite.StrategyFor(e => e.PropertyOne).Shrunk());
            Assert.False(composite.StrategyFor(e => e.PropertyTwo).Shrunk());
            Assert.True(composite.StrategyFor(e => e.PropertyThree).Shrunk());
        }

        [Fact]
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
                ShrinkingStrategy
                    .For(something)
                    .Add(Get.From(something).AllValues())
                    .Add(Simple.AllValues())
                    .RegisterAll();

            composite.Shrink(() => something.PropertyOne != 0 && something.PropertyTwo != 42);

            Assert.False(composite.Shrunk());
            Assert.False(composite.StrategyFor(e => e.PropertyOne).Shrunk());
            Assert.False(composite.StrategyFor(e => e.PropertyTwo).Shrunk());
            Assert.True(composite.StrategyFor(e => e.PropertyThree).Shrunk());
        }

        public class SomethingToShrink
        {
            public int PropertyOne { get; set; }
            public int PropertyTwo { get; set; }
            public int PropertyThree { get; set; }
        }
    }
}