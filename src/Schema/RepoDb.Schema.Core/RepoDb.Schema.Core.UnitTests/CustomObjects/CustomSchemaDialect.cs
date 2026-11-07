#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// A dialect of a made-up database that the tests of the core use, so they do not depend on the dialect of any provider.
    /// It quotes with brackets, double quotes and backticks, and it has the functions, the keywords and the operator that the tests look for.
    /// </summary>
    public class CustomSchemaDialect : SchemaDialect
    {
        #region Private Variables

        private static readonly string[] Functions = { "getdate", "now", "random", "rand", "sys_guid", "uuid" };
        private static readonly SchemaDefaultPattern[] Patterns =
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^(getdate|now)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampUtc, @"^getutcdate\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^(newid|uuid)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.RandomNumber, @"^(rand|random)\s*\(\s*[\d\.]*\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^next\s+value\s+for\s+.+$")
        };
        private static readonly string[] Keywords = { "current timestamp", "sysdate", "systimestamp" };
        private static readonly string[] Operators = { "::" };
        private static readonly SchemaIdentifierQuote[] Quotes =
        {
            new SchemaIdentifierQuote('[', ']'),
            new SchemaIdentifierQuote('"'),
            new SchemaIdentifierQuote('`')
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CustomSchemaDialect"/> class.
        /// </summary>
        public CustomSchemaDialect() : base("Custom")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the single instance of the dialect.
        /// </summary>
        public static CustomSchemaDialect Instance { get; } = new CustomSchemaDialect();

        /// <summary>
        /// Gets the quotes of the identifiers.
        /// </summary>
        public override IReadOnlyList<SchemaIdentifierQuote> IdentifierQuotes => Quotes;

        /// <summary>
        /// Gets the names of the functions that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions => Functions;

        /// <summary>
        /// Gets the operators that the other dialects do not have.
        /// </summary>
        public override IReadOnlyList<string> NonPortableOperators => Operators;

        /// <summary>
        /// Gets the patterns of the default expressions that the dialect adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => Patterns;

        /// <summary>
        /// Gets the keywords that are not deterministic that the dialect adds to the standard ones.
        /// </summary>
        protected override IEnumerable<string> OwnNonDeterministicKeywords => Keywords;

        #endregion

        #region Public Methods

        /// <summary>
        /// Rewrites a literal into the standard form: the casts at its end are removed, the binaries are hexadecimal literals and the escape texts are not understood.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns>The literal in the standard form, or <c>null</c> if it is not understood.</returns>
        public override string NormalizeLiteral(string literal) =>
            literal.StartsWith("E'",
                StringComparison.OrdinalIgnoreCase) ? null : SchemaLiteral.ConvertQuotedHex(SchemaLiteral.ConvertHexNumber(SchemaLiteral.StripTrailingCast(literal,
                "::")),
                @"\x");

        #endregion
    }
}
