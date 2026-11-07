#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Splits a SQL expression into tokens: the literals, the quoted names, the words and the rest, so the quoted names are never confused with the text of a literal.
    /// </summary>
    internal static class SchemaExpressionTokenizer
    {
        #region Private Methods

        /// <summary>
        /// Checks whether the character can be a part of a number.
        /// </summary>
        /// <param name="value">The character.</param>
        /// <returns><c>true</c> if the character can be a part of a number.</returns>
        private static bool IsNumberPart(char value) =>
            char.IsLetterOrDigit(value) || value == '.';

        /// <summary>
        /// Checks whether the character can be a part of a word.
        /// </summary>
        /// <param name="value">The character.</param>
        /// <returns><c>true</c> if the character can be a part of a word.</returns>
        private static bool IsWordPart(char value) =>
            char.IsLetterOrDigit(value) || value == '_' || value == '$' || value == '#';

        /// <summary>
        /// Checks whether the character can start a word.
        /// </summary>
        /// <param name="value">The character.</param>
        /// <returns><c>true</c> if the character can start a word.</returns>
        private static bool IsWordStart(char value) =>
            char.IsLetter(value) || value == '_';

        /// <summary>
        /// Reads the operator at the position, which is a token by itself.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="position">The position to read from, which is moved after the token.</param>
        /// <returns>The token.</returns>
        private static SchemaExpressionToken ReadOperator(string expression,
            ref int position) =>
            new SchemaExpressionToken(SchemaExpressionTokenKind.Other,
                expression.Substring(position++,
                    1));

        /// <summary>
        /// Reads a text between quotes, where a doubled closing quote is a quote of the text. A text that is not closed is read, to the end, as any other text.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="position">The position of the opening quote, which is moved after the token.</param>
        /// <param name="closing">The closing quote.</param>
        /// <param name="kind">The kind of the token if the text is closed.</param>
        /// <returns>The token.</returns>
        private static SchemaExpressionToken ReadQuoted(string expression,
            ref int position,
            char closing,
            SchemaExpressionTokenKind kind)
        {
            var start = position++;
            var name = new StringBuilder();
            while (position < expression.Length)
            {
                var current = expression[position++];
                if (current != closing)
                {
                    name.Append(current);
                }
                else if (position < expression.Length && expression[position] == closing)
                {
                    name.Append(closing);
                    position++;
                }
                else
                {
                    return new SchemaExpressionToken(kind,
                        expression.Substring(start,
                            position - start),
                        name.ToString());
                }
            }

            return new SchemaExpressionToken(SchemaExpressionTokenKind.Other,
                expression.Substring(start));
        }

        /// <summary>
        /// Reads the token at the position.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="position">The position to read from, which is moved after the token.</param>
        /// <param name="quotes">The identifier quotes of the source database.</param>
        /// <returns>The token.</returns>
        private static SchemaExpressionToken ReadToken(string expression,
            ref int position,
            IReadOnlyList<SchemaIdentifierQuote> quotes)
        {
            var current = expression[position];
            foreach (var quote in quotes)
            {
                if (quote.Open == current)
                {
                    return ReadQuoted(expression,
                        ref position,
                        quote.Close,
                        SchemaExpressionTokenKind.Identifier);
                }
            }

            if (current == '\'')
            {
                return ReadQuoted(expression,
                    ref position,
                    current,
                    SchemaExpressionTokenKind.Literal);
            }

            if (IsWordStart(current))
            {
                return ReadWhile(expression,
                    ref position,
                    IsWordPart,
                    SchemaExpressionTokenKind.Word);
            }

            if (char.IsWhiteSpace(current))
            {
                return ReadWhile(expression,
                    ref position,
                    char.IsWhiteSpace,
                    SchemaExpressionTokenKind.Other);
            }

            return char.IsDigit(current) ? ReadWhile(expression,
                ref position,
                IsNumberPart,
                SchemaExpressionTokenKind.Other) : ReadOperator(expression,
                ref position);
        }

        /// <summary>
        /// Reads the characters that match the condition.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="position">The position to read from, which is moved after the token.</param>
        /// <param name="condition">The condition of the characters of the token.</param>
        /// <param name="kind">The kind of the token.</param>
        /// <returns>The token.</returns>
        private static SchemaExpressionToken ReadWhile(string expression,
            ref int position,
            Func<char, bool> condition,
            SchemaExpressionTokenKind kind)
        {
            var start = position;
            while (position < expression.Length && condition(expression[position]))
            {
                position++;
            }

            return new SchemaExpressionToken(kind,
                expression.Substring(start,
                    position - start));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Splits the expression into tokens. The text of the tokens, put together, is the expression.
        /// </summary>
        /// <param name="expression">The SQL expression.</param>
        /// <param name="quotes">The identifier quotes of the source database.</param>
        /// <returns>The tokens of the expression.</returns>
        public static IList<SchemaExpressionToken> Tokenize(string expression,
            IReadOnlyList<SchemaIdentifierQuote> quotes)
        {
            var tokens = new List<SchemaExpressionToken>();
            var position = 0;
            while (position < expression.Length)
            {
                tokens.Add(ReadToken(expression,
                    ref position,
                    quotes));
            }

            return tokens;
        }

        #endregion
    }
}
