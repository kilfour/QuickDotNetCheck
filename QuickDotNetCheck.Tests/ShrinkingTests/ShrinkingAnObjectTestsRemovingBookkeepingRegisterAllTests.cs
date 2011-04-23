using QuickDotNetCheck;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using Xunit;

namespace QuickDotNetCheckTests.ShrinkingTests
{
    public class ShrinkingAnObjectTestsRemovingBookkeepingRegisterAllTests
    {
        [Fact(Skip="TODO")]
        public void Shrunk()
        {
            var something = Generate.One<SomethingToShrink>();
            something.IntProperty = 42;
            var composite =
                ShrinkingStrategy
                    .For(something)
                    .Add(Simple.Values<int>())
                    .Add(Simple.Values<string>())
                    .RegisterAll();

            composite.Shrink(() => something.IntProperty == 42);

            Assert.False(composite.Shrunk());
            Assert.False(composite.StrategyFor(e => e.IntProperty).Shrunk());
            Assert.True(composite.StrategyFor(e => e.StringProperty).Shrunk());
        }

        public class SomethingToShrink
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }
    }
}