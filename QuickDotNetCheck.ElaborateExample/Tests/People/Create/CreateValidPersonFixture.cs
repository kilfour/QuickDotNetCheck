using System;
using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;
using QuickDotNetCheck.ElaborateExample.People.Create;
using QuickDotNetCheck.ElaborateExample.Tests.DataAccess.Helpers;
using QuickDotNetCheck.ElaborateExample.Tests.People.Helpers;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using QuickGenerate.Writing;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Create
{
    public class CreateValidPersonFixture : Fixture
    {
        private CreatePersonRequest request;
        private int numberOfPeopleInDbBeforeAct;

        public override void Arrange()
        {
            request =
                new DomainGenerator()
                    .One<CreatePersonRequest>();
        }

        public override void BeforeAct()
        {
            numberOfPeopleInDbBeforeAct = new NumberOfPeopleInDb().Get();
        }

        protected override void Act()
        {
            new CreatePersonHandler(DatabaseTest.NHibernateSession()).Handle(request);
        }

        [Spec]
        public void DbContainsOneMorePerson()
        {
            Ensure.Equal(numberOfPeopleInDbBeforeAct + 1, new NumberOfPeopleInDb().Get());
        }

        [Spec]
        public void DbContainsPersonWithFirstName()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("FirstName", request.FirstName)));
        }

        [Spec]
        public void DbContainsPersonWithLastName()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("LastName", request.LastName)));
        }

        [Spec]
        public void DbContainsPersonWithTitle()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Title", request.Title)));
        }

        [Spec]
        public void DbContainsPersonWithBirthDate()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("BirthDate", request.BirthDate)));
        }

        [Spec]
        public void DbContainsPersonWithAddressStreet()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Address.Street", request.AddressStreet)));
        }

        [Spec]
        public void DbContainsPersonWithAddressCity()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Address.City", request.AddressCity)));
        }

        [Spec]
        public void DbContainsPersonWithAddressPostalCode()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Address.PostalCode", request.AddressPostalCode)));
        }

        [Spec]
        public void DbContainsPersonWithAddressCountry()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Address.Country", request.AddressCountry)));
        }

        [Spec]
        public void DbContainsPersonWithPhone()
        {
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Phone", request.Phone)));
        }

        [Spec]
        public void DbContainsThisPerson()
        {
            var people =
                DatabaseTest.NHibernateSession()
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

        private CompositeShrinkingStrategy<CreatePersonRequest> shrinkingStrategy;

        public override void Shrink(Func<bool> runFunc)
        {
            shrinkingStrategy =
                ShrinkingStrategy.For(request)
                    .Add(Simple.AllValues())
                    .Add(Get.From(request).AllValues())
                    .RegisterAll();

            shrinkingStrategy.Shrink(runFunc);
        }

        public override string ToString()
        {
            var stream = new StringStream();
            stream.Write(GetType().Name);
            stream.WriteLine();
            stream.Write(shrinkingStrategy.Report());
            return stream.ToReader().ReadToEnd();
        }
    }


}
