using QuickDotNetCheck.ElaborateExample.Domain;
using QuickDotNetCheck.ElaborateExample.Tests.DataAccess.Helpers;
using QuickGenerate;

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
    }
}