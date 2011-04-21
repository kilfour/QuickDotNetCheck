using System;

namespace QuickDotNetCheck
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IfAfterAttribute : Attribute
    {
        public Type PostconditionType { get; private set; }
        public IfAfterAttribute(Type postconditionType)
        {
            PostconditionType = postconditionType;
        }
    }
}