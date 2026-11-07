#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that classifies the default expression of a column (see <see cref="ColumnDefaultValue"/>) in the language of its dialect, so the composer of another
    /// database can create the equivalent default value, and that cleans the text of the literals.
    /// </summary>
    public static class SchemaDefaultValue
    {
        #region Private Variables

        private static readonly ConcurrentDictionary<string, Regex> Compiled = new ConcurrentDictionary<string, Regex>(StringComparer.Ordinal);
        private static readonly ColumnDefaultValue[] KindOrder =
        {
            ColumnDefaultValue.CurrentDate,
            ColumnDefaultValue.CurrentTime,
            ColumnDefaultValue.CurrentTimestampUtc,
            ColumnDefaultValue.CurrentTimestampWithOffset,
            ColumnDefaultValue.CurrentTimestamp,
            ColumnDefaultValue.NewSequentialGuid,
            ColumnDefaultValue.NewGuid,
            ColumnDefaultValue.RandomNumber,
            ColumnDefaultValue.CurrentUser,
            ColumnDefaultValue.CurrentDatabase,
            ColumnDefaultValue.SequenceNextValue
        };
        private static readonly Regex Number = new Regex(@"^[+-]?\d+(\.\d+)?([eE][+-]?\d+)?$",
            RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));
        private static readonly Regex Text = new Regex(@"^N?'(?<value>([^']|'')*)'$",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
            TimeSpan.FromSeconds(1));
        private static readonly Regex Word = new Regex(@"[a-z_][a-z0-9_]*",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
            TimeSpan.FromSeconds(1));

        #endregion

        #region Private Methods

        /// <summary>
        /// Classifies a name or a call that is not a literal: <c>NULL</c>, a boolean, a pattern of the dialect, or an expression that is not known.
        /// </summary>
        /// <param name="name">The text, without the quotes of the identifiers.</param>
        /// <param name="dialect">The dialect of the expression.</param>
        /// <returns>The kind of the default value.</returns>
        private static ColumnDefaultValue ClassifyName(string name,
            ISchemaDialect dialect)
        {
            switch (name.ToLowerInvariant())
            {
                case "null":
                    return ColumnDefaultValue.Null;
                case "true":
                    return ColumnDefaultValue.BooleanTrue;
                case "false":
                    return ColumnDefaultValue.BooleanFalse;
            }

            var kind = KindOrder.FirstOrDefault(item => dialect.DefaultPatterns.Any(pattern => pattern.Kind == item && GetRegex(pattern.Pattern).IsMatch(name)));
            if (kind != ColumnDefaultValue.Unclassified)
            {
                return kind;
            }

            // An expression that calls a function, or uses a name, is not known; the one that only has literals and operators is an expression
            return Word.IsMatch(StripTexts(name)) ? ColumnDefaultValue.Unsupported : ColumnDefaultValue.Expression;
        }

        /// <summary>
        /// Gets the regular expression of a pattern of a dialect, which is compiled once.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>The regular expression.</returns>
        private static Regex GetRegex(string pattern) =>
            Compiled.GetOrAdd(pattern,
                key => new Regex(key,
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
                    TimeSpan.FromSeconds(1)));

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
        /// Removes the quotes of the identifiers of the dialect (i.e.: <c>[getdate]</c>) from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="quotes">The quotes of the identifiers of the dialect.</param>
        /// <returns>The text without the quotes of the identifiers.</returns>
        private static string StripQuotes(string text,
            IReadOnlyList<SchemaIdentifierQuote> quotes) =>
            quotes.Aggregate(text,
                (current,
                quote) => current.Replace(quote.Open.ToString(),
                    string.Empty).Replace(quote.Close.ToString(),
                    string.Empty));

        /// <summary>
        /// Removes the texts between quotes, so the words that they have are not taken for names.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The text without the texts between quotes.</returns>
        private static string StripTexts(string text) =>
            Regex.Replace(text,
                @"'([^']|'')*'",
                string.Empty,
                RegexOptions.CultureInvariant,
                TimeSpan.FromSeconds(1));

        #endregion

        #region Public Methods

        /// <summary>
        /// Classifies the default expression of a column. The parentheses that wrap the expression (i.e.: <c>((0))</c>), the quotes of the identifiers
        /// and what the dialect rewrites (see <see cref="ISchemaDialect.NormalizeLiteral"/>) are not part of the classification.
        /// </summary>
        /// <param name="expression">The default expression, as it is read from the source database.</param>
        /// <param name="dialect">The dialect of the source database (the standard one if it is <c>null</c>).</param>
        /// <returns>The kind of the default value: <see cref="ColumnDefaultValue.None"/> if there is no expression.</returns>
        public static ColumnDefaultValue Classify(string expression,
            ISchemaDialect dialect)
        {
            var source = dialect ?? SchemaDialect.Standard;
            var text = Unwrap(expression);
            if (text.Length == 0)
            {
                return ColumnDefaultValue.None;
            }

            var literal = source.NormalizeLiteral(text);
            if (literal == null)
            {
                return ColumnDefaultValue.Unsupported;
            }

            return Number.IsMatch(literal) || Text.IsMatch(literal) ? ColumnDefaultValue.Literal : ClassifyName(StripQuotes(literal,
                source.IdentifierQuotes),
                source);
        }

        /// <summary>
        /// Gets the text of the literal that is the default expression: without the parentheses that wrap it and the prefix of the text (<c>N'x'</c>).
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
                text = text.Substring(1,
                    text.Length - 2).Trim();
            }

            return text;
        }

        #endregion
    }
}
