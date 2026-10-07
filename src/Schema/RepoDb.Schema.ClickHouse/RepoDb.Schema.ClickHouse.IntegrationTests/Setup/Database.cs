#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;

namespace RepoDb.Schema.ClickHouse.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the name of the source database (the schema is read from here).
        /// </summary>
        public const string SourceName = "repodb_schema_source";

        /// <summary>
        /// Gets the name of the target database (the schema is created here).
        /// </summary>
        public const string TargetName = "repodb_schema_target";

        /// <summary>
        /// Gets the name of the other database of the source (a database is the schema of ClickHouse, so it plays the role of a non-default schema).
        /// </summary>
        public const string SourceSalesName = "repodb_schema_source_sales";

        /// <summary>
        /// Gets the name of the other database of the target (it plays the role of a non-default schema).
        /// </summary>
        public const string TargetSalesName = "repodb_schema_target_sales";

        /// <summary>
        /// Gets the connection string to be used for the default database of ClickHouse.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the source database (the schema is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the target database (the schema is created here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // The connections
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_SCHEMA_CONSTR_MASTER") ??
                GetConnectionString("default");
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_SCHEMA_CONSTR_SOURCE") ??
                GetConnectionString(SourceName);
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_SCHEMA_CONSTR_TARGET") ??
                GetConnectionString(TargetName);

            // Initialize the ClickHouse
            GlobalConfiguration
                .Setup()
                .UseClickHouseSchema();

            // Create the source tables and the target databases
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target databases are dedicated to the tests, so they are dropped and created again
            using (var connection = new ClickHouseConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { TargetName, TargetSalesName })
                {
                    connection.ExecuteNonQuery($"DROP DATABASE IF EXISTS `{name}` SYNC");
                    connection.ExecuteNonQuery($"CREATE DATABASE `{name}`");
                }
            }
        }

        #endregion

        #region Helpers

        private static bool _sourceTablesCreated;

        private static string GetConnectionString(string database) =>
            $"Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database={database};Protocol=http;UseCustomDecimals=false;";

        private static void Execute(IDbConnection connection, params string[] statements)
        {
            foreach (var statement in statements)
            {
                connection.ExecuteNonQuery(statement);
            }
        }

        private static void CreateSourceTables()
        {
            if (_sourceTablesCreated)
            {
                return;
            }

            using (var connection = new ClickHouseConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { SourceName, SourceSalesName })
                {
                    connection.ExecuteNonQuery($"DROP DATABASE IF EXISTS `{name}` SYNC");
                    connection.ExecuteNonQuery($"CREATE DATABASE `{name}`");
                }
            }

            using (var connection = new ClickHouseConnection(GetConnectionString(SourceSalesName)).EnsureOpen())
            {
                CreateSalesTables(connection);
            }

            using (var connection = new ClickHouseConnection(ConnectionStringForSource).EnsureOpen())
            {
                CreateMainTables(connection);
                CreateIndexTables(connection);
                CreateOddNameTables(connection);
            }

            _sourceTablesCreated = true;
        }

        private static void CreateMainTables(IDbConnection connection) =>
            Execute(connection,
                @"CREATE TABLE `Country` (
                    `Id` Int32,
                    `Name` String
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `Person` (
                    `Id` Int64,
                    `Name` String,
                    `NameUpper` String MATERIALIZED upper(`Name`),
                    `Age` Nullable(Int32) DEFAULT 0,
                    `CountryId` Nullable(Int32),
                    `Salary` Nullable(Decimal(18, 2)),
                    `CreatedDateUtc` DateTime64(3) DEFAULT now64(3),
                    CONSTRAINT `CK_Person_Age` CHECK `Age` >= 0,
                    INDEX `IX_Person_Name` (`Name`, `Id`) TYPE minmax GRANULARITY 1
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `OrderLine` (
                    `OrderId` Int32,
                    `LineNumber` Int32,
                    `Quantity` Int32
                ) ENGINE = MergeTree ORDER BY (`OrderId`, `LineNumber`)",
                @"CREATE TABLE `NoKey` (
                    `Value` Nullable(String),
                    `Payload` Nullable(String)
                ) ENGINE = MergeTree ORDER BY tuple()",
                @"CREATE TABLE `Ledger` (
                    `Id` Int32,
                    `InvoiceId` Int32
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `Item` (
                    `Id` Int32
                ) ENGINE = MergeTree ORDER BY `Id`");

        private static void CreateSalesTables(IDbConnection connection) =>
            Execute(connection,
                @"CREATE TABLE `Invoice` (
                    `Id` Int32,
                    `Total` Decimal(19, 4)
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `InvoiceLine` (
                    `Id` Int32,
                    `InvoiceId` Int32
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `Item` (
                    `Id` Int32
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `ItemRef` (
                    `Id` Int32,
                    `ItemId` Int32
                ) ENGINE = MergeTree ORDER BY `Id`");

        private static void CreateIndexTables(IDbConnection connection) =>
            Execute(connection,
                // 2 indexes: a multi-column one and one of a single column
                @"CREATE TABLE `Product` (
                    `Id` Int32,
                    `Code` String,
                    `Sku` String,
                    `Name` String,
                    `Category` Int32,
                    `Price` Decimal(18, 2),
                    `IsActive` Bool DEFAULT true,
                    INDEX `IX_Product_Category_Price` (`Category`, `Price`) TYPE minmax GRANULARITY 1,
                    INDEX `IX_Product_Active_Name` `Name` TYPE minmax GRANULARITY 1
                ) ENGINE = MergeTree ORDER BY `Id`",

                // The types
                @"CREATE TABLE `AllTypes` (
                    `CInt8` Int8,
                    `CInt16` Int16,
                    `CInt32` Int32,
                    `CInt64` Int64,
                    `CUInt8` UInt8,
                    `CUInt16` UInt16,
                    `CUInt32` UInt32,
                    `CUInt64` UInt64,
                    `CFloat32` Float32,
                    `CFloat64` Float64,
                    `CDecimal` Decimal(10, 3),
                    `CString` String,
                    `CFixedString` FixedString(5),
                    `CDate` Date,
                    `CDateTime` DateTime,
                    `CDateTime64` DateTime64(4),
                    `CUuid` UUID,
                    `CBool` Bool,
                    `CNullableString` Nullable(String),
                    `CNullableInt32` Nullable(Int32),
                    `CLowCardinality` LowCardinality(String),
                    `CNullableLowCardinality` LowCardinality(Nullable(String)),
                    `CArray` Array(Int32)
                ) ENGINE = MergeTree ORDER BY tuple()");

        private static void CreateOddNameTables(IDbConnection connection) =>
            Execute(connection,
                // Tables whose names contain a dot, a space and a closing bracket
                @"CREATE TABLE `Odd.Name` (
                    `Id` Int32,
                    `Value` Nullable(String),
                    INDEX `IX_OddName_Value` `Value` TYPE minmax GRANULARITY 1
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `Odd.Child` (
                    `Id` Int32,
                    `ParentId` Int32
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `Order Details` (
                    `Id` Int32,
                    `Unit Price` Decimal(10, 2)
                ) ENGINE = MergeTree ORDER BY `Id`",
                @"CREATE TABLE `Weird]Name` (
                    `Id` Int32
                ) ENGINE = MergeTree ORDER BY `Id`");

        #endregion
    }
}
