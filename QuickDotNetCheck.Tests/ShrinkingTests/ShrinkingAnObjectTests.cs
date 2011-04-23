using QuickDotNetCheck;
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
                ShrinkingStrategy
                    .For(something)
                    .AddValuesFor(e => e.IntProperty, -1, 0, 1)
                    .AddValuesFor(e => e.StringProperty, "", null);

            composite.Shrink(() => true);

            Assert.True(composite.Shrunk());
            Assert.True(composite.StrategyFor(e => e.IntProperty).Shrunk());
            Assert.True(composite.StrategyFor(e => e.StringProperty).Shrunk());
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