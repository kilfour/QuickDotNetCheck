using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;
using QuickDotNetCheck.ElaborateExample.Tests.DataAccess.Helpers;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Helpers
{
    public class NumberOfPeopleInDb
    {
        public int Get()
        {
            return DatabaseTest.NHibernateSession()
                .CreateCriteria<Person>()
                .SetProjection(Projections.Count("Id"))
                .UniqueResult<int>();
        }
    }
}
