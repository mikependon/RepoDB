#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.AuroraDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// A class used as a startup setup for the RepoDb Aurora PostgreSQL bulk-operations test database.
    /// </summary>
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the connection string used to reach the Aurora PostgreSQL server itself (the <c>postgres</c>
        /// maintenance database) - used only to create the target database if it does not already exist.
        /// </summary>
        public static string ConnectionStringForSystem { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the Aurora PostgreSQL test database.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the creation of the database.
        /// </summary>
        public static void Initialize()
        {
            ConnectionStringForSystem =
                Environment.GetEnvironmentVariable("REPODB_AURORADB_CONSTR_SYSTEM") ??
                "Server=127.0.0.1;Port=5435;Database=postgres;User Id=postgres;Password=RepoDB2026;";

            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_AURORADB_CONSTR_BULK") ??
                "Server=127.0.0.1;Port=5435;Database=RepoDbBulk;User Id=postgres;Password=RepoDB2026;";

            // Initialize AuroraDb
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();

            // Create the database first
            CreateDatabase();

            // Create the tables
            CreateTables();
        }

        /// <summary>
        /// Creates the <c>RepoDbBulk</c> database if it does not already exist. Unlike MariaDB's
        /// <c>CREATE DATABASE IF NOT EXISTS</c>, PostgreSQL has no such guarded form - creating a
        /// database is done against the <c>postgres</c> maintenance database, and a duplicate name is a
        /// hard error, so existence is checked first via <c>pg_database</c>.
        /// </summary>
        public static void CreateDatabase()
        {
            using var connection = new AuroraDbConnection(ConnectionStringForSystem);
            var exists = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM pg_database WHERE datname = 'RepoDbBulk';") > 0;
            if (!exists)
            {
                connection.ExecuteNonQuery("CREATE DATABASE \"RepoDbBulk\";");
            }
        }

        /// <summary>
        /// Clean up all the table.
        /// </summary>
        public static void Cleanup()
        {
            using var connection = new AuroraDbConnection(ConnectionString);
            connection.Truncate<BulkOperationIdentityTable>();
            connection.Truncate<BulkOperationNonIdentityTable>();
        }

        #endregion

        #region Aurora

        private static bool? isAuroraCluster;

        /// <summary>
        /// Gets a value that indicates whether the target server is an Amazon Aurora PostgreSQL cluster (the <c>aurora_replica_status()</c>
        /// function exists), as opposed to a plain PostgreSQL server standing in for it (e.g. the local docker container or the CI service).
        /// </summary>
        public static bool IsAuroraCluster
        {
            get
            {
                if (isAuroraCluster == null)
                {
                    using var connection = new AuroraDbConnection(ConnectionString);
                    isAuroraCluster = connection.ExecuteScalar<bool>("SELECT to_regproc('pg_catalog.aurora_replica_status') IS NOT NULL;");
                }
                return isAuroraCluster.Value;
            }
        }

        /// <summary>
        /// Marks the current test as inconclusive if the target server is not an Amazon Aurora PostgreSQL cluster.
        /// </summary>
        public static void AssumeAuroraCluster()
        {
            if (!IsAuroraCluster)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Inconclusive("The test requires an Amazon Aurora PostgreSQL cluster. On a plain PostgreSQL server, " +
                    "the dialect probe of the AWS wrapper ('aurora_replica_status()') aborts the ambient transaction of a TransactionScope.");
            }
        }

        /// <summary>
        /// Marks the current test as inconclusive if the target server is an Amazon Aurora PostgreSQL cluster.
        /// </summary>
        public static void AssumeNonAuroraServer()
        {
            if (IsAuroraCluster)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Inconclusive("The test only applies to a server that is not an Amazon Aurora PostgreSQL cluster.");
            }
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
        /// Creates an identity table that has some important fields. All fields are nullable except
        /// <c>Id</c> and <c>RowGuid</c>. <c>Id</c> is <c>BIGINT GENERATED ALWAYS AS IDENTITY</c> -
        /// Aurora PostgreSQL's native identity column, the direct equivalent of MariaDB's
        /// <c>AUTO_INCREMENT</c> and reported back by <c>RETURNING</c> (see
        /// <c>AuroraDbText.GetInsertFromPseudoTableForReturnIdentitySql</c>) rather than a pre-assigned
        /// session-variable value. <c>RowGuid</c> is a native <c>UUID</c> column, which Npgsql binds
        /// directly to/from <see cref="Guid"/> - no property handler required.
        /// </summary>
        public static void CreateBulkOperationIdentityTable()
        {
            var commandText = @"
                CREATE TABLE IF NOT EXISTS ""BulkOperationIdentityTable""
                (
                    ""Id"" BIGINT GENERATED ALWAYS AS IDENTITY,
                    ""RowGuid"" UUID NOT NULL,
                    ""ColumnBit"" SMALLINT NULL,
                    ""ColumnDateTime"" TIMESTAMP NULL,
                    ""ColumnDateTime2"" TIMESTAMP(6) NULL,
                    ""ColumnDecimal"" DECIMAL(18,2) NULL,
                    ""ColumnFloat"" DOUBLE PRECISION NULL,
                    ""ColumnInt"" INTEGER NULL,
                    ""ColumnNVarChar"" VARCHAR(2000) NULL,
                    PRIMARY KEY (""Id"")
                );";
            using var connection = new AuroraDbConnection(ConnectionString);
            connection.ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// Creates a non-identity table that has some important fields. All fields are nullable except
        /// the primary key. Unlike <see cref="CreateBulkOperationIdentityTable"/>, <c>Id</c> here is a
        /// plain <c>BIGINT</c> primary key (no <c>GENERATED ALWAYS AS IDENTITY</c>) - the caller's value
        /// is stored as-is. Used by tests that need to know a row's <c>Id</c> ahead of time (e.g.
        /// matching against a separately-built anonymous/expando object by primary key), which a
        /// server-generated identity value can't support.
        /// </summary>
        public static void CreateBulkOperationNonIdentityTable()
        {
            var commandText = @"
                CREATE TABLE IF NOT EXISTS ""BulkOperationNonIdentityTable""
                (
                    ""Id"" BIGINT NOT NULL,
                    ""RowGuid"" UUID NOT NULL,
                    ""ColumnBit"" SMALLINT NULL,
                    ""ColumnDateTime"" TIMESTAMP NULL,
                    ""ColumnDateTime2"" TIMESTAMP(6) NULL,
                    ""ColumnDecimal"" DECIMAL(18,2) NULL,
                    ""ColumnFloat"" DOUBLE PRECISION NULL,
                    ""ColumnInt"" INTEGER NULL,
                    ""ColumnNVarChar"" VARCHAR(2000) NULL,
                    PRIMARY KEY (""Id"")
                );";
            using var connection = new AuroraDbConnection(ConnectionString);
            connection.ExecuteNonQuery(commandText);
        }

        #endregion
    }
}
