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
    /// The SQL dialect of SQL Server: what the library needs to know about its language to carry the schema to another database.
    /// </summary>
    public sealed class SqlServerSchemaDialect : SchemaDialect
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SqlServerSchemaDialect"/> class.
        /// </summary>
        public SqlServerSchemaDialect()
            : base("SqlServer")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dialect of SQL Server.
        /// </summary>
        public static SqlServerSchemaDialect Instance { get; } = new SqlServerSchemaDialect();

        /// <summary>
        /// Gets the characters that quote an identifier in SQL Server.
        /// </summary>
        public override IReadOnlyList<SchemaIdentifierQuote> IdentifierQuotes { get; } = new[]
        {
            new SchemaIdentifierQuote('[', ']'),
            new SchemaIdentifierQuote('"')
        };

        /// <summary>
        /// Gets the names of the functions of SQL Server that are not deterministic.
        /// </summary>
        public override IReadOnlyList<string> NonDeterministicFunctions { get; } = new[]
        {
            "getdate",
            "getutcdate",
            "newid",
            "newsequentialid",
            "rand",
            "session_user",
            "sysdatetime",
            "sysdatetimeoffset",
            "sysutcdatetime",
            "system_user",
            "user"
        };

        /// <summary>
        /// Gets the patterns of the default expressions that SQL Server adds to the standard ones.
        /// </summary>
        protected override IEnumerable<SchemaDefaultPattern> OwnDefaultPatterns => new[]
        {
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^convert\s*\(\s*date\s*,\s*(getdate|sysdatetime)\s*\(\s*\)\s*(,\s*\d+\s*)?\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDate, @"^cast\s*\(\s*(getdate|sysdatetime)\s*\(\s*\)\s+as\s+date\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestamp, @"^(getdate|sysdatetime)\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampUtc, @"^(getutcdate|sysutcdatetime)\s*\(\s*\d*\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentTimestampWithOffset, @"^sysdatetimeoffset\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewSequentialGuid, @"^newsequentialid\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.NewGuid, @"^newid\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.RandomNumber, @"^rand\s*\(\s*[\d\.]*\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentUser, @"^(suser_sname|suser_name|system_user|user_name)(\s*\(\s*\))?$"),
            new SchemaDefaultPattern(ColumnDefaultValue.CurrentDatabase, @"^db_name\s*\(\s*\)$"),
            new SchemaDefaultPattern(ColumnDefaultValue.SequenceNextValue, @"^next\s+value\s+for\s+.+$")
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Rewrites a literal into the standard form: a binary written as a hexadecimal number becomes a hexadecimal literal.
        /// </summary>
        /// <param name="literal">The literal, as SQL Server writes it.</param>
        /// <returns>The literal in the standard form.</returns>
        public override string NormalizeLiteral(string literal) =>
            SchemaLiteral.ConvertHexNumber(literal);

        #endregion
    }
}
