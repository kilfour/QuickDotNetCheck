using System;

namespace QuickDotNetCheck.ElaborateExample.Domain
{
    public class Person : IHaveAnId<int>
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual Address Address { get; set; }
        public virtual string Phone { get; set; }

        protected Person() { }

        public Person(string firstName, string lastName, Address address, DateTime birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            BirthDate = birthDate;
        }
    }
}
