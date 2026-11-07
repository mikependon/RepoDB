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
    /// The SQL dialect of Db2: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class Db2SchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Db2SchemaDialect"/> class.
        /// </summary>
        public Db2SchemaDialect()
            : base("Db2")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of Db2.
        /// </summary>
        public static Db2SchemaDialect Instance { get; } = new Db2SchemaDialect();

        /// <summary>
        /// Gets the patterns of the default expressions that Db2 adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^current\s+timestamp$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^current\s+date$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTime, @"^current\s+time$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentUser, @"^current\s+user$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^next\s+value\s+for\s+.+$")
        };

        /// <summary>
        /// Gets the keywords of Db2 that are not deterministic.
        /// </summary>
        protected override IEnumerable<string> OwnNonDeterministicKeywords => new[]
        {
            "current date",
            "current time",
            "current timestamp",
            "current user"
        };

        #endregion
    }
}
