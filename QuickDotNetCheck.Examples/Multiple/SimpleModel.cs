using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickDotNetCheck.Examples.Multiple
{
    public class Project
    {
        public Budget Budget { get; set; }

        private readonly List<Payement> payements;
        public IEnumerable<Payement> Payements
        {
            get { return payements; }
        }

        public Project()
        {
            payements = new List<Payement>();
        }

        public void Add(Payement payement)
        {
            if(Budget == null)
                throw new ApplicationException("Can't add payement to a project without a budget.");
            payements.Add(payement);
        }
    }

    public class Budget { }
    public class Payement { }
}
