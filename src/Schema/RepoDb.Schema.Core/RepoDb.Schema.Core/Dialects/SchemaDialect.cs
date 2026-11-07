#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that defines the standard SQL dialect. The dialect of a database inherits it and gives only what the database writes differently.
    /// </summary>
    public class SchemaDialect : ISchemaDialect
    {
        #region Private Variables

        private static readonly string[] StandardKeywords =
        {
            "current_date",
            "current_time",
            "current_timestamp",
            "current_user",
            "localtime",
            "localtimestamp"
        };
        private static readonly SchemaDefaultPattern[] StandardPatterns =
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^current_date$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDatabase, @"^current_catalog$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTime, @"^(current_time|localtime)(\s*\(\s*\d*\s*\))?$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^(current_timestamp|localtimestamp)(\s*\(\s*\d*\s*\))?$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentUser, @"^(current_user|session_user|user)$")
        };
        private IReadOnlyList<SchemaDefaultPattern> _defaultPatterns;
        private IReadOnlyList<string> _nonDeterministicKeywords;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SchemaDialect"/> class.
        /// </summary>
        /// <param name="family">The name of the family of the dialect.</param>
        public SchemaDialect(string family)
        {
            Family = family ?? throw new ArgumentNullException(nameof(family));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the standard dialect, which is used when the dialect of a provider is not known.
        /// </summary>
        public static SchemaDialect Standard { get; } = new SchemaDialect("Standard");

        /// <summary>
        /// Gets the patterns of the default expressions: the standard ones, and the ones of the dialect.
        /// </summary>
        public IReadOnlyList<SchemaDefaultPattern> DefaultPatterns => _defaultPatterns ?? (_defaultPatterns = StandardPatterns.Concat(OwnDefaultPatterns).ToList());

        /// <summary>
        /// Gets the name of the family of the dialect.
        /// </summary>
        public string Family { get; }

        /// <summary>
        /// Gets the characters that quote an identifier. The standard one is the double quote.
        /// </summary>
        public virtual IReadOnlyList<SchemaIdentifierQuote> IdentifierQuotes { get; } = new[] { new SchemaIdentifierQuote('"') };

        /// <summary>
        /// Gets the names of the functions that are not deterministic. The standard dialect has none.
        /// </summary>
        public virtual IReadOnlyList<string> NonDeterministicFunctions => Array.Empty<string>();

        /// <summary>
        /// Gets the keywords that are not deterministic: the standard ones, and the ones of the dialect.
        /// </summary>
        public IReadOnlyList<string> NonDeterministicKeywords => _nonDeterministicKeywords ?? (_nonDeterministicKeywords = StandardKeywords.Concat(OwnNonDeterministicKeywords).ToList());

        /// <summary>
        /// Gets the operators of the dialect that the other dialects do not have. The standard dialect has none.
        /// </summary>
        public virtual IReadOnlyList<string> NonPortableOperators => Array.Empty<string>();

        /// <summary>
        /// Gets the patterns of the default expressions that the dialect adds to the standard ones.
        /// </summary>
        protected virtual IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => Array.Empty<SchemaDefaultPattern>();

        /// <summary>
        /// Gets the keywords that are not deterministic that the dialect adds to the standard ones.
        /// </summary>
        protected virtual IEnumerable<string> OwnNonDeterministicKeywords => Array.Empty<string>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks whether the native type holds a date without a time. In the standard, it is <c>date</c>.
        /// </summary>
        /// <param name="nativeType">The native type of the column.</param>
        /// <returns><c>true</c> if the native type holds a date only.</returns>
        public virtual bool IsDateOnly(string nativeType) =>
            string.Equals(nativeType?.Trim(),
                "date",
                StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Rewrites a literal into the standard form. The standard dialect has nothing to rewrite.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>The literal.</returns>
        public virtual string NormalizeLiteral(string literal) => literal;

        #endregion
    }
}
