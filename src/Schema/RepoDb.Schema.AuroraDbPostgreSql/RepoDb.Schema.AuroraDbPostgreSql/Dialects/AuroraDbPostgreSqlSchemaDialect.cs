#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL dialect of Aurora PostgreSQL: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class AuroraDbPostgreSqlSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="AuroraDbPostgreSqlSchemaDialect"/> class.
        /// </summary>
        public AuroraDbPostgreSqlSchemaDialect()
            : base("PostgreSql")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of Aurora PostgreSQL.
        /// </summary>
        public static AuroraDbPostgreSqlSchemaDialect Instance { get; } = new AuroraDbPostgreSqlSchemaDialect();

        /// <summary>
        /// Gets the names of the functions of Aurora PostgreSQL that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "clock_timestamp",
            "gen_random_uuid",
            "now",
            "random",
            "session_user",
            "statement_timestamp",
            "timeofday",
            "transaction_timestamp",
            "user",
            "uuid_generate_v4",
            "version"
        };

        /// <summary>
        /// Gets the operators of Aurora PostgreSQL that the other dialects do not have.
        /// </summary>
        public override IReadOnlyList<string> NonPortableOperators { get; } = new[]
        {
            "::"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that Aurora PostgreSQL adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^(now|transaction_timestamp|statement_timestamp|clock_timestamp)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampUtc, @"^timezone\s*\(\s*'utc'\s*,\s*(now\s*\(\s*\)|current_timestamp)\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampUtc, @"^(now\s*\(\s*\)|current_timestamp)\s+at\s+time\s+zone\s+'utc'$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^(gen_random_uuid|uuid_generate_v4)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.RandomNumber, @"^random\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDatabase, @"^current_database\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^nextval\s*\(.*\)$")
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Rewrites a literal into the standard form: the casts that end it are removed, and a binary written as a hexadecimal text becomes a hexadecimal literal.
        /// A text with escapes (<c>E'...'</c>) is not understood.
        /// </summary>
        /// <param name="literal">The literal, as Aurora PostgreSQL writes it.</param>
        /// <returns>The literal in the standard form, or <c>null</c> if it is not understood.</returns>
        public override string NormalizeLiteral(string literal)
        {
            var stripped = SchemaLiteral.StripTrailingCast(literal,
                "::");
            return stripped.StartsWith("E'",
                StringComparison.OrdinalIgnoreCase) ? null : SchemaLiteral.ConvertQuotedHex(stripped,
                "\\x");
        }

        #endregion
    }
}
