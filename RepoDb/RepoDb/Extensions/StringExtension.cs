using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="String"/>.
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
        /// Removes the non-alphanumeric characters.
        /// </summary>
        /// <param name="value">The string value where the non-alphanumeric characters will be removed.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before removing the non-alphanumeric characters.</param>
        /// <returns>The alphanumeric string.</returns>
        internal static string AsAlphaNumeric(this string value, bool trim = false)
        {
            if (trim)
            {
                value = value.Trim();
            }

            int originalLength = value.Length;
            char[] filtered = new char[originalLength];
            for (var pos = 0; pos < originalLength; pos++)
            {
                char currentChar = value[pos];
                if ((currentChar >= '0' && currentChar <= 'z') &&  // Are we within the alpha-numeric range of the ASCII table?
                    (currentChar >= 'a' ||                         // Lower-case
                     currentChar <= '9' ||                         // Numeric
                     (currentChar >= 'A' && currentChar <= 'Z')))  // Upper-case 
                {
                    filtered[pos] = currentChar;
                }
                else
                {
                    filtered[pos] = '_';
                }
            }

            return new string(filtered);
        }

        /// <summary>
        /// Unquotes a string.
        /// </summary>
        /// <param name="value">The string value to be unqouted.</param>
        /// <returns>The unquoted string.</returns>
        public static string AsUnquoted(this string value)
        {
            int originalLength = value.Length;
            int validCharCount = 0;

            char[] unquoted = new char[originalLength];

            for (var pos = 0; pos < originalLength; pos++)
            {
                var currentChar = value[pos];
                if (currentChar != '[' &&
                    currentChar != ']' &&
                    currentChar != '\'' &&
                    currentChar != '\"')
                {
                    unquoted[validCharCount] = currentChar;
                    validCharCount++;
                }
            }

            return new string(unquoted, 0, validCharCount);
        }

        /// <summary>
        /// Unquotes a string.
        /// </summary>
        /// <param name="value">The string value to be unqouted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before unquoting.</param>
        /// <param name="separator">The separator in which the quotes will be removed.</param>
        /// <returns>The unquoted string.</returns>
        public static string AsUnquoted(this string value, bool trim = false, string separator = ".")
        {
            if (trim)
            {
                value = value.Trim();
            }

            if (value.Length == 0)
            {
                return String.Empty;
            }
            else if (string.IsNullOrEmpty(separator))
            {
                return value.AsUnquoted();
            }
            else
            {
                var splitted = value.Split(separator.ToCharArray());
                if (splitted.Count() == 1)
                {
                    return value.AsUnquoted();
                }
                else
                {
                    return splitted.Select(s => s.AsUnquoted()).Join(separator);
                }
            }
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <returns>The quoted string.</returns>
        public static string AsQuoted(this string value)
        {
            if (!value.StartsWith("["))
            {
                value = string.Concat("[", value);
            }
            if (!value.EndsWith("]"))
            {
                value = string.Concat(value, "]");
            }
            return value;
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before quoting.</param>
        /// <param name="separator">The separator in which the quotes will be placed.</param>
        /// <returns>The quoted string.</returns>
        public static string AsQuoted(this string value, bool trim = false, string separator = ".")
        {
            if (trim)
            {
                value = value.Trim();
            }

            if (value.Length == 0)
            {
                return String.Empty;
            }
            else if (string.IsNullOrEmpty(separator))
            {
                return value.AsQuoted();
            }
            else
            {
                var splitted = value.Split(separator.ToCharArray());
                if (splitted.Count() == 1)
                {
                    return value.AsQuoted();
                }
                else
                {
                    return splitted.Select(s => s.AsQuoted()).Join(separator);
                }
            }
        }

        // AsEnumerable
        internal static IEnumerable<string> AsEnumerable(this string value)
        {
            yield return value;
        }

        // AsJoinQualifier
        internal static string AsJoinQualifier(this string value, string leftAlias, string rightAlias)
        {
            return string.Concat(leftAlias, ".", value.AsQuoted(), " = ", rightAlias, ".", value.AsQuoted());
        }

        // AsField
        internal static string AsField(this string value)
        {
            return value.AsQuoted();
        }

        // AsParameter
        internal static string AsParameter(this string value, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return index > 0 ? string.Concat(prefix, value.AsUnquoted(), "_", index) :
                string.Concat(prefix, value.AsUnquoted());
        }

        // AsAliasField
        internal static string AsAliasField(this string value, string alias)
        {
            return string.Concat(alias, ".", value.AsQuoted());
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this string value, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return string.Concat(AsParameter(value, index, prefix), " AS ", AsField(value));
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this string value, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return string.Concat(AsField(value), " = ", AsParameter(value, index, prefix));
        }

        // AsFieldAndAliasField
        internal static string AsFieldAndAliasField(this string value, string alias)
        {
            return string.Concat(AsField(value), " = ", alias, ".", AsField(value));
        }

        /* IEnumerable<string> */

        // AsFields
        internal static IEnumerable<string> AsFields(this IEnumerable<string> values)
        {
            return values?.Select(value => value.AsField());
        }

        // AsParameters
        internal static IEnumerable<string> AsParameters(this IEnumerable<string> values, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return values?.Select(value => value.AsParameter(index, prefix));
        }

        // AsAliasFields
        internal static IEnumerable<string> AsAliasFields(this IEnumerable<string> values, string alias)
        {
            return values?.Select(value => value.AsAliasField(alias));
        }

        // AsParametersAsFields
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<string> values, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return values?.Select(value => value.AsParameterAsField(index, prefix));
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<string> values, int index = 0, string prefix = Constant.DefaultParameterPrefix)
        {
            return values?.Select(value => value.AsFieldAndParameter(index, prefix));
        }

        // AsFieldsAndAliasFields
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<string> values, string alias)
        {
            return values?.Select(value => value.AsFieldAndAliasField(alias));
        }
    }
}
