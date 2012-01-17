using System;

namespace QuickDotNetCheck
{
    public class Spec
    {
        public string Name { get; set; }

        private readonly Action invariant;
        
        private Func<bool> precondition = () => true;
        
        private Func<bool> postcondition = () => true;

        public Spec(Action invariant)
        {
            this.invariant = invariant;
        }

        public virtual int Verify()
        {
            Ensuring.Count = 0;
            invariant();
            return Ensuring.Count;
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