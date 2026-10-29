#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Text;

namespace RepoDb.Schema
{
    /// <summary>
    /// Finds the quoted identifiers of a SQL expression (<c>[name]</c>, <c>"name"</c> and <c>`name`</c>), skipping the string literals.
    /// The expression is not parsed: unquoted names, and names that are qualified or called as functions, are not returned.
    /// </summary>
    internal static class SchemaDefinitionExpressionScanner
    {
        #region Internal Methods

        /// <summary>
        /// Gets the distinct quoted identifiers of the expression that can name a column of the table of the expression.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <returns>The identifiers, in the order that they first appear.</returns>
        internal static IList<string> GetQuotedIdentifiers(string expression)
        {
            var identifiers = new List<string>();
            if (string.IsNullOrEmpty(expression))
            {
                return identifiers;
            }
            var position = 0;
            while (position < expression.Length)
            {
                var start = position;
                var identifier = ReadNext(expression,
                    ref position);
                if (identifier != null && !IsQualifiedOrCalled(expression, start, position) && !identifiers.Contains(identifier))
                {
                    identifiers.Add(identifier);
                }
            }
            return identifiers;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the next token of the expression, and moves the position past it.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="position">The position to read from. It is moved past the token that was read.</param>
        /// <returns>The identifier if the token is a quoted identifier; otherwise, <c>null</c>.</returns>
        private static string ReadNext(string expression,
            ref int position)
        {
            var current = expression[position];
            if (current == '\'')
            {
                position = SkipQuoted(expression, position, '\'');
                return null;
            }
            var closing = GetClosingQuote(current);
            if (closing == '\0')
            {
                position++;
                return null;
            }
            return ReadIdentifier(expression,
                ref position,
                closing);
        }

        /// <summary>
        /// Gets the character that closes an identifier that is opened by the character.
        /// </summary>
        /// <param name="opening">The character that may open an identifier.</param>
        /// <returns>The closing character, or <c>\0</c> if the character does not open an identifier.</returns>
        private static char GetClosingQuote(char opening)
        {
            switch (opening)
            {
                case '[':
                    return ']';
                case '"':
                    return '"';
                case '`':
                    return '`';
                default:
                    return '\0';
            }
        }

        /// <summary>
        /// Reads a quoted identifier, where the closing character is escaped by doubling it.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="position">The position of the opening character. It is moved past the closing character.</param>
        /// <param name="closing">The character that closes the identifier.</param>
        /// <returns>The identifier, without its quotes.</returns>
        private static string ReadIdentifier(string expression,
            ref int position,
            char closing)
        {
            var identifier = new StringBuilder();
            position++;
            while (position < expression.Length)
            {
                var current = expression[position];
                if (current == closing && position + 1 < expression.Length && expression[position + 1] == closing)
                {
                    identifier.Append(closing);
                    position += 2;
                    continue;
                }
                position++;
                if (current == closing)
                {
                    break;
                }
                identifier.Append(current);
            }
            return identifier.ToString();
        }

        /// <summary>
        /// Skips a quoted literal, where the closing quote is escaped by doubling it.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="start">The position of the opening quote.</param>
        /// <param name="quote">The quote character.</param>
        /// <returns>The position after the closing quote (or the end of the expression if the literal is not closed).</returns>
        private static int SkipQuoted(string expression,
            int start,
            char quote)
        {
            var position = start + 1;
            while (position < expression.Length)
            {
                if (expression[position] == quote)
                {
                    if (position + 1 < expression.Length && expression[position + 1] == quote)
                    {
                        position += 2;
                        continue;
                    }
                    return position + 1;
                }
                position++;
            }
            return position;
        }

        /// <summary>
        /// Checks whether the identifier is part of a qualified name (<c>[dbo].[name]</c>) or is called as a function (<c>[name](</c>).
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="start">The position where the identifier starts.</param>
        /// <param name="end">The position after the identifier.</param>
        /// <returns><c>true</c> if the identifier cannot be a column of the table; otherwise, <c>false</c>.</returns>
        private static bool IsQualifiedOrCalled(string expression,
            int start,
            int end)
        {
            var before = start - 1;
            while (before >= 0 && char.IsWhiteSpace(expression[before]))
            {
                before--;
            }
            var after = end;
            while (after < expression.Length && char.IsWhiteSpace(expression[after]))
            {
                after++;
            }
            return (before >= 0 && expression[before] == '.') ||
                (after < expression.Length && (expression[after] == '.' || expression[after] == '('));
        }

        #endregion
    }
}
