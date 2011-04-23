using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;
using QuickDotNetCheck.ElaborateExample.Tests.DataAccess.Helpers;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Helpers
{
    public class NumberOfPeopleInDbWhere
    {
        public int Get(SimpleExpression expression)
        {
            return DatabaseTest.NHibernateSession()
                .CreateCriteria<Person>()
                .Add(expression)
                .SetProjection(Projections.Count("Id"))
                .UniqueResult<int>();
        }
    }
}