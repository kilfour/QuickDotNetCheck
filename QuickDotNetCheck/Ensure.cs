using System;
using QuickDotNetCheck.Exceptions;

namespace QuickDotNetCheck
{
    public static class Ensuring
    {
        public static int Count { get; set; }
    }

    public static class Ensure
    {
        public static void Throws<TException>()  where TException : Exception
        {
            Ensuring.Count++;
            if(Suite.LastException == null)
                throw new FalsifiableException(
                    string.Format("An exception of type : {0}", typeof(TException)),
                    "No exception thrown.");

            if (Suite.LastException.GetType() != typeof(TException))
                throw new FalsifiableException(
                    string.Format("An exception of type : {0}", typeof(TException)),
                    string.Format("An exception of type : {0}", Suite.LastException.GetType()));

            Suite.LastException = null;
        }

        public static void True(bool flag)
        {
            Ensuring.Count++;
            if (flag)
                return;
            throw new FalsifiableException("True", "False");
        }

        public static void True(bool flag, string message)
        {
            Ensuring.Count++;
            if (flag)
                return;
            throw new FalsifiableException("True", "False", message);
        }

        public static void False(bool flag)
        {
            Ensuring.Count++;
            if(!flag)
                return;
            throw new FalsifiableException("False", "True");
        }

        public static void False(bool flag, string message)
        {
            Ensuring.Count++;
            if (!flag)
                return;
            throw new FalsifiableException("False", "True", message);
        }

        public static void Null(object value)
        {
            Ensuring.Count++;
            if (value == null)
                return;
            throw new FalsifiableException("null", value.ToString());
        }

        public static void Null(object value, string message)
        {
            Ensuring.Count++;
            if (value == null)
                return;
            throw new FalsifiableException("null", value.ToString(), message);
        }

        public static void NotNull(object value)
        {
            Ensuring.Count++;
            if (value != null)
                return;
            throw new FalsifiableException("not null", "null");
        }

        public static void NotNull(object value, string message)
        {
            Ensuring.Count++;
            if (value != null)
                return;
            throw new FalsifiableException("not null", "null", message);
        }

        public static void Equal(object expected, object actual)
        {
            Ensuring.Count++;
            if(expected == null && actual != null)
                throw new FalsifiableException(null, actual.ToString());
            if (expected == null)
                return;
            if (expected.Equals(actual))
                return;
            throw new FalsifiableException(expected.ToString(), actual == null ? null : actual.ToString());
        }

        public static void Equal(object expected, object actual, string message)
        {
            Ensuring.Count++;
            if (expected.Equals(actual))
                return;
            throw new FalsifiableException(expected.ToString(), actual.ToString(), message);
        }

        public static void NotEqual(object expected, object actual)
        {
            Ensuring.Count++;
            if (!expected.Equals(actual))
                return;
            throw new FalsifiableException("Not " + expected, actual.ToString());
        }

        public static void Fail()
        {
            Ensuring.Count++;
            throw new FalsifiableException("Success", "Failure");
        }

        public static void Holds()
        {
            Ensuring.Count++;
        }

        public static void SmallerThan(int expected, int actual)
        {
            Ensuring.Count++;
            if (actual < expected)
                return;
            ThrowSmallerThan(expected, actual);
        }

        public static void SmallerThan(DateTime expected, DateTime actual)
        {
            Ensuring.Count++;
            if (actual < expected)
                return;
            ThrowSmallerThan(expected, actual);
        }

        private static void ThrowSmallerThan(object expected, object actual)
        {
            throw new FalsifiableException(
                string.Format("Smaller than {0}", expected),
                actual.ToString());
        }

        public static void SmallerThanOrEqual(int expected, int actual)
        {
            Ensuring.Count++;
            if (actual <= expected)
                return;
            ThrowSmallerThanOrEqual(expected, actual);
        }

        public static void SmallerThanOrEqual(DateTime expected, DateTime actual)
        {
            Ensuring.Count++;
            if (actual <= expected)
                return;
            ThrowSmallerThanOrEqual(expected, actual);
        }

        private static void ThrowSmallerThanOrEqual(object expected, object actual)
        {
            throw new FalsifiableException(
                string.Format("Smaller than or equal to {0}", expected),
                actual.ToString());
        }

        public static void GreaterThan(int expected, int actual)
        {
            Ensuring.Count++;
            if (actual > expected)
                return;
            ThrowGreaterThan(expected, actual);
        }

        public static void GreaterThan(DateTime expected, DateTime actual)
        {
            Ensuring.Count++;
            if (actual > expected)
                return;
            ThrowGreaterThan(expected, actual);
        }

        private static void ThrowGreaterThan(object expected, object actual)
        {
            throw new FalsifiableException(
                string.Format("Greater than {0}", expected),
                actual.ToString());
        }

        public static void GreaterThanOrEqual(int expected, int actual)
        {
            Ensuring.Count++;
            if (actual >= expected)
                return;
            ThrowGreaterThanOrEqual(expected, actual);
        }

        public static void GreaterThanOrEqual(DateTime expected, DateTime actual)
        {
            Ensuring.Count++;
            if (actual >= expected)
                return;
            ThrowGreaterThanOrEqual(expected, actual);
        }

        private static void ThrowGreaterThanOrEqual(object expected, object actual)
        {
            throw new FalsifiableException(
                string.Format("Smaller than or equal to {0}", expected),
                actual.ToString());
        }
    }
}