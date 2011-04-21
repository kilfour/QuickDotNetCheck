using System.Linq;
using System.Reflection;

namespace QuickDotNetCheck.Implementation
{
    public static class Reflectionize
    {
        public static bool HasAttribute<T>(this MethodInfo methodInfo)
            where T : class
        {
            return methodInfo.GetCustomAttributes(true).Any(src => (src as T) != null);
        }

        public static T GetAttribute<T>(this MethodInfo methodInfo)
            where T : class
        {
            return methodInfo.GetCustomAttributes(true)
                .Where(src => (src as T) != null)
                .ToList().ConvertAll(src => ((T)src))
                .Single();
        }
    }
}