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
    /// The SQL dialect of Firebird: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class FirebirdSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdSchemaDialect"/> class.
        /// </summary>
        public FirebirdSchemaDialect()
            : base("Firebird")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of Firebird.
        /// </summary>
        public static FirebirdSchemaDialect Instance { get; } = new FirebirdSchemaDialect();

        /// <summary>
        /// Gets the names of the functions of Firebird that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "gen_id",
            "gen_uuid"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that Firebird adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^gen_id\s*\(.*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^gen_uuid\s*\(\s*\)$")
        };

        #endregion
    }
}
