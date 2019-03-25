using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    internal static class Check
    {
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw new ArgumentNullException(parameterName);
            }
            return value;
        }

        public static IReadOnlyList<T> NotEmpty<T>(
          IReadOnlyList<T> value,
           string parameterName)
        {
            NotNull(value, parameterName);
            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw new ArgumentException(AbstractionsStrings.CollectionArgumentIsEmpty(parameterName));
            }
            return value;
        }

        public static string NotEmpty(string value, string parameterName)
        {
            Exception exception = null;
            if (value == null)
                exception = new ArgumentNullException(parameterName);
            else if (value.Trim().Length == 0)
                exception = new ArgumentException(AbstractionsStrings.ArgumentIsEmpty(parameterName));
            if (exception != null)
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw exception;
            }
            return value;
        }

        public static string NullButNotEmpty(string value, string parameterName)
        {
            if (value != null && value.Length == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw new ArgumentException(AbstractionsStrings.ArgumentIsEmpty(parameterName));
            }
            return value;
        }

        public static IReadOnlyList<T> HasNoNulls<T>(
          IReadOnlyList<T> value,
           string parameterName)
          where T : class
        {
            NotNull(value, parameterName);
            if (value.Any<T>(e => e == null))
            {
                NotEmpty(parameterName, nameof(parameterName));
                throw new ArgumentException(parameterName);
            }
            return value;
        }
    }
}
