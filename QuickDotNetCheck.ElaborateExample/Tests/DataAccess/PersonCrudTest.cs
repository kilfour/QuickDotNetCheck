using QuickDotNetCheck.ElaborateExample.Domain;
using QuickGenerate;
using Xunit;

namespace QuickDotNetCheck.ElaborateExample.Tests.DataAccess
{
    public class PersonCrudTest : CrudTest<Person, int>
    {
        protected override DomainGenerator GenerateAndSaveGenerator()
        {
            return
                new DomainGenerator()
                    .With<Person>(opt => opt.Ignore(person => person.Id))
                    .ForEach<Person>(SaveToSession);
        }

        [Fact]
        public void foo()
        {
            
        }            
    }
}