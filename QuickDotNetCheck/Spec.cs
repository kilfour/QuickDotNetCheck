using System;

namespace QuickDotNetCheck
{
    public class Spec
    {
        public string Name { get; private set; }
        private readonly Action invariant;

        public Spec(string name, Action invariant)
        {
            Name = name;
            this.invariant = invariant;
        }

        public virtual void Verify()
        {
            invariant();
        }
    }
}