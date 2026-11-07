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
    /// The SQL dialect of SQLite (Ahtola): what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class AhtolaSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="AhtolaSchemaDialect"/> class.
        /// </summary>
        public AhtolaSchemaDialect()
            : base("Sqlite")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of SQLite (Ahtola).
        /// </summary>
        public static AhtolaSchemaDialect Instance { get; } = new AhtolaSchemaDialect();

        /// <summary>
        /// Gets the characters that quote an identifier in SQLite (Ahtola).
        /// </summary>
        public override IReadOnlyList<SchemaIdentifierQuote> IdentifierQuotes { get; } = new[]
        {
            new SchemaIdentifierQuote('[', ']'),
            new SchemaIdentifierQuote('"'),
            new SchemaIdentifierQuote('`')
        };

        /// <summary>
        /// Gets the names of the functions of SQLite (Ahtola) that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "changes",
            "last_insert_rowid",
            "random",
            "randomblob",
            "total_changes"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that SQLite (Ahtola) adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^datetime\s*\(\s*'now'\s*,\s*'localtime'\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampUtc, @"^datetime\s*\(\s*'now'\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^date\s*\(\s*'now'(\s*,\s*'localtime')?\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.RandomNumber, @"^random\s*\(\s*\)$")
        };

        #endregion
    }
}
