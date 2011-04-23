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
            new Suite(10, 5)
                .Using(() => new DatabaseTest())
                .Register(() => new CreateValidPersonFixture())
                .Register(() => new UpdateValidPersonFixture())
                .Run();
        }
    }
}
