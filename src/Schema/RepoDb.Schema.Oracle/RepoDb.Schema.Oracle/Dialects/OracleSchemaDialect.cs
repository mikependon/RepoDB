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
    /// The SQL dialect of Oracle: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class OracleSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="OracleSchemaDialect"/> class.
        /// </summary>
        public OracleSchemaDialect()
            : base("Oracle")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of Oracle.
        /// </summary>
        public static OracleSchemaDialect Instance { get; } = new OracleSchemaDialect();

        /// <summary>
        /// Gets the names of the functions of Oracle that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "sys_context",
            "sys_guid",
            "uid",
            "userenv"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that Oracle adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^sysdate$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampWithOffset, @"^systimestamp(\s*\(\s*\d*\s*\))?$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampUtc, @"^sys_extract_utc\s*\(\s*(systimestamp|current_timestamp)\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^trunc\s*\(\s*sysdate\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^sys_guid\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentUser, @"^sys_context\s*\(\s*'userenv'\s*,\s*'(session_user|current_user)'\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^[\w\.""]+\.nextval$")
        };

        /// <summary>
        /// Gets the keywords of Oracle that are not deterministic.
        /// </summary>
        protected override IEnumerable<string> OwnNonDeterministicKeywords => new[]
        {
            "dbtimezone",
            "sessiontimezone",
            "sysdate",
            "systimestamp"
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks whether the native type holds a date without a time. Oracle has none: its <c>DATE</c> holds a time.
        /// </summary>
        /// <param name="nativeType">The native type of the column.</param>
        /// <returns>Always <c>false</c>.</returns>
        public override bool IsDateOnly(string nativeType) => false;

        #endregion
    }
}
