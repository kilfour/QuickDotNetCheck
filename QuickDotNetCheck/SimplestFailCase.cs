using System.Collections.Generic;
using System.Text;
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

        public string Report()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("--------------------Simplest Fail Case--------------------");
            int ix = 1;
            foreach (var transition in Fixtures)
            {
                sb.Append(ix.ToString());
                sb.Append(" : ");
                sb.Append(transition.ToString());
                sb.AppendLine("");
                ix++;
            }
            sb.AppendLine("----------------------------------------------------------");
            return sb.ToString();
        }
    }
}