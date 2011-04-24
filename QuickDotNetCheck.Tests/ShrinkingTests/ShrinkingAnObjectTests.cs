using QuickDotNetCheck;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using Xunit;

namespace QuickDotNetCheckTests.ShrinkingTests
{
    public class ShrinkingAnObjectTests
    {
        [Fact]
        public void Shrunk()
        {
            var something = Generate.One<SomethingToShrink>();
            var composite =
                new ManipulationStrategy()
                    .Add(-1, 0, 1, "")
                    .AddNull<string>()
                    .RegisterAll(something);

            composite.Shrink(() => true);

            Assert.True(composite.Shrunk());
            Assert.True(composite.Shrunk(something, e => e.IntProperty));
            Assert.True(composite.Shrunk(something, e => e.StringProperty));
        }

        [Fact]
        public void NotShrunk()
        {
            var something = Generate.One<SomethingToShrink>();
            var composite =
                ShrinkingStrategy
                    .For(something)
                    .AddValuesFor(e => e.IntProperty, -1, 0, 1)
                    .AddValuesFor(e => e.StringProperty, "", null);

            composite.Shrink(() => false);

            Assert.False(composite.Shrunk());
            Assert.False(composite.StrategyFor(e => e.IntProperty).Shrunk());
            Assert.False(composite.StrategyFor(e => e.StringProperty).Shrunk());
        }

        public class SomethingToShrink
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }
    }
}