#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using ClickHouse.Driver.ADO;
using RepoDb.DbSettings;
using RepoDb.Schema.ClickHouse;

namespace RepoDb.Data.ClickHouse.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the name of the source database (the data is read from here).
        /// </summary>
        public const string SourceName = "repodb_data_source";

        /// <summary>
        /// Gets the name of the other database of the source (it plays the role of the <c>Sales</c> schema).
        /// </summary>
        public const string SourceSalesName = "repodb_data_source_sales";

        /// <summary>
        /// Gets the name of the target database (the data is copied here).
        /// </summary>
        public const string TargetName = "repodb_data_target";

        /// <summary>
        /// Gets the name of the other database of the target (it plays the role of the <c>Sales</c> schema).
        /// </summary>
        public const string TargetSalesName = "repodb_data_target_sales";

        /// <summary>
        /// Gets the connection string to be used for the ClickHouse default database.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the source database (the data is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the target database (the data is copied here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // The connections
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_DATA_CONSTR_MASTER") ??
                GetConnectionString("default");
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_DATA_CONSTR_SOURCE") ??
                GetConnectionString(SourceName);
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_CLICKHOUSE_DATA_CONSTR_TARGET") ??
                GetConnectionString(TargetName);

            // Initialize the ClickHouse
            GlobalConfiguration
                .Setup()
                .UseClickHouse(new ClickHouseBulkDbSetting())
                .UseClickHouseSchema();

            // Create the source tables (with their rows) and the target databases
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

            // The fields of the tables are cached (by the connection string and the name of the table), and the tables are not the same in the next test
            DbFieldCache.Flush();
        }

        #endregion

        #region Helpers

        private static string GetConnectionString(string database) =>
            $"Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database={database};Protocol=http;UseCustomDecimals=false;";

        private static void Execute(IDbConnection connection, params string[] statements)
        {
            foreach (var statement in statements)
            {
                connection.ExecuteNonQuery(statement);
            }
        }

        #endregion

        #region CreateTables

        private static void CreateSourceTables()
        {
            using (var master = new ClickHouseConnection(ConnectionStringForMaster).EnsureOpen())
            {
                // The source tables are created (with their rows) only once: they are not changed by the tests
                var exists = Convert.ToInt32(master.ExecuteScalar($"SELECT count() FROM system.tables WHERE database = '{SourceName}' AND name = 'Empty'"));
                if (exists > 0)
                {
                    return;
                }
                foreach (var name in new[] { SourceName, SourceSalesName })
                {
                    master.ExecuteNonQuery($"DROP DATABASE IF EXISTS `{name}` SYNC");
                    master.ExecuteNonQuery($"CREATE DATABASE `{name}`");
                }
            }

            // The tables of the Sales schema (another database)
            using (var connection = new ClickHouseConnection(GetConnectionString(SourceSalesName)).EnsureOpen())
            {
                Execute(connection,
                    @"CREATE TABLE `Invoice` (`Id` Int32, `Total` Decimal(19, 4)) ENGINE = MergeTree ORDER BY `Id`",
                    @"CREATE TABLE `InvoiceLine` (`Id` Int32, `InvoiceId` Int32, `Description` String) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Invoice` (`Id`, `Total`) VALUES (1, 100.0000), (2, 250.5000)",
                    @"INSERT INTO `InvoiceLine` (`Id`, `InvoiceId`, `Description`) VALUES (1, 1, 'Line 1'), (2, 1, 'Line 2'), (3, 2, 'Line 3')");
            }

            using (var connection = new ClickHouseConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The tree: Country <- Person <- Pet <- Toy (ClickHouse has no foreign key: the tables are related by the values only)
                Execute(connection,
                    @"CREATE TABLE `Country` (`Id` Int32, `Name` String) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Country` (`Id`, `Name`) VALUES (1, 'Philippines'), (2, 'Japan'), (3, 'Germany')",
                    @"CREATE TABLE `Person`
                    (
                        `Id` Int32,
                        `Name` String,
                        `Age` Nullable(Int32),
                        `CountryId` Nullable(Int32),
                        `Salary` Nullable(Decimal(18, 2)),
                        `Birthday` Nullable(Date32),
                        `IsActive` Bool,
                        `CreatedDateUtc` DateTime64(3),
                        INDEX `IX_Person_Name` (`Name`) TYPE minmax GRANULARITY 1
                    ) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Person` (`Id`, `Name`, `Age`, `CountryId`, `Salary`, `Birthday`, `IsActive`, `CreatedDateUtc`) VALUES
                        (1, 'Alice', 30, 1, 1000.50, '1990-01-15', 1, '2026-01-01 10:00:00.123'),
                        (2, 'Bob', 45, 2, 2500.00, '1975-06-30', 1, '2026-01-02 11:00:00.456'),
                        (3, 'Carol', NULL, NULL, NULL, NULL, 0, '2026-01-03 12:00:00.789'),
                        (4, 'Dave', 22, 1, 800.25, '2003-12-01', 1, '2026-01-04 13:00:00.000'),
                        (5, 'Eve', 37, 3, 3100.75, '1988-03-09', 0, '2026-01-05 14:00:00.321'),
                        (6, 'Frank', 52, 2, NULL, '1970-11-22', 1, '2026-01-06 15:00:00.654')",
                    @"CREATE TABLE `Pet` (`Id` Int32, `Name` String, `PersonId` Int32) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Pet` (`Id`, `Name`, `PersonId`) VALUES (1, 'Rex', 1), (2, 'Tom', 1), (3, 'Kiko', 2), (4, 'Luna', 4), (5, 'Milo', 5)",
                    @"CREATE TABLE `Toy` (`Id` Int32, `Name` String, `PetId` Int32) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Toy` (`Id`, `Name`, `PetId`) VALUES (1, 'Ball', 1), (2, 'Bone', 1), (3, 'Mouse', 2), (4, 'Yarn', 3), (5, 'Stick', 4)");

                // A table that is not related to any other table
                Execute(connection,
                    @"CREATE TABLE `Currency` (`Code` String, `Name` String) ENGINE = MergeTree ORDER BY `Code`",
                    @"INSERT INTO `Currency` (`Code`, `Name`) VALUES ('PHP', 'Philippine Peso'), ('JPY', 'Japanese Yen'), ('EUR', 'Euro')");

                // One column of each of the common data types, and a row of NULL values
                Execute(connection,
                    @"CREATE TABLE `Sample`
                    (
                        `Id` Int32,
                        `BigIntValue` Nullable(Int64),
                        `BitValue` Nullable(Bool),
                        `DecimalValue` Nullable(Decimal(18, 4)),
                        `FloatValue` Nullable(Float64),
                        `DateTime2Value` Nullable(DateTime64(6)),
                        `DateValue` Nullable(Date32),
                        `GuidValue` Nullable(String),
                        `TextValue` Nullable(String),
                        `CharValue` Nullable(String)
                    ) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Sample` VALUES
                        (1, 9223372036854775807, 1, 12345.6789, 3.14159265358979, '2026-02-03 04:05:06.123456', '2026-02-03', '6f9619ff-8b86-d011-b42d-00c04fc964ff', 'A long text with unicode: ñ é 日本語', 'abc'),
                        (2, -9223372036854775807, 0, -0.0001, -2.5E-10, '1900-01-01 00:00:00', '2299-12-31', '00000000-0000-0000-0000-000000000000', 'text', ''),
                        (3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)");

                // A table whose name and column have a space
                Execute(connection,
                    @"CREATE TABLE `Order Details` (`Id` Int32, `Unit Price` Decimal(10, 2)) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Order Details` (`Id`, `Unit Price`) VALUES (1, 9.99), (2, 19.50)");

                // A table with many rows, to copy the rows in many batches
                Execute(connection,
                    @"CREATE TABLE `Number` (`Value` Int32, `Label` String) ENGINE = MergeTree ORDER BY `Value`",
                    @"INSERT INTO `Number` (`Value`, `Label`) SELECT toInt32(number + 1), concat('Number ', toString(number + 1)) FROM numbers(2500)");

                // A self-referencing table
                Execute(connection,
                    @"CREATE TABLE `Employee` (`Id` Int32, `Name` String, `ManagerId` Nullable(Int32)) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Employee` (`Id`, `Name`, `ManagerId`) VALUES (1, 'CEO', NULL), (2, 'VP', 1), (3, 'Developer', 2)");

                // 2 tables that reference each other
                Execute(connection,
                    @"CREATE TABLE `CycleA` (`Id` Int32, `BId` Nullable(Int32)) ENGINE = MergeTree ORDER BY `Id`",
                    @"CREATE TABLE `CycleB` (`Id` Int32, `AId` Nullable(Int32)) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `CycleA` (`Id`, `BId`) VALUES (1, 1)",
                    @"INSERT INTO `CycleB` (`Id`, `AId`) VALUES (1, 1)");

                // A table without a primary key
                Execute(connection,
                    @"CREATE TABLE `NoKey` (`Value` Nullable(String), `Amount` Nullable(Int32)) ENGINE = MergeTree ORDER BY tuple()",
                    @"INSERT INTO `NoKey` (`Value`, `Amount`) VALUES ('A', 1), ('A', 1), (NULL, NULL)");

                // A table of the default database that refers to a table of the Sales database
                Execute(connection,
                    @"CREATE TABLE `Ledger` (`Id` Int32, `InvoiceId` Int32) ENGINE = MergeTree ORDER BY `Id`",
                    @"INSERT INTO `Ledger` (`Id`, `InvoiceId`) VALUES (1, 1), (2, 2)");

                // A table without rows (it is the last one: it tells that the source tables are all created)
                Execute(connection,
                    @"CREATE TABLE `Empty` (`Id` Int32, `Name` Nullable(String)) ENGINE = MergeTree ORDER BY `Id`");
            }
        }

        #endregion
    }
}
