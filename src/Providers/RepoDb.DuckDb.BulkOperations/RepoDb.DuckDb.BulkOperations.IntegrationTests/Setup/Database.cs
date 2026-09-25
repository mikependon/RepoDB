#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using DuckDB.NET.Data;
using RepoDb.DuckDb.BulkOperations.IntegrationTests.Models;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// The startup setup of the DuckDB bulk operations test database.
    /// </summary>
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the connection string of the test database.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the test database.
        /// </summary>
        public static void Initialize()
        {
            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_DUCKDB_BULK_CONSTR") ??
                $"Data Source={Path.Combine(Path.GetTempPath(), "repodb_duckdb_bulk_tests.db")}";

            GlobalConfiguration
                .Setup()
                .UseDuckDb();

            CreateTables();
        }

        /// <summary>
        /// Cleans up the test tables.
        /// </summary>
        public static void Cleanup()
        {
            using var connection = new DuckDBConnection(ConnectionString);
            connection.Truncate<BulkOperationIdentityTable>();
            connection.Truncate<BulkOperationNonIdentityTable>();
        }

        #endregion

        #region CreateTables

        /// <summary>
        /// Creates the test tables.
        /// </summary>
        public static void CreateTables()
        {
            using var connection = new DuckDBConnection(ConnectionString);
            connection.ExecuteNonQuery(@"CREATE SEQUENCE IF NOT EXISTS seq_bulkoperationidentitytable_id START 1;");
            connection.ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS ""BulkOperationIdentityTable""
                (
                    ""Id"" BIGINT PRIMARY KEY DEFAULT nextval('seq_bulkoperationidentitytable_id'),
                    ""RowGuid"" UUID NOT NULL,
                    ""ColumnBit"" UTINYINT NULL,
                    ""ColumnDateTime"" TIMESTAMP NULL,
                    ""ColumnDateTime2"" TIMESTAMP NULL,
                    ""ColumnDecimal"" DECIMAL(18,2) NULL,
                    ""ColumnFloat"" DOUBLE NULL,
                    ""ColumnInt"" INTEGER NULL,
                    ""ColumnNVarChar"" VARCHAR NULL
                );");
            connection.ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS ""BulkOperationNonIdentityTable""
                (
                    ""Id"" BIGINT PRIMARY KEY,
                    ""RowGuid"" UUID NOT NULL,
                    ""ColumnBit"" UTINYINT NULL,
                    ""ColumnDateTime"" TIMESTAMP NULL,
                    ""ColumnDateTime2"" TIMESTAMP NULL,
                    ""ColumnDecimal"" DECIMAL(18,2) NULL,
                    ""ColumnFloat"" DOUBLE NULL,
                    ""ColumnInt"" INTEGER NULL,
                    ""ColumnNVarChar"" VARCHAR NULL
                );");
        }

        #endregion
    }
}
