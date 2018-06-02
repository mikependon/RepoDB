using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>RepoDb.String</i> object.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Joins an array string with a given separator.
        /// </summary>
        /// <param name="strings">The enumerable list of strings.</param>
        /// <param name="separator">The separator to be used.</param>
        /// <returns>A joined string from a given array of strings separated by the defined separator.</returns>
        public static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }

        /// <summary>
        /// Removes the database quotes from the string.
        /// </summary>
        /// <param name="value">The string value where the database quotes will be removed.</param>
        /// <returns></returns>
        public static string AsUnquoted(this string value)
        {
            var v = value?.IndexOf(".") >= 0 ? value.Split(".".ToCharArray()).Last() : value;
            return Regex.Replace(v, @"[\[\]']+", "");
        }

        // AsEnumerable
        public static IEnumerable<string> AsEnumerable(this string value)
        {
            return value != null ? new[] { value } : null;
        }

        // AsJoinQualifier
        public static string AsJoinQualifier(this string value, string leftAlias, string rightAlias)
        {
            return $"{leftAlias}.[{value.AsUnquoted()}] = {rightAlias}.[{value.AsUnquoted()}]";
        }

        // AsField
        public static string AsField(this string value)
        {
            return $"[{value.AsUnquoted()}]";
        }

        // AsParameter
        public static string AsParameter(this string value)
        {
            return $"@{value.AsUnquoted()}";
        }

        // AsParameterAsField
        public static string AsParameterAsField(this string value)
        {
            return $"{AsParameter(value.AsUnquoted())} {Constant.As.ToUpper()} {AsField(value.AsUnquoted())}";
        }

        // AsFieldAndParameter
        public static string AsFieldAndParameter(this string value)
        {
            return $"{AsField(value.AsUnquoted())} = {AsParameter(value.AsUnquoted())}";
        }

        // AsFieldAndAliasField
        public static string AsFieldAndAliasField(this string value, string alias)
        {
            return $"{AsField(value.AsUnquoted())} = {alias}.{AsField(value.AsUnquoted())}";
        }

        /* IEnumerable<string> */

        // AsFields
        internal static IEnumerable<string> AsFields(this IEnumerable<string> values)
        {
            return values?.Select(value => value.AsField());
        }

        // AsParameters
        internal static IEnumerable<string> AsParameters(this IEnumerable<string> values)
        {
            return values?.Select(value => value.AsParameter());
        }

        // AsParametersAsFields
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<string> values)
        {
            return values?.Select(value => value.AsParameterAsField());
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<string> values)
        {
            return values?.Select(value => value.AsFieldAndParameter());
        }

        // AsFieldsAndAliasFields
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<string> values, string alias)
        {
            return values?.Select(value => value.AsFieldAndAliasField(alias));
        }
    }
}
