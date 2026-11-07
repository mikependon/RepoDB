#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL dialect of ClickHouse: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class ClickHouseSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseSchemaDialect"/> class.
        /// </summary>
        public ClickHouseSchemaDialect()
            : base("ClickHouse")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of ClickHouse.
        /// </summary>
        public static ClickHouseSchemaDialect Instance { get; } = new ClickHouseSchemaDialect();

        /// <summary>
        /// Gets the characters that quote an identifier in ClickHouse.
        /// </summary>
        public override IReadOnlyList<SchemaIdentifierQuote> IdentifierQuotes { get; } = new[]
        {
            new SchemaIdentifierQuote('`'),
            new SchemaIdentifierQuote('"')
        };

        /// <summary>
        /// Gets the names of the functions of ClickHouse that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "generateuuidv4",
            "now",
            "now64",
            "rand",
            "today"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that ClickHouse adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^(now|now64)\s*\(\s*\d*\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^(today|current_date)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^generateuuidv4\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.RandomNumber, @"^rand\s*\(\s*\)$")
        };

        #endregion
    }
}
