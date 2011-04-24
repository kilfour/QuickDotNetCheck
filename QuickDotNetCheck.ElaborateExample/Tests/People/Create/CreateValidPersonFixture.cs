using System;
using NHibernate.Criterion;
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
                    .WithStringNameCounterPattern()
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
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("FirstName", request.FirstName)));
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("LastName", request.LastName)));
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Title", request.Title)));
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("BirthDate", request.BirthDate)));
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Address.Street", request.AddressStreet)));
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Address.City", request.AddressCity)));
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Address.PostalCode", request.AddressPostalCode)));
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Address.Country", request.AddressCountry)));
            Ensure.GreaterThan(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Phone", request.Phone)));
        }

        private ManipulationStrategy manipulationStrategy;

        public override void Shrink(Func<bool> runFunc)
        {
            manipulationStrategy =
                new ManipulationStrategy()
                    .Add(Simple.AllValues())
                    .RegisterAll(request);
            
            manipulationStrategy.Shrink(runFunc);
        }

        public override string ToString()
        {
            var stream = new StringStream();
            stream.Write(GetType().Name);
            stream.WriteLine();
            stream.Write(manipulationStrategy.Report());
            return stream.ToReader().ReadToEnd();
        }
    }


}
