using System.Collections.Generic;
using QuickDotNetCheck;
using QuickGenerate;
using Xunit;

namespace QuickDotNetCheckTests.ShrinkingTests
{
    public class ShrinkingAnObjectTestsIgnoreTests
    {
        [Fact]
        public void Shrunk()
        {
            var something = Generate.One<SomethingToShrink>();
            something.IntProperty = 42;
            var composite =
                ShrinkingStrategy
                    .For(something)
                    .Ignore(e => e.IntProperty)
                    .RegisterAll();

            Assert.Throws<KeyNotFoundException>(() => composite.StrategyFor(e => e.IntProperty));
        }

        public class SomethingToShrink
        {
            public int IntProperty { get; set; }
        }
    }
}