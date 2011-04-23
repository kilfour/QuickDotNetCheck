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
                    .Add(Simple.Values<int>())
                    .Add(Simple.Values<String>())
                    .Add(Simple.Values<DateTime>())
                    .Add(Get.From(request).All<string>())
                    .Add(Get.From(request).All<int>())
                    .Add(Get.From(request).All<DateTime>())
                    .Register(e => e.FirstName)
                    .Register(e => e.LastName)
                    .Register(e => e.Title)
                    .Register(e => e.BirthDate)
                    .Register(e => e.AddressStreet)
                    .Register(e => e.AddressCity)
                    .Register(e => e.AddressPostalCode)
                    .Register(e => e.AddressCountry)
                    .Register(e => e.LastName)
                    .Register(e => e.Phone);

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
