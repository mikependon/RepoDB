using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="String"/>.
    /// </summary>
    public static class StringExtension
    {
        private static readonly Regex alphaNumericRegex = new(@"[^a-zA-Z0-9]", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        
        /// <summary>
        /// Joins an array string with a given separator.
        /// </summary>
        /// <param name="strings">The enumerable list of strings.</param>
        /// <param name="separator">The separator to be used.</param>
        /// <returns>A joined string from a given array of strings separated by the defined separator.</returns>
        public static string Join(this IEnumerable<string> strings,
            string separator) =>
            Join(strings, separator, true);

        /// <summary>
        /// Joins an array string with a given separator.
        /// </summary>
        /// <param name="strings">The enumerable list of strings.</param>
        /// <param name="separator">The separator to be used.</param>
        /// <param name="trim">The boolean value that indicates whether to trim each string before joining.</param>
        /// <returns>A joined string from a given array of strings separated by the defined separator.</returns>
        public static string Join(this IEnumerable<string> strings,
            string separator,
            bool trim)
        {
            if (trim)
            {
                strings = strings.Select(s => s.Trim());
            }
            return string.Join(separator, strings);
        }

        /// <summary>
        /// Removes the non-alphanumeric characters.
        /// </summary>
        /// <param name="value">The string value where the non-alphanumeric characters will be removed.</param>
        /// <returns>The alphanumeric string.</returns>
        public static string AsAlphaNumeric(this string value) =>
            AsAlphaNumeric(value, true);

        /// <summary>
        /// Removes the non-alphanumeric characters.
        /// </summary>
        /// <param name="value">The string value where the non-alphanumeric characters will be removed.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before removing the non-alphanumeric characters.</param>
        /// <returns>The alphanumeric string.</returns>
        public static string AsAlphaNumeric(this string value,
            bool trim)
        {
            if (trim)
            {
                value = value.Trim();
            }

            return alphaNumericRegex.Replace(value, "_");
        }

        /// <summary>
        /// Unquotes a string.
        /// </summary>
        /// <param name="value">The string value to be unqouted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The unquoted string.</returns>
        public static string AsUnquoted(this string value,
            IDbSetting dbSetting) =>
            AsUnquoted(value, false, dbSetting);

        /// <summary>
        /// Unquotes a string.
        /// </summary>
        /// <param name="value">The string value to be unqouted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before unquoting.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The unquoted string.</returns>
        public static string AsUnquoted(this string value,
            bool trim,
            IDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                return value;
            }
            if (value.IndexOf(StringConstant.Period) < 0)
            {
                return value.AsUnquotedInternal(trim, dbSetting);
            }
            else
            {
                var splitted = value.Split(StringConstant.Period.ToCharArray());
                return splitted.Select(s => s.AsUnquotedInternal(trim, dbSetting)).Join(StringConstant.Period);
            }
        }

        /// <summary>
        /// Unquotes a string.
        /// </summary>
        /// <param name="value">The string value to be unqouted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before quoting.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The unquoted string.</returns>
        private static string AsUnquotedInternal(this string value,
            bool trim,
            IDbSetting dbSetting)
        {
            if (!string.IsNullOrWhiteSpace(dbSetting.OpeningQuote))
            {
                value = value.Replace(dbSetting.OpeningQuote, string.Empty);
            }
            if (!string.IsNullOrWhiteSpace(dbSetting.ClosingQuote))
            {
                value = value.Replace(dbSetting.ClosingQuote, string.Empty);
            }
            if (trim)
            {
                value = value.Trim();
            }
            return value;
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The quoted string.</returns>
        public static string AsQuoted(this string value,
            IDbSetting dbSetting) =>
            AsQuoted(value, false, false, dbSetting);

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before quoting.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The quoted string.</returns>
        public static string AsQuoted(this string value,
            bool trim,
            IDbSetting dbSetting) =>
            AsQuoted(value, trim, false, dbSetting);

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before quoting.</param>
        /// <param name="ignoreSchema">The boolean value that indicates whether to ignore the schema.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The quoted string.</returns>
        public static string AsQuoted(this string value,
            bool trim,
            bool ignoreSchema,
            IDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                return value;
            }
            if (trim)
            {
                value = value.Trim();
            }
            var firstIndex = value.IndexOf(StringConstant.Period);
            if (ignoreSchema || firstIndex < 0)
            {
                return value.AsQuotedInternal(dbSetting);
            }
            else
            {
                return AsQuotedForDatabaseSchemaTableInternal(value, dbSetting);
            }
        }

        /// <summary>
        /// Quotes a string for database, schema and a table.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The quoted string.</returns>
        private static string AsQuotedForDatabaseSchemaTableInternal(this string value,
            IDbSetting dbSetting)
        {
            // TODO: Refactor this method
            var splitted = value.Split(StringConstant.Period.ToCharArray());
            if (splitted.Length > 2)
            {
                var list = new List<string>(splitted.Length);
                var current = default(string);
                foreach (var item in splitted)
                {
                    if (!string.IsNullOrWhiteSpace(current))
                    {
                        if (current.StartsWith(dbSetting.OpeningQuote) && item.EndsWith(dbSetting.ClosingQuote))
                        {
                            list.Add(string.Concat(current, StringConstant.Period, item));
                            current = null;
                        }
                    }
                    else
                    {
                        if (item.StartsWith(dbSetting.OpeningQuote))
                        {
                            if (item.EndsWith(dbSetting.ClosingQuote))
                            {
                                list.Add(item.AsQuotedInternal(dbSetting));
                            }
                            else
                            {
                                current = item;
                            }
                        }
                        else
                        {
                            list.Add(item.AsQuotedInternal(dbSetting));
                        }
                    }
                }
                if (current != null)
                {
                    list.Add(current.AsQuotedInternal(dbSetting));
                }
                return string.Join(StringConstant.Period, list);
            }
            else
            {
                return string.Join(StringConstant.Period, splitted.Select(item => item.AsQuotedInternal(dbSetting)));
            }
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The quoted string.</returns>
        private static string AsQuotedInternal(this string value,
            IDbSetting dbSetting)
        {
            if (!value.StartsWith(dbSetting.OpeningQuote))
            {
                value = string.Concat(dbSetting.OpeningQuote, value);
            }
            if (!value.EndsWith(dbSetting.ClosingQuote))
            {
                value = string.Concat(value, dbSetting.ClosingQuote);
            }
            return value;
        }

        /// <summary>
        /// Returns the string as a field name in the database.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="dbSetting">The <see cref="IDbSetting"/> object to be used.</param>
        /// <returns>The string value represented as database field.</returns>
        public static string AsField(this string value,
            IDbSetting dbSetting) =>
            value.AsQuoted(true, true, dbSetting);

        /// <summary>
        /// Returns the string as a parameter name in the database.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="dbSetting">The <see cref="IDbSetting"/> object to be used.</param>
        /// <returns>The string value represented as database parameter.</returns>
        public static string AsParameter(this string value,
            IDbSetting dbSetting) =>
            AsParameter(value, 0, dbSetting);

        /// <summary>
        /// Returns the string as a parameter name in the database.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="dbSetting">The <see cref="IDbSetting"/> object to be used.</param>
        /// <returns>The string value represented as database parameter.</returns>
        public static string AsParameter(this string value,
            int index,
            IDbSetting dbSetting)
        {
            if (dbSetting != null)
            {
                value = string.Concat(dbSetting.ParameterPrefix,
                    (value.StartsWith(dbSetting.ParameterPrefix) ? value.Substring(1) : value)
                        .AsUnquoted(true, dbSetting).AsAlphaNumeric());
            }
            return index > 0 ? string.Concat(value, "_", index.ToString()) : value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftAlias"></param>
        /// <param name="rightAlias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsJoinQualifier(this string value,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting) =>
            string.Concat(leftAlias, StringConstant.Period, value.AsQuoted(true, true, dbSetting), " = ",
                rightAlias, StringConstant.Period, value.AsQuoted(true, true, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="alias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsAliasField(this string value,
            string alias,
            IDbSetting dbSetting) =>
            string.Concat(alias, StringConstant.Period, value.AsQuoted(true, true, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsParameterAsField(this string value,
            int index,
            IDbSetting dbSetting) =>
            string.Concat(AsParameter(value, index, dbSetting), " AS ", AsField(value, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsFieldAndParameter(this string value,
            int index,
            IDbSetting dbSetting) =>
            string.Concat(AsField(value, dbSetting), " = ", AsParameter(value, index, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftAlias"></param>
        /// <param name="rightAlias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static string AsFieldAndAliasField(this string value,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting) =>
            string.Concat(leftAlias, StringConstant.Period, AsField(value, dbSetting), " = ", rightAlias, StringConstant.Period, AsField(value, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsFields(this IEnumerable<string> values,
            IDbSetting dbSetting) =>
            values?.Select(value => value.AsField(dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="alias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsAliasFields(this IEnumerable<string> values,
            string alias,
            IDbSetting dbSetting) =>
            values?.Select(value => value.AsAliasField(alias, dbSetting));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="leftAlias"></param>
        /// <param name="rightAlias"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<string> values,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting) =>
            values?.Select(value => value.AsFieldAndAliasField(leftAlias, rightAlias, dbSetting));
    }
}
