using NHibernate;
using QuickDotNetCheck.ElaborateExample.Domain;

namespace QuickDotNetCheck.ElaborateExample.People.Create
{
    public class CreatePersonHandler
    {
        private readonly ISession session;

        public CreatePersonHandler(ISession session)
        {
            this.session = session;
        }

        public void Handle(CreatePersonRequest request)
        {
            var address =
                new Address(
                    request.AddressStreet,
                    request.AddressCity,
                    request.AddressPostalCode,
                    request.AddressCountry);

            var person =
                new Person(
                    request.FirstName,
                    request.LastName,
                    address,
                    request.BirthDate)
                    {
                        Title = request.Title,
                        Phone = request.Phone
                    };

            session.Save(person);
        }
    }
}
