#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Text.RegularExpressions;

namespace RepoDb.Schema
{
    /// <summary>
    /// Helps a dialect to rewrite its literals into the standard form (see <see cref="ISchemaDialect.NormalizeLiteral"/>).
    /// </summary>
    public static class SchemaLiteral
    {
        #region Private Variables

        private static readonly Regex CastType = new Regex(@"^\s*[\w\s\.""]+(\(\s*\d+(\s*,\s*\d+)?\s*\))?(\[\s*\])*\s*$",
            RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));
        private static readonly Regex HexNumber = new Regex(@"^0x(?<digits>[0-9a-f]*)$",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
            TimeSpan.FromSeconds(1));

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds the last cast operator that is outside of the parentheses and of the texts between quotes.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="castOperator">The operator that casts a value.</param>
        /// <returns>The position of the operator, or <c>-1</c> if there is none.</returns>
        private static int FindCast(string literal,
            string castOperator)
        {
            var found = -1;
            var depth = 0;
            var quoted = false;
            for (var index = 0; index < literal.Length; index++)
            {
                if (literal[index] == '\'')
                {
                    quoted = !quoted;
                }
                else if (!quoted && literal[index] == '(')
                {
                    depth++;
                }
                else if (!quoted && literal[index] == ')')
                {
                    depth--;
                }
                else if (!quoted && depth == 0 && string.CompareOrdinal(literal,
                    index,
                    castOperator,
                    0,
                    castOperator.Length) == 0)
                {
                    found = index;
                    index += castOperator.Length - 1;
                }
            }

            return found;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Rewrites a binary that is written as a hexadecimal number (<c>0xDEAD</c>) into the hexadecimal literal of the standard (<c>X'DEAD'</c>).
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>The hexadecimal literal, or the literal as it is if it is not a hexadecimal number.</returns>
        public static string ConvertHexNumber(string literal)
        {
            var match = HexNumber.Match(literal);
            return match.Success ? "X'" + match.Groups["digits"].Value + "'" : literal;
        }

        /// <summary>
        /// Rewrites a binary that is written as a text that starts with a marker (<c>'\xDEAD'</c>) into the hexadecimal literal of the standard (<c>X'DEAD'</c>).
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="marker">The marker that starts the text, i.e.: <c>\x</c>.</param>
        /// <returns>The hexadecimal literal, or the literal as it is if it does not start with the marker.</returns>
        public static string ConvertQuotedHex(string literal,
            string marker)
        {
            var match = Regex.Match(literal,
                "^'" + Regex.Escape(marker) + "(?<digits>[0-9a-f]*)'$",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(1));
            return match.Success ? "X'" + match.Groups["digits"].Value + "'" : literal;
        }

        /// <summary>
        /// Removes the casts that end a literal (<c>'x'::text</c> is <c>'x'</c>), as many times as they end it. A cast inside of the parentheses is not removed.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="castOperator">The operator that casts a value in the dialect, i.e.: <c>::</c>.</param>
        /// <returns>The literal without its casts.</returns>
        public static string StripTrailingCast(string literal,
            string castOperator)
        {
            var current = literal;
            var position = FindCast(current,
                castOperator);
            while (position >= 0 && CastType.IsMatch(current.Substring(position + castOperator.Length)))
            {
                current = current.Substring(0,
                    position).TrimEnd();
                position = FindCast(current,
                    castOperator);
            }

            return current;
        }

        #endregion
    }
}
