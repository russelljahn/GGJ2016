using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Sense.Extensions
{
    public static class StringExtensions
    {
        public static bool Is<TProperty>(this string propertyName, Expression<Func<TProperty>> expression)
        {
            var unaryExpression = expression.Body as UnaryExpression;

            var memberExpression = unaryExpression != null
                ? (MemberExpression) unaryExpression.Operand
                : (MemberExpression) expression.Body;

            return memberExpression.Member.Name == propertyName;
        }

        /// <summary>
        ///     "Nicifies" string, adding spaces before every uppercase character after the first character.
        ///     Spaces won't be added between consecutive uppercase characters to preserve acronyms.
        /// </summary>
        public static string Nicify(this string str)
        {
            return Regex.Replace(str, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrEmpty(value) || value.All(char.IsWhiteSpace);
        }
    }
}
