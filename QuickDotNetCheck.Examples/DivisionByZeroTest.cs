using System;
using System.Text;
using Xunit;
using QuickGenerate.Primitives;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickDotNetCheck.ShrinkingStrategies.Manipulations;

namespace QuickDotNetCheck.Examples
{
    public class DivisionByZeroTest
    {
        [Fact]
        public void Go()
        {
            var suite = new Suite(100, 10);
            suite.Using(() => new Sut());
            suite.Register(() => new MyFixture(suite));
            suite.Run();
        }

        public class Sut
        {
            public int Run(int a, int b)
            {
                return a / b;
            }
        }

        public class MyFixture : Fixture
        {
            private int a { get; set; }
            private int b { get; set; }
            private int output;
            private readonly Suite suite;

            public MyFixture(Suite suite)
            {
                this.suite = suite;
            }

            public override void Arrange()
            {
                a = new IntGenerator(0, 20).GetRandomValue();
                b = new IntGenerator(0, 20).GetRandomValue();
            }

            protected override void Act()
            {
                output = suite.Get<Sut>().Run(a, b);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(GetType().Name);
                if (!shrinkA.Shrunk())
                    sb.AppendFormat(", a : {0}", a);
                if (!shrinkB.Shrunk())
                    sb.AppendFormat(", b : {0}", b);
                sb.Append(".");
                return sb.ToString();
            }

            private ShrinkingStrategy shrinkA;
            private ShrinkingStrategy shrinkB;

            public override void Shrink(Func<bool> runFunc)
            {
                shrinkA =
                    new ShrinkingStrategy()
                        .Add(Manipulate.This(this)
                                 .Change(e => e.a, -1)
                                 .Change(e => e.a, 0)
                                 .Change(e => e.a, 1));
                shrinkB =
                    new ShrinkingStrategy()
                        .Add(Manipulate.This(this)
                                 .Change(e => e.b, -1)
                                 .Change(e => e.b, 0)
                                 .Change(e => e.b, 1));
                shrinkA.Shrink(runFunc);
                shrinkB.Shrink(runFunc);
            }
        }
    }
}
