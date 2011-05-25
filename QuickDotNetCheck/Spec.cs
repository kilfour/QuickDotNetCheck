using System;

namespace QuickDotNetCheck
{
    public class Spec
    {
        public string Name { get; private set; }

        private readonly Action invariant;
        
        private Func<bool> precondition = () => true;
        
        private Func<bool> postcondition = () => true;

        public Spec(string name, Action invariant)
        {
            Name = name;
            this.invariant = invariant;
        }

        public virtual void Verify()
        {
            invariant();
        }

        public Spec If(Func<bool> condition)
        {
            precondition = condition;
            return this;
        }

        public bool VerifyPrecondition()
        {
            return precondition();
        }

        public Spec IfAfter(Func<bool> condition)
        {
            postcondition = condition;
            return this;
        }

        public bool VerifyPostcondition()
        {
            return postcondition();
        }
    }
}