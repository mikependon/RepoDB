#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Adapts the tokens of a SQL expression to another database: the quoted names are quoted with the quotes of the destination database,
    /// and the functions and the operators that are not portable are found.
    /// </summary>
    internal static class SchemaExpressionTranslator
    {
        #region Private Variables

        private static readonly HashSet<string> Keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "all", "and", "any", "as", "between", "case", "check", "else", "end", "exists", "in", "is", "like", "not", "or", "some", "then", "when"
        };
        private static readonly HashSet<string> PortableFunctions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "abs", "coalesce", "lower", "nullif", "round", "upper"
        };

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets what is not portable at the position of the token.
        /// </summary>
        /// <param name="tokens">The tokens of the expression.</param>
        /// <param name="index">The position of the token.</param>
        /// <returns>The function (i.e.: <c>getdate()</c>) or the operator that is not portable, or <c>null</c> if the token is portable.</returns>
        private static string GetNonPortableUsage(IList<SchemaExpressionToken> tokens,
            int index)
        {
            var token = tokens[index];
            if (token.Kind == SchemaExpressionTokenKind.Other && token.Text == "::")
            {
                return token.Text;
            }

            return token.Kind == SchemaExpressionTokenKind.Word && !Keywords.Contains(token.Text) && !PortableFunctions.Contains(token.Text) && IsCall(tokens,
                index) ? token.Text + "()" : null;
        }

        /// <summary>
        /// Checks whether the word is followed by an opening parenthesis, which makes it a call.
        /// </summary>
        /// <param name="tokens">The tokens of the expression.</param>
        /// <param name="index">The position of the word.</param>
        /// <returns><c>true</c> if the word is called.</returns>
        private static bool IsCall(IList<SchemaExpressionToken> tokens,
            int index)
        {
            for (var next = index + 1; next < tokens.Count; next++)
            {
                var token = tokens[next];
                if (token.Kind != SchemaExpressionTokenKind.Other || !string.IsNullOrWhiteSpace(token.Text))
                {
                    return token.Kind == SchemaExpressionTokenKind.Other && token.Text == "(";
                }
            }

            return false;
        }

        /// <summary>
        /// Quotes the name with the quotes of the destination database, where a closing quote inside of the name is doubled.
        /// </summary>
        /// <param name="name">The name without the quotes.</param>
        /// <param name="destination">The setting of the destination database.</param>
        /// <returns>The quoted name.</returns>
        private static string Quote(string name,
            IDbSetting destination)
        {
            var closing = destination.ClosingQuote ?? string.Empty;
            return string.Concat(destination.OpeningQuote,
                closing.Length == 0 ? name : name.Replace(closing,
                    closing + closing),
                closing);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds the first function or operator that can not be carried to another database as it is.
        /// Only the functions that the databases share (see the portable functions) are carried.
        /// </summary>
        /// <param name="tokens">The tokens of the expression.</param>
        /// <returns>The function (i.e.: <c>getdate()</c>) or the operator (<c>::</c>), or <c>null</c> if the expression is portable.</returns>
        public static string FindNonPortable(IList<SchemaExpressionToken> tokens) =>
            Enumerable.Range(0,
                tokens.Count).Select(index => GetNonPortableUsage(tokens,
                    index)).FirstOrDefault(usage => usage != null);

        /// <summary>
        /// Puts the tokens back together, with the quoted names quoted with the quotes of the destination database.
        /// </summary>
        /// <param name="tokens">The tokens of the expression.</param>
        /// <param name="destination">The setting of the destination database, which gives the quotes.</param>
        /// <returns>The expression for the destination database.</returns>
        public static string Translate(IEnumerable<SchemaExpressionToken> tokens,
            IDbSetting destination) =>
            string.Concat(tokens.Select(token => token.Kind == SchemaExpressionTokenKind.Identifier ? Quote(token.Name,
                destination) : token.Text));

        #endregion
    }
}
