using System;
using System.Collections.Generic;
using System.Linq;

namespace Sense.Extensions
{
    // ReSharper disable InconsistentNaming
    public static class IEnumerableExtensions
    // ReSharper restore InconsistentNaming
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in source)
            {
                action(item);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static string ToVerboseString<T>(this IEnumerable<T> enumerable)
        {
            var strings = enumerable.Select(element => element.ToString());
            return string.Join(", ", strings.ToArray());
        }

        public static string ToVerboseString<T>(this IEnumerable<T> enumerable, Func<T, string> toStringFunction)
        {
            var strings = enumerable.Select(toStringFunction);
            return string.Join(", ", strings.ToArray());
        }
    }
}
