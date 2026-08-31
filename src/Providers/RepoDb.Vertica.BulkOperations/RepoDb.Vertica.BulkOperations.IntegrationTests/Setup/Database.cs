#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.Vertica.BulkOperations.IntegrationTests.Models;
using System;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// A class used as a startup setup for the RepoDb Vertica bulk-operations test database.
    /// </summary>
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the connection string to be used for the Vertica database.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the creation of the database.
        /// </summary>
        public static void Initialize()
        {
            ConnectionString = Environment.GetEnvironmentVariable("REPODB_VERTICA_CONSTR") ??
                @"Host=127.0.0.1;Port=5433;Database=RepoDb;User=dbadmin;Password=RepoDB2026;Pooling=false;";

            GlobalConfiguration
                .Setup()
                .UseVertica(useInvariantCulture: true);

            CreateTables();
        }

        /// <summary>
        /// Clean up all the table.
        /// </summary>
        public static void Cleanup()
        {
            using var connection = new VerticaConnection(ConnectionString);
            connection.Truncate<BulkOperationIdentityTable>();
            connection.Truncate<BulkOperationNonIdentityTable>();
        }

        #endregion

        #region CreateTables

        /// <summary>
        /// Create the necessary tables for testing.
        /// </summary>
        public static void CreateTables()
        {
            CreateBulkOperationIdentityTable();
            CreateBulkOperationNonIdentityTable();
        }

        /// <summary>
        ///
        /// </summary>
        public static void CreateBulkOperationIdentityTable()
        {
            using var connection = new VerticaConnection(ConnectionString);

            connection.ExecuteNonQuery(@"DROP TABLE IF EXISTS ""BulkOperationIdentityTable"" CASCADE;");

            connection.ExecuteNonQuery(@"CREATE TABLE ""BulkOperationIdentityTable""
                (
                    ""Id"" IDENTITY(1, 1),
                    ""RowGuid"" UUID NOT NULL,
                    ""ColumnBit"" BOOLEAN,
                    ""ColumnDateTime"" TIMESTAMP,
                    ""ColumnDateTime2"" TIMESTAMP,
                    ""ColumnDecimal"" DECIMAL(18,2),
                    ""ColumnFloat"" DOUBLE PRECISION,
                    ""ColumnInt"" INTEGER,
                    ""ColumnNVarChar"" VARCHAR(2000),
                    PRIMARY KEY (""Id"")
                );");
        }

        /// <summary>
        ///
        /// </summary>
        public static void CreateBulkOperationNonIdentityTable()
        {
            using var connection = new VerticaConnection(ConnectionString);

            connection.ExecuteNonQuery(@"DROP TABLE IF EXISTS ""BulkOperationNonIdentityTable"" CASCADE;");

            connection.ExecuteNonQuery(@"CREATE TABLE ""BulkOperationNonIdentityTable""
                (
                    ""Id"" BIGINT NOT NULL,
                    ""RowGuid"" UUID NOT NULL,
                    ""ColumnBit"" BOOLEAN,
                    ""ColumnDateTime"" TIMESTAMP,
                    ""ColumnDateTime2"" TIMESTAMP,
                    ""ColumnDecimal"" DECIMAL(18,2),
                    ""ColumnFloat"" DOUBLE PRECISION,
                    ""ColumnInt"" INTEGER,
                    ""ColumnNVarChar"" VARCHAR(2000),
                    PRIMARY KEY (""Id"")
                );");
        }

        #endregion
    }
}
