#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that is used to define the SQL dialect of a database: what the library needs to know about the language of the database to carry the schema
    /// from it to another database. The extension of each database gives its own dialect, so the library knows none of them.
    /// </summary>
    public interface ISchemaDialect
    {
        #region Properties

        /// <summary>
        /// Gets the patterns of the default expressions of the dialect (i.e.: the function of the current date and time), each with the kind of default value that it tells.
        /// </summary>
        IReadOnlyList<SchemaDefaultPattern> DefaultPatterns { get; }

        /// <summary>
        /// Gets the name of the family of the dialect. Two databases of the same family write the same language, so nothing is converted between them.
        /// </summary>
        string Family { get; }

        /// <summary>
        /// Gets the characters that quote an identifier in the dialect.
        /// </summary>
        IReadOnlyList<SchemaIdentifierQuote> IdentifierQuotes { get; }

        /// <summary>
        /// Gets the names of the functions of the dialect that are not deterministic (the date and the time, the random values, the session and the server).
        /// </summary>
        IReadOnlyList<string> NonDeterministicFunctions { get; }

        /// <summary>
        /// Gets the keywords and the special registers of the dialect that are not deterministic, which are used without parentheses.
        /// </summary>
        IReadOnlyList<string> NonDeterministicKeywords { get; }

        /// <summary>
        /// Gets the operators of the dialect that the other dialects do not have (i.e.: the operator that casts a value).
        /// </summary>
        IReadOnlyList<string> NonPortableOperators { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks whether the native type of a column holds a date without a time.
        /// </summary>
        /// <param name="nativeType">The native type of the column.</param>
        /// <returns><c>true</c> if the native type holds a date only.</returns>
        bool IsDateOnly(string nativeType);

        /// <summary>
        /// Rewrites a literal of the dialect (i.e.: the one with a cast or a binary written in its own way) into the standard form that the library understands.
        /// </summary>
        /// <param name="literal">The literal, as the dialect writes it.</param>
        /// <returns>The literal in the standard form, or <c>null</c> if the literal is not understood.</returns>
        string NormalizeLiteral(string literal);

        #endregion
    }
}
