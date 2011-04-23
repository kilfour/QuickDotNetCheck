using System;

namespace QuickDotNetCheck.ElaborateExample.People.Update
{
    public class UpdatePersonRequest
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public int AddressPostalCode { get; set; }
        public string AddressCountry { get; set; }
        public virtual string Phone { get; set; }
    }
}