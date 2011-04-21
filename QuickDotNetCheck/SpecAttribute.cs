using System;

namespace QuickDotNetCheck
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SpecAttribute : Attribute {  }
}