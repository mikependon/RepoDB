#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;

namespace RepoDb.Schema
{
    /// <summary>
    /// Tells the SQL dialect of the provider that a schema is read from, so the expressions of its objects can be adapted to another database.
    /// The providers that share a dialect (i.e.: MySQL and MariaDB) are one family, and the identifier quotes are the ones that the family accepts.
    /// </summary>
    internal static class SchemaExpressionDialect
    {
        #region Private Variables

        private static readonly (char Open, char Close) Backtick = ('`', '`');
        private static readonly (char Open, char Close) Bracket = ('[', ']');
        private static readonly (char Open, char Close) DoubleQuote = ('"', '"');
        private static readonly Dictionary<string, string> Families = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["AHTOLA"] = "Sqlite",
            ["AURORAMYSQL"] = "MySql",
            ["CLICKHOUSE"] = "ClickHouse",
            ["DB2"] = "Db2",
            ["DUCKDB"] = "DuckDb",
            ["FIREBIRD"] = "Firebird",
            ["HANA"] = "SapHana",
            ["MARIADB"] = "MySql",
            ["MSSQL"] = "SqlServer",
            ["MSSQLITE"] = "Sqlite",
            ["MYSQL"] = "MySql",
            ["MYSQLC"] = "MySql",
            ["ORACLE"] = "Oracle",
            ["PGSQL"] = "PostgreSql",
            ["TURSO"] = "Sqlite",
            ["VERTICA"] = "Vertica"
        };
        private static readonly Dictionary<string, (char Open, char Close)[]> Quotes = new Dictionary<string, (char Open, char Close)[]>
        {
            ["ClickHouse"] = new[] { Backtick, DoubleQuote },
            ["MySql"] = new[] { Backtick },
            ["Sqlite"] = new[] { Bracket, DoubleQuote, Backtick },
            ["SqlServer"] = new[] { Bracket, DoubleQuote }
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the family of the dialect of the provider.
        /// </summary>
        /// <param name="provider">The name of the provider (see <see cref="RepoDb.Interfaces.IDbSetting.Provider"/>).</param>
        /// <returns>The family, or <c>null</c> if the provider is not known.</returns>
        public static string GetFamily(string provider) =>
            provider != null && Families.TryGetValue(provider,
                out var family) ? family : null;

        /// <summary>
        /// Gets the identifier quotes that the family accepts.
        /// </summary>
        /// <param name="family">The family of the dialect (see <see cref="GetFamily"/>).</param>
        /// <returns>The pairs of the opening and the closing quotes.</returns>
        public static IReadOnlyList<(char Open, char Close)> GetQuotes(string family) =>
            Quotes.TryGetValue(family,
                out var quotes) ? quotes : new[] { DoubleQuote };

        #endregion
    }
}
