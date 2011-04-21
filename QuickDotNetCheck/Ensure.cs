using System;
using QuickDotNetCheck.Exceptions;

namespace QuickDotNetCheck
{
    public static class Ensure
    {
        public static void Throws<TException>()  where TException : Exception
        {
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
            if (flag)
                return;
            throw new FalsifiableException("True", "False");
        }

        public static void False(bool flag)
        {
            if(!flag)
                return;
            throw new FalsifiableException("False", "True");
        }

        public static void Null(object value)
        {
            if (value == null)
                return;
            throw new FalsifiableException("null", value.ToString());
        }

        public static void NotNull(object value)
        {
            if (value != null)
                return;
            throw new FalsifiableException("not null", "null");
        }

        public static void Equal(object expected, object actual)
        {
            if (expected.Equals(actual))
                return;
            throw new FalsifiableException(expected.ToString(), actual.ToString());
        }

        public static void NotEqual(object expected, object actual)
        {
            if (!expected.Equals(actual))
                return;
            throw new FalsifiableException("Not " + expected, actual.ToString());
        }

        public static void Fail()
        {
            throw new FalsifiableException("Success", "Failure");
        }

        public static void SmallerThan(int expected, int actual)
        {
            if (actual < expected)
                return;
            ThrowSmallerThan(expected, actual);
        }

        public static void SmallerThan(DateTime expected, DateTime actual)
        {
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
            if (actual <= expected)
                return;
            ThrowSmallerThanOrEqual(expected, actual);
        }

        public static void SmallerThanOrEqual(DateTime expected, DateTime actual)
        {
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
            if (actual > expected)
                return;
            ThrowGreaterThan(expected, actual);
        }

        public static void GreaterThan(DateTime expected, DateTime actual)
        {
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
            if (actual >= expected)
                return;
            ThrowGreaterThanOrEqual(expected, actual);
        }

        public static void GreaterThanOrEqual(DateTime expected, DateTime actual)
        {
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