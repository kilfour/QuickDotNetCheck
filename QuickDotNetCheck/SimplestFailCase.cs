using System.Collections.Generic;
using QuickDotNetCheck.NotInTheRoot;

namespace QuickDotNetCheck
{
    public class SimplestFailCase
    {
        public List<IFixture> Fixtures;
        public SimplestFailCase(List<IFixture> fixtures)
        {
            Fixtures = fixtures;
        }
    }
}