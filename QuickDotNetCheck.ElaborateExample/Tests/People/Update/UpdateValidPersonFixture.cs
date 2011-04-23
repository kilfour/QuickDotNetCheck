using System;
using System.Linq;
using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.Domain;
using QuickDotNetCheck.ElaborateExample.People.Update;
using QuickDotNetCheck.ElaborateExample.Tests.DataAccess.Helpers;
using QuickDotNetCheck.ElaborateExample.Tests.People.Helpers;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using QuickGenerate.Implementation;
using QuickGenerate.Writing;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Update
{
    public class UpdateValidPersonFixture : Fixture
    {
        private UpdatePersonRequest request;
        private int numberOfPeopleInDbBeforeAct;
        private Gatherer<Person> originalPerson;

        public override void Arrange()
        {
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
                    
            request =
                new DomainGenerator()
                    .WithStringNameCounterPattern()
                    .With<UpdatePersonRequest>(opt => opt.For(r => r.Id, id))
                    .One<UpdatePersonRequest>();
        }

        public override bool CanAct()
        {
            return new NumberOfPeopleInDb().Get() > 0;
        }

        public override void BeforeAct()
        {
            numberOfPeopleInDbBeforeAct = new NumberOfPeopleInDb().Get();
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

        private CompositeShrinkingStrategy<UpdatePersonRequest> shrinkingStrategy;

        public override void Shrink(Func<bool> runFunc)
        {
            shrinkingStrategy =
                ShrinkingStrategy.For(request)
                    .Add(Simple.AllValues())
                    .Add(Get.From(request).AllValues())
                    .Add(originalPerson.AllCollected())
                    .Add(originalPerson.RecallFrom(p => p.Address).AllCollected())
                    .Ignore(e => e.Id)
                    .RegisterAll();

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
