using QuickDotNetCheck.ElaborateExample.Tests.DataAccess.Helpers;
using QuickDotNetCheck.ElaborateExample.Tests.People.Create;
using QuickDotNetCheck.ElaborateExample.Tests.People.Update;
using Xunit;

namespace QuickDotNetCheck.ElaborateExample.Tests.People
{
    public class PeopleTests
    {
        [Fact]
        public void All()
        {
            var report =
                new Suite(1, 10)
                    .Using(() => new DatabaseTest())
                    .Register(() => new CreateValidPersonFixture())
                    .Register(() => new UpdateValidPersonFixture())
                    .Run();

            Assert.True(report.Succeeded(), report.Report());
        }
    }
}
