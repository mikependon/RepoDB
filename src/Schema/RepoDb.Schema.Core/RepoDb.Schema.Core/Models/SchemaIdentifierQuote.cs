#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that holds the characters that quote an identifier in a dialect (i.e.: a name of a table or a column).
    /// </summary>
    public sealed class SchemaIdentifierQuote
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SchemaIdentifierQuote"/> class.
        /// </summary>
        /// <param name="open">The character that opens the identifier.</param>
        /// <param name="close">The character that closes the identifier.</param>
        public SchemaIdentifierQuote(char open,
            char close)
        {
            Open = open;
            Close = close;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SchemaIdentifierQuote"/> class that opens and closes with the same character.
        /// </summary>
        /// <param name="quote">The character that opens and closes the identifier.</param>
        public SchemaIdentifierQuote(char quote)
            : this(quote,
                quote)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the character that closes the identifier.
        /// </summary>
        public char Close { get; }

        /// <summary>
        /// Gets the character that opens the identifier.
        /// </summary>
        public char Open { get; }

        #endregion
    }
}
