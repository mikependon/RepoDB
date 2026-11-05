#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds a token of a SQL expression.
    /// </summary>
    internal sealed class SchemaExpressionToken
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SchemaExpressionToken"/> class.
        /// </summary>
        /// <param name="kind">The kind of the token.</param>
        /// <param name="text">The text of the token, as it is in the expression.</param>
        /// <param name="name">The name without the quotes, for a quoted name or a literal.</param>
        public SchemaExpressionToken(SchemaExpressionTokenKind kind,
            string text,
            string name = null)
        {
            Kind = kind;
            Text = text;
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the kind of the token.
        /// </summary>
        public SchemaExpressionTokenKind Kind { get; }

        /// <summary>
        /// Gets the name without the quotes (the escaped quotes are restored), for a quoted name or a literal.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the text of the token, as it is in the expression.
        /// </summary>
        public string Text { get; }

        #endregion
    }
}
