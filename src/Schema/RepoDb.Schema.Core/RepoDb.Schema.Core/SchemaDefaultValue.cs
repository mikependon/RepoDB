#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Text.RegularExpressions;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that classifies the default expression of a column (see <see cref="ColumnDefaultValue"/>) in the languages of the databases, so the composer of another
    /// database can create the equivalent default value, and that cleans the text of the literals.
    /// </summary>
    internal static class SchemaDefaultValue
    {
        #region Private Variables

        private const RegexOptions Options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled;

        /// <summary>
        /// A number.
        /// </summary>
        private static readonly Regex Number = new Regex(@"^[+-]?\d+(\.\d+)?([eE][+-]?\d+)?$", Options);

        /// <summary>
        /// A text between quotes (a doubled quote is fine), with the prefix of its national character set or its charset (<c>N'x'</c>, <c>_utf8mb4'x'</c>) and the type cast
        /// of PostgreSQL (<c>'x'::character varying</c>).
        /// </summary>
        private static readonly Regex Text = new Regex(@"^(N|_[a-z0-9]+)?'(?<value>([^']|'')*)'(\s*::\s*[\w\s\.""]+(\(\d+(,\s*\d+)?\))?)?$", Options);

        /// <summary>
        /// The function whose call (or special register) is the default value, by the kind of default value. The first pattern that matches is the kind.
        /// </summary>
        private static readonly (ColumnDefaultValue Kind, Regex Pattern)[] Patterns =
        {
            (ColumnDefaultValue.Null, new Regex(@"^null$", Options)),
            (ColumnDefaultValue.BooleanTrue, new Regex(@"^true$", Options)),
            (ColumnDefaultValue.BooleanFalse, new Regex(@"^false$", Options)),
            (ColumnDefaultValue.CurrentDate, new Regex(@"^(current_date(\s*\(\s*\))?|curdate\s*\(\s*\)|today\s*\(\s*\)|current\s+date|date\s*\(\s*'now'(\s*,\s*'localtime')?\s*\)|trunc\s*\(\s*sysdate\s*\)|(cast\s*\(\s*(getdate|sysdatetime|now|current_timestamp)\s*(\(\s*\))?\s+as\s+date\s*\))|(convert\s*\(\s*date\s*,\s*(getdate|sysdatetime)\s*\(\s*\)\s*(,\s*\d+\s*)?\)))(\s*::\s*date)?$", Options)),
            (ColumnDefaultValue.CurrentTime, new Regex(@"^(current_time|localtime)(\s*\(\s*\d*\s*\))?$|^curtime\s*\(\s*\d*\s*\)$|^current\s+time$", Options)),
            (ColumnDefaultValue.CurrentTimestampUtc, new Regex(@"^((getutcdate|sysutcdatetime|utc_timestamp)\s*\(\s*\d*\s*\)|sys_extract_utc\s*\(\s*(systimestamp|current_timestamp)\s*\)|timezone\s*\(\s*'utc'\s*,\s*(now\s*\(\s*\)|current_timestamp)\s*\)|now\s*\(\s*\)\s+at\s+time\s+zone\s+'utc'|current_timestamp\s+at\s+time\s+zone\s+'utc'|datetime\s*\(\s*'now'\s*\))(\s*::\s*[\w\s]+)?$", Options)),
            (ColumnDefaultValue.CurrentTimestampWithOffset, new Regex(@"^(sysdatetimeoffset\s*\(\s*\)|systimestamp(\s*\(\s*\d*\s*\))?)$", Options)),
            (ColumnDefaultValue.CurrentTimestamp, new Regex(@"^((getdate|sysdatetime|now|transaction_timestamp|statement_timestamp|clock_timestamp)\s*\(\s*\)|now64\s*\(\s*\d*\s*\)|(current_timestamp|localtimestamp)(\s*\(\s*\d*\s*\))?|sysdate|current\s+timestamp|datetime\s*\(\s*'now'\s*,\s*'localtime'\s*\))(\s*::\s*[\w\s]+)?$", Options)),
            (ColumnDefaultValue.NewSequentialGuid, new Regex(@"^newsequentialid\s*\(\s*\)$", Options)),
            (ColumnDefaultValue.NewGuid, new Regex(@"^(newid|uuid|gen_random_uuid|uuid_generate_v4|sys_guid|random_uuid|generateuuidv4)\s*\(\s*\)(\s*::\s*[\w\s]+)?$", Options)),
            (ColumnDefaultValue.RandomNumber, new Regex(@"^(rand|random)\s*\(\s*[\d\.]*\s*\)$", Options)),
            (ColumnDefaultValue.CurrentUser, new Regex(@"^((suser_sname|suser_name|system_user|session_user|user_name|current_user|user)(\s*\(\s*\))?|sys_context\s*\(\s*'userenv'\s*,\s*'(session_user|current_user)'\s*\))$", Options)),
            (ColumnDefaultValue.CurrentDatabase, new Regex(@"^((db_name|database|current_database)\s*\(\s*\)|current_catalog)$", Options)),
            (ColumnDefaultValue.SequenceNextValue, new Regex(@"^(nextval\s*\(.*\)|next\s+value\s+for\s+.+|[\w\.""]+\.nextval|gen_id\s*\(.*\))(\s*::\s*[\w\s]+)?$", Options))
        };

        /// <summary>
        /// A word (the name of a function or of a column): an expression that has one is not known.
        /// </summary>
        private static readonly Regex Word = new Regex(@"[a-z_][a-z0-9_]*", Options);

        #endregion

        #region Public Methods

        /// <summary>
        /// Classifies the default expression of a column. The brackets of the names, the parentheses that wrap the expression (the ones that SQL Server writes, i.e.: <c>((0))</c>)
        /// and the type casts are not part of the classification.
        /// </summary>
        /// <param name="expression">The default expression, as it is read from the source database.</param>
        /// <returns>The kind of the default value: <see cref="ColumnDefaultValue.None"/> if there is no expression.</returns>
        public static ColumnDefaultValue Classify(string expression)
        {
            var text = Unwrap(expression);
            if (text.Length == 0)
            {
                return ColumnDefaultValue.None;
            }
            if (Number.IsMatch(text) || Text.IsMatch(text))
            {
                return ColumnDefaultValue.Literal;
            }
            var name = Unbracket(text);
            foreach (var (kind, pattern) in Patterns)
            {
                if (pattern.IsMatch(name))
                {
                    return kind;
                }
            }

            // An expression that calls a function, or uses a name, is not known; the one that only has literals and operators is an expression
            return Word.IsMatch(StripTexts(name)) ? ColumnDefaultValue.Unsupported : ColumnDefaultValue.Expression;
        }

        /// <summary>
        /// Gets the text of the literal that is the default expression: without the parentheses that wrap it, the prefix of the text (<c>N'x'</c>) and the type cast (<c>'x'::text</c>).
        /// </summary>
        /// <param name="expression">The default expression (of a column whose kind is <see cref="ColumnDefaultValue.Literal"/>).</param>
        /// <returns>The literal, i.e.: <c>0</c> or <c>'N/A'</c>; the expression without the parentheses if it is not a text.</returns>
        public static string GetLiteral(string expression)
        {
            var text = Unwrap(expression);
            var match = Text.Match(text);
            return match.Success ? $"'{match.Groups["value"].Value}'" : text;
        }

        /// <summary>
        /// Removes the parentheses that wrap the whole expression, as many times as they wrap it (<c>((0))</c> is <c>0</c>; <c>(a) + (b)</c> is not changed).
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The expression, trimmed and without the parentheses that wrap it (an empty text if there is no expression).</returns>
        public static string Unwrap(string expression)
        {
            var text = expression?.Trim() ?? string.Empty;
            while (text.Length >= 2 && text[0] == '(' && text[text.Length - 1] == ')' && IsWrapped(text))
            {
                text = text.Substring(1, text.Length - 2).Trim();
            }
            return text;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks whether the first parenthesis of the text is closed by the last one (the parentheses inside of the texts between quotes are not counted).
        /// </summary>
        /// <param name="text">The text, that starts with <c>(</c> and ends with <c>)</c>.</param>
        /// <returns><c>true</c> if the parentheses wrap the whole text.</returns>
        private static bool IsWrapped(string text)
        {
            var depth = 0;
            var quoted = false;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '\'')
                {
                    quoted = !quoted;
                }
                else if (!quoted && c == '(')
                {
                    depth++;
                }
                else if (!quoted && c == ')')
                {
                    depth--;
                    if (depth == 0 && i < text.Length - 1)
                    {
                        return false;
                    }
                }
            }
            return depth == 0;
        }

        /// <summary>
        /// Removes the quotes of the names (<c>[getdate]</c>, <c>"now"</c>, <c>`uuid`</c>) of the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The text without the quotes of the names.</returns>
        private static string Unbracket(string text) =>
            text.Replace("[", string.Empty).Replace("]", string.Empty).Replace("`", string.Empty);

        /// <summary>
        /// Removes the texts between quotes, so the words that they have are not taken for names.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The text without the texts between quotes.</returns>
        private static string StripTexts(string text) =>
            Regex.Replace(text, @"'([^']|'')*'", string.Empty, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

        #endregion
    }
}
