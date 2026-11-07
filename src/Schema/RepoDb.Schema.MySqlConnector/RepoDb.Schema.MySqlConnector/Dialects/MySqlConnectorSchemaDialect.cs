#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// The SQL dialect of MySQL (MySqlConnector): what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class MySqlConnectorSchemaDialect : SchemaDialect
    {
        #region Private Variables

        private static readonly Regex Introducer = new Regex(@"^_[a-z0-9]+\s*(?=')",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
            TimeSpan.FromSeconds(1));

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorSchemaDialect"/> class.
        /// </summary>
        public MySqlConnectorSchemaDialect()
            : base("MySql")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of MySQL (MySqlConnector).
        /// </summary>
        public static MySqlConnectorSchemaDialect Instance { get; } = new MySqlConnectorSchemaDialect();

        /// <summary>
        /// Gets the characters that quote an identifier in MySQL (MySqlConnector).
        /// </summary>
        public override IReadOnlyList<SchemaIdentifierQuote> IdentifierQuotes { get; } = new[]
        {
            new SchemaIdentifierQuote('`')
        };

        /// <summary>
        /// Gets the names of the functions of MySQL (MySqlConnector) that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "connection_id",
            "curdate",
            "curtime",
            "database",
            "found_rows",
            "last_insert_id",
            "now",
            "rand",
            "row_count",
            "schema",
            "session_user",
            "sysdate",
            "system_user",
            "unix_timestamp",
            "user",
            "utc_date",
            "utc_time",
            "utc_timestamp",
            "uuid",
            "uuid_short",
            "version"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that MySQL (MySqlConnector) adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^(current_date|curdate)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTime, @"^curtime\s*\(\s*\d*\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^(now|sysdate)\s*\(\s*\d*\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampUtc, @"^utc_timestamp\s*\(\s*\d*\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^uuid\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.RandomNumber, @"^rand\s*\(\s*[\d\.]*\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentUser, @"^(current_user|user|system_user|session_user)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDatabase, @"^(database|schema)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^(nextval\s*\(.*\)|next\s+value\s+for\s+.+)$")
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Rewrites a literal into the standard form: the introducer of its character set (<c>_utf8mb4'x'</c>) is removed, and a binary written as a hexadecimal number becomes a hexadecimal literal.
        /// </summary>
        /// <param name="literal">The literal, as MySQL (MySqlConnector) writes it.</param>
        /// <returns>The literal in the standard form.</returns>
        public override string NormalizeLiteral(string literal) =>
            SchemaLiteral.ConvertHexNumber(Introducer.Replace(literal,
                string.Empty));

        #endregion
    }
}
