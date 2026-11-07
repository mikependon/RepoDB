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
    /// The SQL dialect of DuckDB: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class DuckDbSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="DuckDbSchemaDialect"/> class.
        /// </summary>
        public DuckDbSchemaDialect()
            : base("DuckDb")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of DuckDB.
        /// </summary>
        public static DuckDbSchemaDialect Instance { get; } = new DuckDbSchemaDialect();

        /// <summary>
        /// Gets the names of the functions of DuckDB that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "gen_random_uuid",
            "now",
            "random",
            "today",
            "uuid"
        };

        /// <summary>
        /// Gets the operators of DuckDB that the other dialects do not have.
        /// </summary>
        public override IReadOnlyList<string> NonPortableOperators { get; } = new[]
        {
            "::"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that DuckDB adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^now\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^today\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^(uuid|gen_random_uuid)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.RandomNumber, @"^random\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDatabase, @"^current_database\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^nextval\s*\(.*\)$")
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Rewrites a literal into the standard form: the casts that end it are removed.
        /// </summary>
        /// <param name="literal">The literal, as DuckDB writes it.</param>
        /// <returns>The literal in the standard form.</returns>
        public override string NormalizeLiteral(string literal) =>
            SchemaLiteral.StripTrailingCast(literal,
                "::");

        #endregion
    }
}
