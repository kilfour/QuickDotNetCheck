using System;
using System.Collections.Generic;

namespace QuickDotNetCheck.NotInTheRoot
{
    public interface IFixture
    {
        bool CanAct();

        void Arrange();
        void Execute(); // Act is used in derived fixtures, execute decorates this a little
        IDictionary<string, int> AssertSpecs();

        void Shrink(Func<bool> runFunc);
        IEnumerable<string> SpecNames();
    }
}