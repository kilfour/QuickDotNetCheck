using System;
using NHibernate.Criterion;
using QuickDotNetCheck.ElaborateExample.People.Update;
using QuickDotNetCheck.ElaborateExample.Tests.DataAccess.Helpers;
using QuickDotNetCheck.ElaborateExample.Tests.People.Helpers;
using QuickDotNetCheck.ShrinkingStrategies;
using QuickGenerate;
using QuickGenerate.Writing;

namespace QuickDotNetCheck.ElaborateExample.Tests.People.Update
{
    public class UpdatePersonInvalidIdFixture : Fixture
    {
        private UpdatePersonRequest request;

        public override void Arrange()
        {
            request =
                new DomainGenerator()
                    .WithStringNameCounterPattern()
                    .With<UpdatePersonRequest>(opt => opt.For(r => r.Id, 666))
                    .One<UpdatePersonRequest>();
        }

        public override bool CanAct()
        {
            return new NumberOfPeopleInDb().Get() > 0;
        }

        protected override void Act()
        {
            new UpdatePersonHandler(DatabaseTest.NHibernateSession()).Handle(request);
        }

        [Spec]
        public void DbDoesNotContainPersonWithId()
        {
            Ensure.Equal(0, new NumberOfPeopleInDbWhere().Get(Restrictions.Eq("Id", request.Id)));
        }

        private ShrinkingStrategy shrinkingStrategy;

        public override void Shrink(Func<bool> runFunc)
        {
            shrinkingStrategy =
                new ShrinkingStrategy()
                    .Add(Simple.AllValues())
                    .Add(Get.From(request).AllValues())
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