using System;
using System.Collections.Generic;

namespace QuickDotNetCheck.Examples.Multiple
{
    public class Project
    {
        public Budget Budget { get; set; }

        private readonly List<SubProject> subProjects;
        public IEnumerable<SubProject> SubProjects
        {
            get { return subProjects; }
        }

        public Project()
        {
            subProjects = new List<SubProject>();
        }

        public void Add(SubProject subProject)
        {
            subProjects.Add(subProject);
            subProject.Project = this;
        }
    }

    public class SubProject
    {
        public Project Project { get; set; }

        private readonly List<Payement> payements;
        public IEnumerable<Payement> Payements
        {
            get { return payements; }
        }

        public SubProject()
        {
            payements = new List<Payement>();
        }

        public void Add(Payement payement)
        {
            if (Project.Budget == null)
                throw new ApplicationException("Can't add payement to a project without a budget.");
            payements.Add(payement);
        }
    }

    public class Budget { }
    public class Payement { }
}
