using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guoc.BigMall.Infrastructure.Extension;

namespace Guoc.BigMall.Infrastructure.Utils
{
    public static class Ensure
    {
        public static bool When(Func<bool> condition)
        {
            return condition();
        }

        public static void Then(this bool conditionValue, Action handler)
        {
            if (conditionValue) handler();
        }

        public static void NotNull<T>(T param, string paramName, string message)
        {
            if (param == null)
                throw new FriendlyException(message, new ArgumentNullException(paramName, message));
        }

        public static void NotNull<T>(T param, string message)
        {
            if (param == null)
                throw new FriendlyException(message);
        }

        public static void NotNullOrEmpty(string param, string paramName, string message)
        {
            if (string.IsNullOrEmpty(param))
                throw new FriendlyException(message, new ArgumentNullException(paramName, message));
        }

        public static void NotNullOrEmpty(string param, string message)
        {
            if (string.IsNullOrEmpty(param))
                throw new FriendlyException(message);
        }

        public static void NotNullOrEmpty<T>(IEnumerable<T> param, string paramName, string message)
        {
            if (param == null || param.Count() == 0)
                throw new FriendlyException(message, new ArgumentNullException(paramName, message));
        }

        public static void NotNullOrEmpty<T>(IEnumerable<T> param, string message)
        {
            if (param == null || param.Count() == 0)
                throw new FriendlyException(message);
        }

        public static void True(bool param, string paramName, string message)
        {
            if (param == false)
                throw new FriendlyException(message, new ArgumentException(message, paramName));
        }

        public static void True(bool param, string message)
        {
            if (param == false)
                throw new FriendlyException(message);
        }

        public static void False(bool param, string message)
        {
            if (param == true)
                throw new FriendlyException(message);
        }

        public static void EqualThan<T>(T expected, T actual, string message)
        {
            if (!object.Equals(expected, actual))
                throw new FriendlyException(message);
        }

        public static void NotEqualThan<T>(T expected, T actual, string message)
        {
            if (object.Equals(expected, actual))
                throw new FriendlyException(message);
        }

        public static void GreaterThan<T>(T param, T comparand, string message)
            where T : IComparable<T>
        {
            if (!param.GreaterThan(comparand))
                throw new FriendlyException(message);
        }

        public static void GreaterThan<T>(T param, T comparand, string paramName, string message)
            where T : IComparable<T>
        {
            if (!param.GreaterThan(comparand))
                throw new FriendlyException(message, new ArgumentException(message, paramName));
        }

        public static void GreaterOrEqualThan<T>(T param, T comparand, string message)
            where T : IComparable<T>
        {
            if (!param.GreaterOrEqualThan(comparand))
                throw new FriendlyException(message);
        }

        public static void GreaterOrEqualThan<T>(T param, T comparand, string paramName, string message)
            where T : IComparable<T>
        {
            if (!param.GreaterOrEqualThan(comparand))
                throw new FriendlyException(message, new ArgumentException(message, paramName));
        }

        public static void SmallerThan<T>(T param, T comparand, string message)
            where T : IComparable<T>
        {
            if (!param.SmallerThan(comparand))
                throw new FriendlyException(message);
        }

        public static void SmallerThan<T>(T param, T comparand, string paramName, string message)
            where T : IComparable<T>
        {
            if (!param.SmallerThan(comparand))
                throw new FriendlyException(message, new ArgumentException(message, paramName));
        }

        public static void SmallerOrEqualThan<T>(T param, T comparand, string message)
            where T : IComparable<T>
        {
            if (!param.SmallerOrEqualThan(comparand))
                throw new FriendlyException(message);
        }

        public static void SmallerOrEqualThan<T>(T param, T comparand, string paramName, string message)
            where T : IComparable<T>
        {
            if (!param.SmallerOrEqualThan(comparand))
                throw new FriendlyException(message, new ArgumentException(message, paramName));
        }

        public static void Between<T>(T param, T minValue, T maxValue, string message)
            where T : IComparable<T>
        {
            if (minValue.GreaterThan(maxValue))
            {
                var error = "minValue 必须小于或等于 maxValue。";
                throw new FriendlyException(error, new ArgumentException(error, "minValue"));
            }
            if (param.SmallerThan(minValue) || param.GreaterThan(maxValue))
                throw new FriendlyException(message);
        }

        public static void Between<T>(T param, T minValue, T maxValue, string paramName, string message)
            where T : IComparable<T>
        {
            if (minValue.GreaterThan(maxValue))
            {
                var error = "minValue 必须小于或等于 maxValue。";
                throw new FriendlyException(error, new ArgumentException(error, "minValue"));
            }
            if (param.SmallerThan(minValue) || param.GreaterThan(maxValue))
                throw new FriendlyException(message, new ArgumentException(message, paramName));
        }

        public static void In<T>(T param, IEnumerable<T> source, string message)
        {
            if (source == null || source.Count() == 0 || !source.Any(p => object.Equals(p, param)))
                throw new FriendlyException(message);
        }

        public static void NotIn<T>(T param, IEnumerable<T> source, string message)
        {
            if (source.Any(p => object.Equals(p, param)))
                throw new FriendlyException(message);
        }
    }
}
