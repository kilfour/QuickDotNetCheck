using System;

namespace QuickDotNetCheck.ElaborateExample.People.Update
{
    public class UpdatePersonRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime BirthDate { get; set; }
        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public int AddressPostalCode { get; set; }
        public string AddressCountry { get; set; }
        public string Phone { get; set; }
    }
}