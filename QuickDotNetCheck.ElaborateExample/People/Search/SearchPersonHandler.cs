using NHibernate;
using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;

namespace QuickDotNetCheck.ElaborateExample.People.Search
{
    public class SearchPersonHandler
    {
        private readonly ISession session;

        public SearchPersonHandler(ISession session)
        {
            this.session = session;
        }

        public Person Handle(SearchPersonRequest request)
        {
            return
                session
                    .CreateCriteria<Person>()
                    .Add(Restrictions.Eq("FirstName", request.FirstName))
                    .UniqueResult<Person>();
        }
    }
}
