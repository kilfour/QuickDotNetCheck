using System;
using System.Linq;
using QuickGenerate;
using Xunit;

namespace QuickDotNetCheck.Examples.Multiple
{
    public class MySuite
    {
        [Fact]
        public void VerifyAll()
        {
            new Suite(50)
                .Using(() => new Project())
                .Do(100, opt => opt
                                    .Register<AddSubProject>()
                                    .Register<AddBudget>()
                                    .Register<AddPayement>())
                .Run();
        }
    }

    public class AddBudget : Fixture, IUse<Project>
    {
        private Project project;

        public void Set(Project state)
        {
            project = state;
        }

        public override bool CanAct()
        {
            return project.Budget == null;
        }

        protected override void Act()
        {
            project.Budget = new Budget();
        }
    }

    public class AddSubProject : Fixture, IUse<Project>
    {
        private SubProject subProject;
        private Project project;
        public void Set(Project state)
        {
            project = state;
        }
        
        protected override void Act()
        {
            subProject = new SubProject();
            project.Add(subProject);
        }

        [Spec]
        public void ProjectHasThisSubProject()
        {
            Ensure.True(project.SubProjects.Any(sp => sp == subProject));
        }

        [Spec]
        public void SubProjectHasTheProject()
        {
            Ensure.Equal(project, subProject.Project);
        }
    }

    public class AddPayement : Fixture, IUse<Project>
    {
        private Project project;
        private SubProject subProject;

        public void Set(Project state)
        {
            project = state;
        }
        
        public override bool CanAct()
        {
            return project.SubProjects.Count() > 0;
        }

        protected override void Act()
        {
            subProject = project.SubProjects.PickOne();
            subProject.Add(new Payement());
        }

        public Spec ThrowsIfNoBudget()
        {
            return
                new Spec(Ensure.Throws<ApplicationException>)
                    .If(() => project.Budget == null);
        }

        public Spec DoesNotThrowIfBudget()
        {
            return
                new Spec(Ensure.Holds)
                    .If(() => project.Budget != null);
        }
    }
}