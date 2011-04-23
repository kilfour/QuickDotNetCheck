using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;
using QuickDotNetCheck.ElaborateExample.People.Update;
using QuickDotNetCheck.ElaborateExample.Tests.People.Helpers;
using QuickGenerate;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Update
{
    public class UpdateValidPersonFixture : Fixture
    {
        private readonly ISession session;

        public UpdateValidPersonFixture(ISession session)
        {
            this.session = session;
        }

        private UpdatePersonRequest request;

        public override void Arrange()
        {
            var validIds =
                session
                    .CreateCriteria<Person>()
                    .SetProjection(Projections.Property("Id"))
                    .List<int>()
                    .ToArray();

            request =
                new DomainGenerator()
                    .With<UpdatePersonRequest>(opt => opt.For(r => r.Id, validIds))
                    .One<UpdatePersonRequest>();
        }

        private int numberOfPeopleInDbBeforeAct;

        public override bool CanAct()
        {
            return new NumberOfPeopleInDb(session).Get() > 0;
        }

        public override void BeforeAct()
        {
            numberOfPeopleInDbBeforeAct = new NumberOfPeopleInDb(session).Get();
        }
        
        protected override void Act()
        {
            new UpdatePersonHandler(session).Handle(request);
        }

        [Spec]
        public void DbContainsTheSameAmountOfPeople()
        {
            Ensure.Equal(numberOfPeopleInDbBeforeAct, new NumberOfPeopleInDb(session).Get());
        }

        [Spec]
        public void DbContainsThisPerson()
        {
            var people =
                session
                    .CreateCriteria<Person>()
                    .Add(Restrictions.Eq("FirstName", request.FirstName))
                    .Add(Restrictions.Eq("LastName", request.LastName))
                    .Add(Restrictions.Eq("Title", request.Title))
                    .Add(Restrictions.Eq("BirthDate", request.BirthDate))
                    .Add(Restrictions.Eq("Address.Street", request.AddressStreet))
                    .Add(Restrictions.Eq("Address.City", request.AddressCity))
                    .Add(Restrictions.Eq("Address.PostalCode", request.AddressPostalCode))
                    .Add(Restrictions.Eq("Address.Country", request.AddressCountry))
                    .Add(Restrictions.Eq("Phone", request.Phone))
                    .List<Person>();

            Ensure.GreaterThan(0, people.Count);
        }
    }
}
