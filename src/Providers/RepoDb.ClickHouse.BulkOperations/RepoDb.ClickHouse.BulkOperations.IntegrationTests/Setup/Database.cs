#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.ClickHouse.BulkOperations.IntegrationTests.Models;
using System;
using ClickHouse.Driver.ADO;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// A class used as a startup setup for the RepoDb ClickHouse bulk-operations test database.
    /// </summary>
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the connection string used to reach the ClickHouse server's default database - used only to
        /// create the <c>RepoDb</c> database if it does not already exist.
        /// </summary>
        public static string ConnectionStringForSystem { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the ClickHouse database.
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
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_CONSTR_SYSTEM") ??
                "Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=default;Protocol=http;UseCustomDecimals=false;";

            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_CONSTR") ??
                "Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=RepoDb;Protocol=http;UseCustomDecimals=false;";

            // Initialize ClickHouse
            GlobalConfiguration
                .Setup()
                .UseClickHouse(true);

            // Create the database first
            CreateDatabase();

            // Create the tables
            CreateTables();
        }

        /// <summary>
        /// Creates the <c>RepoDb</c> database if it does not already exist.
        /// </summary>
        public static void CreateDatabase()
        {
            using var connection = new ClickHouseConnection(ConnectionStringForSystem);
            connection.ExecuteNonQuery("CREATE DATABASE IF NOT EXISTS RepoDb;");
        }

        /// <summary>
        /// Clean up all the table.
        /// </summary>
        public static void Cleanup()
        {
            using var connection = new ClickHouseConnection(ConnectionString);
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
        /// Creates a table with some important fields, mirroring the shape of the SqlServer/MySQL suites'
        /// identity table. ClickHouse has no identity/auto-increment mechanism (see the remarks on
        /// <c>ClickHouseBulkImportIdentityBehavior.ReturnIdentity</c>), so <c>Id</c> is a plain, caller-supplied
        /// <c>Int64</c> used as the table's sort/primary key (<c>ORDER BY</c>) rather than a generated value.
        /// <c>RowGuid</c> is ClickHouse's native <c>UUID</c> type, which binds directly to/from
        /// <see cref="Guid"/> - no property handler required. <c>ReplacingMergeTree</c> is used (rather than
        /// plain <c>MergeTree</c>) so that repeated test runs which re-insert the same <c>Id</c> collapse to the
        /// latest row instead of accumulating duplicates.
        /// </summary>
        public static void CreateBulkOperationIdentityTable()
        {
            var commandText = @"
                CREATE TABLE IF NOT EXISTS BulkOperationIdentityTable
                (
                    Id Int64,
                    RowGuid UUID,
                    ColumnBit Nullable(UInt8),
                    ColumnDateTime Nullable(DateTime),
                    ColumnDateTime2 Nullable(DateTime64(6)),
                    ColumnDecimal Nullable(Decimal(18,2)),
                    ColumnFloat Nullable(Float64),
                    ColumnInt Nullable(Int32),
                    ColumnNVarChar Nullable(String)
                )
                ENGINE = ReplacingMergeTree
                ORDER BY Id;";
            using var connection = new ClickHouseConnection(ConnectionString);
            connection.ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// Creates a non-identity table that has some important fields. All fields are nullable except the
        /// primary key. <c>Id</c> here is a plain, caller-supplied <c>Int64</c> just like
        /// <see cref="CreateBulkOperationIdentityTable"/> - ClickHouse has no distinct "identity" column concept
        /// at all, so the two tables only differ by name/usage in the tests that consume them, not by schema.
        /// </summary>
        public static void CreateBulkOperationNonIdentityTable()
        {
            var commandText = @"
                CREATE TABLE IF NOT EXISTS BulkOperationNonIdentityTable
                (
                    Id Int64,
                    RowGuid UUID,
                    ColumnBit Nullable(UInt8),
                    ColumnDateTime Nullable(DateTime),
                    ColumnDateTime2 Nullable(DateTime64(6)),
                    ColumnDecimal Nullable(Decimal(18,2)),
                    ColumnFloat Nullable(Float64),
                    ColumnInt Nullable(Int32),
                    ColumnNVarChar Nullable(String)
                )
                ENGINE = ReplacingMergeTree
                ORDER BY Id;";
            using var connection = new ClickHouseConnection(ConnectionString);
            connection.ExecuteNonQuery(commandText);
        }

        #endregion
    }
}
