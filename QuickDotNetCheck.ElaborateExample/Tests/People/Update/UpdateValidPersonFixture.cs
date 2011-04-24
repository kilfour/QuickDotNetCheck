using System;
using System.Linq;
using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;
using QuickDotNetCheck.ElaborateExample.People.Update;
using QuickDotNetCheck.ElaborateExample.Tests.DataAccess.Helpers;
using QuickDotNetCheck.ElaborateExample.Tests.People.Create;
using QuickDotNetCheck.ElaborateExample.Tests.People.Helpers;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using QuickGenerate.Implementation;
using QuickGenerate.Writing;
using Xunit;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Update
{
    public class UpdateValidPersonFixture : Fixture
    {
        private UpdatePersonRequest request;
        private int numberOfPeopleInDbBeforeAct;
        private Gatherer<Person> originalPerson;

        [Fact]
        public void Verify()
        {
            new Suite(2, 5)
                .Using(() => new DatabaseTest())
                .Do(() => new CreateValidPersonFixture())
                .Register(() => new UpdateValidPersonFixture())
                .Run();
        }

        public override void Arrange()
        {
            request =
                new DomainGenerator()
                    .WithStringNameCounterPattern(1000)
                    .One<UpdatePersonRequest>();
        }

        public override bool CanAct()
        {
            return new NumberOfPeopleInDb().Get() > 0;
        }

        public override void BeforeAct()
        {
            numberOfPeopleInDbBeforeAct = new NumberOfPeopleInDb().Get();

            var validIds =
                DatabaseTest.NHibernateSession()
                    .CreateCriteria<Person>()
                    .SetProjection(Projections.Property("Id"))
                    .List<int>()
                    .ToArray();
            var id = validIds.PickOne();
            var person = DatabaseTest.NHibernateSession().Get<Person>(id);
            originalPerson =
                Gather
                    .From(person)
                    .Collect(p => p.Id)
                    .Collect(p => p.FirstName)
                    .Collect(p => p.LastName)
                    .Collect(p => p.Title)
                    .Collect(p => p.BirthDate)
                    .Collect(p => p.Phone)
                    .From(
                        p => p.Address,
                        g => g.Collect(a => a.Street)
                                 .Collect(a => a.PostalCode)
                                 .Collect(a => a.City)
                                 .Collect(a => a.Country));

            request.Id = id;
        }
        
        protected override void Act()
        {
            new UpdatePersonHandler(DatabaseTest.NHibernateSession()).Handle(request);
        }

        [Spec]
        public void DbContainsTheSameAmountOfPeople()
        {
            Ensure.Equal(numberOfPeopleInDbBeforeAct, new NumberOfPeopleInDb().Get());
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

        private ShrinkingStrategy shrinkingStrategy;

        public override void Shrink(Func<bool> runFunc)
        {
            shrinkingStrategy =
                new ShrinkingStrategy()
                    .Add(Simple.AllValues())
                    .AddNull<string>()
                    .Add(Get.From(request).AllValues())
                    .Add(originalPerson.AllCollected())
                    .Add(originalPerson.RecallFrom(p => p.Address).AllCollected())
                    .Ignore<UpdatePersonRequest,int>(e => e.Id)
                    .RegisterAll(request);

            shrinkingStrategy.Shrink(runFunc);
        }

        public override string ToString()
        {
            var stream = new StringStream();
            stream.Write(GetType().Name);
            stream.WriteLine();
            stream.Write(shrinkingStrategy.Report());
            stream.Write("Where Id == ");
            stream.Write(request.Id.ToString());
            return stream.ToReader().ReadToEnd();
        }
    }
}
