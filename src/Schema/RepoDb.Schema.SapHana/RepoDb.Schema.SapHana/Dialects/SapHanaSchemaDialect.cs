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
    /// The SQL dialect of SAP HANA: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class SapHanaSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SapHanaSchemaDialect"/> class.
        /// </summary>
        public SapHanaSchemaDialect()
            : base("SapHana")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of SAP HANA.
        /// </summary>
        public static SapHanaSchemaDialect Instance { get; } = new SapHanaSchemaDialect();

        /// <summary>
        /// Gets the patterns of the default expressions that SAP HANA adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampUtc, @"^current_utctimestamp$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^sysuuid$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^[\w\.""]+\.nextval$")
        };

        /// <summary>
        /// Gets the keywords of SAP HANA that are not deterministic.
        /// </summary>
        protected override IEnumerable<string> OwnNonDeterministicKeywords => new[]
        {
            "current_utcdate",
            "current_utctime",
            "current_utctimestamp",
            "sysuuid"
        };

        #endregion
    }
}
