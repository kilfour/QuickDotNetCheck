using NHibernate;
using QuickDotNetCheck.ElaborateExample.Domain;

namespace QuickDotNetCheck.ElaborateExample.People.Update
{
    public class UpdatePersonHandler
    {
        private readonly ISession session;

        public UpdatePersonHandler(ISession session)
        {
            this.session = session;
        }

        public void Handle(UpdatePersonRequest request)
        {
            var person = session.Get<Person>(request.Id);
            person.Address =
                new Address(
                    request.AddressStreet,
                    request.AddressCity,
                    request.AddressPostalCode,
                    request.AddressCountry);

            person.FirstName = request.FirstName;
            person.LastName = request.LastName;
            person.BirthDate = request.BirthDate;
            //person.Title = request.Title;
            person.Phone = request.Phone;
        }
    }
}
