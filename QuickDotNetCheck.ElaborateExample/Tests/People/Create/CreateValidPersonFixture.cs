using NHibernate;
using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;
using QuickDotNetCheck.ElaborateExample.People.Create;
using QuickDotNetCheck.ElaborateExample.Tests.People.Helpers;
using QuickGenerate;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Create
{
    public class CreateValidPersonFixture : Fixture
    {
        private readonly ISession session;
        private CreatePersonRequest request;
        private int numberOfPeopleInDbBeforeAct;

        public CreateValidPersonFixture(ISession session)
        {
            this.session = session;
        }

        public override void Arrange()
        {
            request =
                new DomainGenerator()
                    .One<CreatePersonRequest>();
        }

        public override void BeforeAct()
        {
            numberOfPeopleInDbBeforeAct = new NumberOfPeopleInDb(session).Get();
        }

        protected override void Act()
        {
            new CreatePersonHandler(session).Handle(request);
        }

        [Spec]
        public void DbContainsOneMorePerson()
        {
            Ensure.Equal(numberOfPeopleInDbBeforeAct + 1, new NumberOfPeopleInDb(session).Get());
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
