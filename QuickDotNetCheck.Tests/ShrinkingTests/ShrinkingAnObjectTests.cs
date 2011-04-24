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
                new ShrinkingStrategy()
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
                new ShrinkingStrategy()
                    .Add(-1, 0, 1, "")
                    .AddNull<string>()
                    .RegisterAll(something);

            composite.Shrink(() => false);

            Assert.False(composite.Shrunk());
            Assert.False(composite.Shrunk(something, e => e.IntProperty));
            Assert.False(composite.Shrunk(something, e => e.StringProperty));
        }

        public class SomethingToShrink
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }
    }
}