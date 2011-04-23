using NHibernate;
using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Helpers
{
    public class NumberOfPeopleInDb
    {
        private readonly ISession session;

        public NumberOfPeopleInDb(ISession session)
        {
            this.session = session;
        }

        public int Get()
        {
            return session
                .CreateCriteria<Person>()
                .SetProjection(Projections.Count("Id"))
                .UniqueResult<int>();
        }
    }
}
