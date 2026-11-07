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
    /// The SQL dialect of Vertica: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class VerticaSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="VerticaSchemaDialect"/> class.
        /// </summary>
        public VerticaSchemaDialect()
            : base("Vertica")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of Vertica.
        /// </summary>
        public static VerticaSchemaDialect Instance { get; } = new VerticaSchemaDialect();

        /// <summary>
        /// Gets the names of the functions of Vertica that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "now",
            "random",
            "sysdate",
            "uuid_generate"
        };

        /// <summary>
        /// Gets the operators of Vertica that the other dialects do not have.
        /// </summary>
        public override IReadOnlyList<string> NonPortableOperators { get; } = new[]
        {
            "::"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that Vertica adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^(now|sysdate)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^uuid_generate\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.RandomNumber, @"^random\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDatabase, @"^current_database\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^nextval\s*\(.*\)$")
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Rewrites a literal into the standard form: the casts that end it are removed.
        /// </summary>
        /// <param name="literal">The literal, as Vertica writes it.</param>
        /// <returns>The literal in the standard form.</returns>
        public override string NormalizeLiteral(string literal) =>
            SchemaLiteral.StripTrailingCast(literal,
                "::");

        #endregion
    }
}
