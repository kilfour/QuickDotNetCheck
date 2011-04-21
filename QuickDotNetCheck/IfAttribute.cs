using System;

namespace QuickDotNetCheck
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IfAttribute : Attribute
    {
        public Type PreconditionType { get; private set; }
        public IfAttribute(Type preconditionType)
        {
            PreconditionType = preconditionType;
        }
    }
}