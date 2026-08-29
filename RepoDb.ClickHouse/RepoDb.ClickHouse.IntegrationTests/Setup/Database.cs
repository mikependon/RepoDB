#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using ClickHouse.Driver.ADO;
using RepoDb.Exceptions;
using RepoDb.ClickHouse.IntegrationTests.Models;

namespace RepoDb.ClickHouse.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used for the system/default database.
        /// </summary>
        public static string ConnectionStringForSystem { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Set the connection string
            ConnectionStringForSystem =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_CONSTR_SYSTEM") ??
                @"Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=default;Protocol=http;UseCustomDecimals=false;";

            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_CONSTR") ??
                @"Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=RepoDb;Protocol=http;UseCustomDecimals=false;";

            // Initialize ClickHouse
            GlobalConfiguration
                .Setup()
                .UseClickHouse();

            // Create databases
            CreateDatabase();

            // Create tables
            CreateTables();
        }

        public static void Cleanup()
        {
            using (var connection = new ClickHouseConnection(ConnectionString))
            {
                try
                {
                    connection.Truncate<CompleteTable>();
                    connection.Truncate<NonIdentityCompleteTable>();
                }
                catch (MissingFieldsException)
                {
                    // Table does not exist
                }
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new ClickHouseConnection(ConnectionString))
            {
                var tables = Helper.CreateCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion

        #region NonIdentityCompleteTable

        public static IEnumerable<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
        {
            using (var connection = new ClickHouseConnection(ConnectionString))
            {
                var tables = Helper.CreateNonIdentityCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion

        #region CreateDatabases

        private static void CreateDatabase()
        {
            using (var connection = new ClickHouseConnection(ConnectionStringForSystem))
            {
                connection.ExecuteNonQuery(@"CREATE DATABASE IF NOT EXISTS RepoDb;");
            }
        }

        #endregion

        #region CreateTables

        private static void CreateTables()
        {
            CreateCompleteTable();
            CreateNonIdentityCompleteTable();
        }

        private static void CreateCompleteTable()
        {
            using (var connection = new ClickHouseConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS CompleteTable
                    (
                        Id Int64,
                        ColumnVarchar Nullable(String),
                        ColumnInt Nullable(Int32),
                        ColumnDecimal2 Nullable(Decimal(18,2)),
                        ColumnDateTime Nullable(DateTime),
                        ColumnBlob Nullable(String),
                        ColumnBlobAsArray Nullable(String),
                        ColumnBinary Nullable(String),
                        ColumnLongBlob Nullable(String),
                        ColumnMediumBlob Nullable(String),
                        ColumnTinyBlob Nullable(String),
                        ColumnVarBinary Nullable(String),
                        ColumnDate Nullable(Date),
                        ColumnDateTime2 Nullable(DateTime64(3)),
                        ColumnTime Nullable(String),
                        ColumnTimeStamp Nullable(DateTime64(3)),
                        ColumnYear Nullable(Int16),
                        ColumnGeometry Nullable(String),
                        ColumnLineString Nullable(String),
                        ColumnMultiLineString Nullable(String),
                        ColumnMultiPoint Nullable(String),
                        ColumnMultiPolygon Nullable(String),
                        ColumnPoint Nullable(String),
                        ColumnPolygon Nullable(String),
                        ColumnBigint Nullable(Int64),
                        ColumnDecimal Nullable(Decimal(10,0)),
                        ColumnDouble Nullable(Float64),
                        ColumnFloat Nullable(Float32),
                        ColumnInt2 Nullable(Int32),
                        ColumnMediumInt Nullable(Int32),
                        ColumnReal Nullable(Float64),
                        ColumnSmallInt Nullable(Int16),
                        ColumnTinyInt Nullable(Int8),
                        ColumnChar Nullable(String),
                        ColumnJson Nullable(Json),
                        ColumnJsonString Nullable(String),
                        ColumnNChar Nullable(String),
                        ColumnNVarChar Nullable(String),
                        ColumnLongText Nullable(String),
                        ColumnMediumText Nullable(String),
                        ColumnText Nullable(String),
                        ColumnTinyText Nullable(String),
                        ColumnBit Nullable(UInt64)
                    )
                    ENGINE = ReplacingMergeTree
                    ORDER BY Id;");
            }
        }

        private static void CreateNonIdentityCompleteTable()
        {
            using (var connection = new ClickHouseConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS NonIdentityCompleteTable
                    (
                        Id Int64,
                        ColumnVarchar Nullable(String),
                        ColumnInt Nullable(Int32),
                        ColumnDecimal2 Nullable(Decimal(18,2)),
                        ColumnDateTime Nullable(DateTime),
                        ColumnBlob Nullable(String),
                        ColumnBlobAsArray Nullable(String),
                        ColumnBinary Nullable(String),
                        ColumnLongBlob Nullable(String),
                        ColumnMediumBlob Nullable(String),
                        ColumnTinyBlob Nullable(String),
                        ColumnVarBinary Nullable(String),
                        ColumnDate Nullable(Date),
                        ColumnDateTime2 Nullable(DateTime64(3)),
                        ColumnTime Nullable(String),
                        ColumnTimeStamp Nullable(DateTime64(3)),
                        ColumnYear Nullable(Int16),
                        ColumnGeometry Nullable(String),
                        ColumnLineString Nullable(String),
                        ColumnMultiLineString Nullable(String),
                        ColumnMultiPoint Nullable(String),
                        ColumnMultiPolygon Nullable(String),
                        ColumnPoint Nullable(String),
                        ColumnPolygon Nullable(String),
                        ColumnBigint Nullable(Int64),
                        ColumnDecimal Nullable(Decimal(10,0)),
                        ColumnDouble Nullable(Float64),
                        ColumnFloat Nullable(Float32),
                        ColumnInt2 Nullable(Int32),
                        ColumnMediumInt Nullable(Int32),
                        ColumnReal Nullable(Float64),
                        ColumnSmallInt Nullable(Int16),
                        ColumnTinyInt Nullable(Int8),
                        ColumnChar Nullable(String),
                        ColumnJson Nullable(Json),
                        ColumnJsonString Nullable(String),
                        ColumnNChar Nullable(String),
                        ColumnNVarChar Nullable(String),
                        ColumnLongText Nullable(String),
                        ColumnMediumText Nullable(String),
                        ColumnText Nullable(String),
                        ColumnTinyText Nullable(String),
                        ColumnBit Nullable(UInt64)
                    )
                    ENGINE = ReplacingMergeTree
                    ORDER BY Id;");
            }
        }

        #endregion
    }
}
