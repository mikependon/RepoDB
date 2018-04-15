using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoDb.Extensions
{
    public static class StringExtension
    {
        // Join
        public static string Join(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }

        // AsUnquoted
        public static string AsUnquoted(this string value)
        {
            return Regex.Replace(value, @"[\[\]']+", "");
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
