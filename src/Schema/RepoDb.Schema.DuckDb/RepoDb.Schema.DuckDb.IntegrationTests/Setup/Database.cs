#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DuckDB.NET.Data;

namespace RepoDb.Schema.DuckDb.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the name of the other schema of the source (it plays the role of a non-default schema).
        /// </summary>
        public const string SourceSalesName = "sales";

        /// <summary>
        /// Gets the name of the other schema of the target (it plays the role of a non-default schema).
        /// </summary>
        public const string TargetSalesName = "sales";

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
            // The connections (the source and the target are 2 database files of the temporary folder)
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_DUCKDB_SCHEMA_CONSTR_SOURCE") ??
                $"Data Source={GetFilePath("source")}";
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_DUCKDB_SCHEMA_CONSTR_TARGET") ??
                $"Data Source={GetFilePath("target")}";

            // Initialize the DuckDb
            GlobalConfiguration
                .Setup()
                .UseDuckDbSchema();

            // Create the source tables and clean the target database
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target database is dedicated to the tests, so all of its tables and sequences are dropped
            using (var connection = new DuckDBConnection(ConnectionStringForTarget).EnsureOpen())
            {
                connection.ExecuteNonQuery($"CREATE SCHEMA IF NOT EXISTS \"{TargetSalesName}\"");
                DropObjects(connection);
            }
        }

        #endregion

        #region Helpers

        private static bool _sourceTablesCreated;

        private static string GetFilePath(string name) =>
            Path.Combine(Path.GetTempPath(), $"repodb_schema_{name}_{Environment.ProcessId}.duckdb");

        private static void DropObjects(IDbConnection connection)
        {
            var tables = new List<(string Schema, string Name)>();
            using (var reader = connection.ExecuteReader("SELECT schema_name, table_name FROM duckdb_tables() WHERE database_name = current_database() AND NOT internal"))
            {
                while (reader.Read())
                {
                    tables.Add((reader.GetString(0), reader.GetString(1)));
                }
            }
            // DuckDB can not drop a table that another table references, so the tables are dropped in as many passes as needed
            while (tables.Count > 0)
            {
                var remaining = new List<(string Schema, string Name)>();
                foreach (var (schema, name) in tables)
                {
                    try
                    {
                        connection.ExecuteNonQuery($"DROP TABLE IF EXISTS \"{schema}\".\"{name}\"");
                    }
                    catch (DuckDBException)
                    {
                        remaining.Add((schema, name));
                    }
                }
                if (remaining.Count == tables.Count)
                {
                    throw new InvalidOperationException("The tables can not be dropped.");
                }
                tables = remaining;
            }

            var sequences = new List<(string Schema, string Name)>();
            using (var reader = connection.ExecuteReader("SELECT schema_name, sequence_name FROM duckdb_sequences() WHERE database_name = current_database() AND NOT temporary"))
            {
                while (reader.Read())
                {
                    sequences.Add((reader.GetString(0), reader.GetString(1)));
                }
            }
            foreach (var (schema, name) in sequences)
            {
                connection.ExecuteNonQuery($"DROP SEQUENCE IF EXISTS \"{schema}\".\"{name}\"");
            }
        }

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

            // The databases are created again
            foreach (var file in new[] { GetFilePath("source"), GetFilePath("target") })
            {
                foreach (var path in new[] { file, file + ".wal" })
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }

            using (var connection = new DuckDBConnection(ConnectionStringForSource).EnsureOpen())
            {
                connection.ExecuteNonQuery($"CREATE SCHEMA IF NOT EXISTS \"{SourceSalesName}\"");
                connection.ExecuteNonQuery($"SET schema = '{SourceSalesName}'");
                CreateSalesTables(connection);
                connection.ExecuteNonQuery("SET schema = 'main'");
                CreateMainTables(connection);
                CreateRelationshipTables(connection);
                CreateIndexTables(connection);
                CreateOddNameTables(connection);
            }

            _sourceTablesCreated = true;
        }

        private static void CreateMainTables(IDbConnection connection) =>
            Execute(connection,
                @"CREATE SEQUENCE ""Country_Id_seq""",
                @"CREATE TABLE ""Country"" (
                    ""Id"" INTEGER DEFAULT nextval('""Country_Id_seq""') NOT NULL,
                    ""Name"" VARCHAR(100) NOT NULL,
                    CONSTRAINT ""PK_Country"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                )",
                @"CREATE SEQUENCE ""Person_Id_seq"" START 10 INCREMENT 5",
                @"CREATE TABLE ""Person"" (
                    ""Id"" BIGINT DEFAULT nextval('""Person_Id_seq""') NOT NULL,
                    ""Name"" VARCHAR(128) NOT NULL,
                    ""NameUpper"" VARCHAR GENERATED ALWAYS AS (upper(""Name"")),
                    ""Age"" INTEGER DEFAULT 0,
                    ""CountryId"" INTEGER,
                    ""Salary"" DECIMAL(18, 2),
                    ""CreatedDateUtc"" TIMESTAMP DEFAULT current_timestamp NOT NULL,
                    CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                    CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                )",
                @"CREATE INDEX ""IX_Person_Name"" ON ""Person"" (""Name"", ""Age"")",
                @"CREATE TABLE ""OrderLine"" (
                    ""OrderId"" INTEGER NOT NULL,
                    ""LineNumber"" INTEGER NOT NULL,
                    ""Quantity"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_OrderLine"" PRIMARY KEY (""OrderId"", ""LineNumber"")
                )",
                @"CREATE TABLE ""NoKey"" (
                    ""Value"" VARCHAR,
                    ""Payload"" BLOB
                )",
                @"CREATE TABLE ""Ledger"" (
                    ""Id"" INTEGER NOT NULL,
                    ""InvoiceId"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_Ledger"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""Item"" (
                    ""Id"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_Item"" PRIMARY KEY (""Id"")
                )");

        private static void CreateSalesTables(IDbConnection connection) =>
            Execute(connection,
                @"CREATE SEQUENCE ""sales"".""Invoice_Id_seq""",
                @"CREATE TABLE ""Invoice"" (
                    ""Id"" INTEGER DEFAULT nextval('""sales"".""Invoice_Id_seq""') NOT NULL,
                    ""Total"" DECIMAL(19, 4) NOT NULL,
                    CONSTRAINT ""PK_Invoice"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""InvoiceLine"" (
                    ""Id"" INTEGER NOT NULL,
                    ""InvoiceId"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_InvoiceLine"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_InvoiceLine_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Invoice"" (""Id"")
                )",
                @"CREATE TABLE ""Item"" (
                    ""Id"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_SalesItem"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""ItemRef"" (
                    ""Id"" INTEGER NOT NULL,
                    ""ItemId"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_ItemRef"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_ItemRef_Item"" FOREIGN KEY (""ItemId"") REFERENCES ""Item"" (""Id"")
                )");

        private static void CreateRelationshipTables(IDbConnection connection) =>
            Execute(connection,
                // A dependency chain
                @"CREATE TABLE ""Parent"" (""Id"" INTEGER NOT NULL, CONSTRAINT ""PK_Parent"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""Child"" (""Id"" INTEGER NOT NULL, ""ParentId"" INTEGER NOT NULL, CONSTRAINT ""PK_Child"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Child_Parent"" FOREIGN KEY (""ParentId"") REFERENCES ""Parent"" (""Id""))",
                @"CREATE TABLE ""GrandChild"" (""Id"" INTEGER NOT NULL, ""ChildId"" INTEGER NOT NULL, CONSTRAINT ""PK_GrandChild"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_GrandChild_Child"" FOREIGN KEY (""ChildId"") REFERENCES ""Child"" (""Id""))",

                // A composite foreign key and a second foreign key (with a cascading rule) in the same table
                @"CREATE TABLE ""Shipment"" (
                    ""Id"" INTEGER NOT NULL,
                    ""OrderId"" INTEGER NOT NULL,
                    ""LineNumber"" INTEGER NOT NULL,
                    ""CountryId"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_Shipment"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Shipment_OrderLine"" FOREIGN KEY (""OrderId"", ""LineNumber"") REFERENCES ""OrderLine"" (""OrderId"", ""LineNumber""),
                    CONSTRAINT ""FK_Shipment_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                )",

                // A self-referencing foreign key
                @"CREATE TABLE ""Employee"" (""Id"" INTEGER NOT NULL, ""ManagerId"" INTEGER, CONSTRAINT ""PK_Employee"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Employee_Manager"" FOREIGN KEY (""ManagerId"") REFERENCES ""Employee"" (""Id""))",

                // A foreign key with the default (no action) delete rule
                @"CREATE TABLE ""Preference"" (
                    ""Id"" INTEGER NOT NULL,
                    ""CountryId"" INTEGER DEFAULT 1 NOT NULL,
                    CONSTRAINT ""PK_Preference"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Preference_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                )",

                // A table with 2 parents that share the same parent: A <- B, A <- C, B <- D, C <- D
                @"CREATE TABLE ""DiamondA"" (""Id"" INTEGER NOT NULL, CONSTRAINT ""PK_DiamondA"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""DiamondB"" (""Id"" INTEGER NOT NULL, ""AId"" INTEGER NOT NULL, CONSTRAINT ""PK_DiamondB"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_DiamondB_DiamondA"" FOREIGN KEY (""AId"") REFERENCES ""DiamondA"" (""Id""))",
                @"CREATE TABLE ""DiamondC"" (""Id"" INTEGER NOT NULL, ""AId"" INTEGER NOT NULL, CONSTRAINT ""PK_DiamondC"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_DiamondC_DiamondA"" FOREIGN KEY (""AId"") REFERENCES ""DiamondA"" (""Id""))",
                @"CREATE TABLE ""DiamondD"" (""Id"" INTEGER NOT NULL, ""BId"" INTEGER NOT NULL, ""CId"" INTEGER NOT NULL, CONSTRAINT ""PK_DiamondD"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_DiamondD_DiamondB"" FOREIGN KEY (""BId"") REFERENCES ""DiamondB"" (""Id""),
                    CONSTRAINT ""FK_DiamondD_DiamondC"" FOREIGN KEY (""CId"") REFERENCES ""DiamondC"" (""Id""))",

                // A table with 2 foreign keys to the same parent table
                @"CREATE TABLE ""Account"" (""Id"" INTEGER NOT NULL, CONSTRAINT ""PK_Account"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""Transfer"" (""Id"" INTEGER NOT NULL, ""FromAccountId"" INTEGER NOT NULL, ""ToAccountId"" INTEGER NOT NULL, CONSTRAINT ""PK_Transfer"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Transfer_FromAccount"" FOREIGN KEY (""FromAccountId"") REFERENCES ""Account"" (""Id""),
                    CONSTRAINT ""FK_Transfer_ToAccount"" FOREIGN KEY (""ToAccountId"") REFERENCES ""Account"" (""Id""))",

                // A table with many children
                @"CREATE TABLE ""FanRoot"" (""Id"" INTEGER NOT NULL, CONSTRAINT ""PK_FanRoot"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""FanChild1"" (""Id"" INTEGER NOT NULL, ""RootId"" INTEGER NOT NULL, CONSTRAINT ""PK_FanChild1"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_FanChild1_FanRoot"" FOREIGN KEY (""RootId"") REFERENCES ""FanRoot"" (""Id""))",
                @"CREATE TABLE ""FanChild2"" (""Id"" INTEGER NOT NULL, ""RootId"" INTEGER NOT NULL, CONSTRAINT ""PK_FanChild2"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_FanChild2_FanRoot"" FOREIGN KEY (""RootId"") REFERENCES ""FanRoot"" (""Id""))",
                @"CREATE TABLE ""FanChild3"" (""Id"" INTEGER NOT NULL, ""RootId"" INTEGER NOT NULL, CONSTRAINT ""PK_FanChild3"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_FanChild3_FanRoot"" FOREIGN KEY (""RootId"") REFERENCES ""FanRoot"" (""Id""))",
                @"CREATE TABLE ""FanChild4"" (""Id"" INTEGER NOT NULL, ""RootId"" INTEGER NOT NULL, CONSTRAINT ""PK_FanChild4"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_FanChild4_FanRoot"" FOREIGN KEY (""RootId"") REFERENCES ""FanRoot"" (""Id""))");

        private static void CreateIndexTables(IDbConnection connection) =>
            Execute(connection,
                // 2 unique indexes, a multi-column index with a descending key and an index of one column
                @"CREATE TABLE ""Product"" (
                    ""Id"" INTEGER NOT NULL,
                    ""Code"" VARCHAR(20) NOT NULL,
                    ""Sku"" VARCHAR(30) NOT NULL,
                    ""Name"" VARCHAR(100) NOT NULL,
                    ""Category"" INTEGER NOT NULL,
                    ""Price"" DECIMAL(18, 2) NOT NULL,
                    ""IsActive"" SMALLINT DEFAULT 1 NOT NULL,
                    CONSTRAINT ""PK_Product"" PRIMARY KEY (""Id"")
                )",
                @"CREATE UNIQUE INDEX ""CIX_Product_Code"" ON ""Product"" (""Code"")",
                @"CREATE UNIQUE INDEX ""UX_Product_Sku"" ON ""Product"" (""Sku"")",
                @"CREATE INDEX ""IX_Product_Category_Price"" ON ""Product"" (""Category"", ""Price"")",
                @"CREATE INDEX ""IX_Product_Active_Name"" ON ""Product"" (""Name"")",

                // The types
                @"CREATE TABLE ""AllTypes"" (
                    ""CTinyInt"" TINYINT,
                    ""CSmallInt"" SMALLINT,
                    ""CInteger"" INTEGER NOT NULL,
                    ""CBigInt"" BIGINT,
                    ""CHugeInt"" HUGEINT,
                    ""CUTinyInt"" UTINYINT,
                    ""CUSmallInt"" USMALLINT,
                    ""CUInteger"" UINTEGER,
                    ""CUBigInt"" UBIGINT,
                    ""CFloat"" FLOAT,
                    ""CDouble"" DOUBLE,
                    ""CDecimal"" DECIMAL(10, 3),
                    ""CVarChar"" VARCHAR,
                    ""CBlob"" BLOB,
                    ""CDate"" DATE,
                    ""CTime"" TIME,
                    ""CTimestamp"" TIMESTAMP,
                    ""CTimestampTz"" TIMESTAMPTZ,
                    ""CInterval"" INTERVAL,
                    ""CUuid"" UUID,
                    ""CBoolean"" BOOLEAN,
                    ""CJson"" JSON,
                    ""CList"" INTEGER[],
                    ""CEnum"" ENUM('a', 'b')
                )");

        private static void CreateOddNameTables(IDbConnection connection) =>
            Execute(connection,
                // Tables whose names contain a dot, a space and a double quote
                @"CREATE TABLE ""Odd.Name"" (
                    ""Id"" INTEGER NOT NULL,
                    ""Value"" VARCHAR(50),
                    CONSTRAINT ""PK_OddName"" PRIMARY KEY (""Id"")
                )",
                @"CREATE INDEX ""IX_OddName_Value"" ON ""Odd.Name"" (""Value"")",
                @"CREATE TABLE ""Odd.Child"" (
                    ""Id"" INTEGER NOT NULL,
                    ""ParentId"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_OddChild"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_OddChild_OddName"" FOREIGN KEY (""ParentId"") REFERENCES ""Odd.Name"" (""Id"")
                )",
                @"CREATE TABLE ""Order Details"" (
                    ""Id"" INTEGER NOT NULL,
                    ""Unit Price"" DECIMAL(10, 2) NOT NULL,
                    CONSTRAINT ""PK_OrderDetails"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""Weird]Name"" (
                    ""Id"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_WeirdName"" PRIMARY KEY (""Id"")
                )");

        #endregion
    }
}
