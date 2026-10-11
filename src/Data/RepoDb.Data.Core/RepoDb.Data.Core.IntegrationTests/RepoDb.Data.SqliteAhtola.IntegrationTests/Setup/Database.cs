#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Ahtola.Data.Sqlite;
using RepoDb.Schema.Sqlite.Ahtola;

namespace RepoDb.Data.SqliteAhtola.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the path of the source database (the data is read from here).
        /// </summary>
        public static string SourcePath { get; private set; }

        /// <summary>
        /// Gets the path of the target database (the data is copied here).
        /// </summary>
        public static string TargetPath { get; private set; }

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
            // The connections (SQLite has no server: the source and the target are 2 files)
            SourcePath = Environment.GetEnvironmentVariable("REPODB_AHTOLA_DATA_SOURCE") ?? Path.Combine(Path.GetTempPath(), "repodb_data_ahtola_source.db");
            TargetPath = Environment.GetEnvironmentVariable("REPODB_AHTOLA_DATA_TARGET") ?? Path.Combine(Path.GetTempPath(), "repodb_data_ahtola_target.db");
            ConnectionStringForSource = $"Data Source={SourcePath};Pooling=False;";
            ConnectionStringForTarget = $"Data Source={TargetPath};Pooling=False;";

            // Initialize the SQLite
            GlobalConfiguration
                .Setup()
                .UseAhtola()
                .UseAhtolaSchema();

            // Create the source tables (with their rows) and clean the target database
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target database is dedicated to the tests, so all of its tables are dropped
            using (var connection = new SqliteConnection(ConnectionStringForTarget).EnsureOpen())
            {
                DropTables(connection);
            }

            // The fields of the tables are cached (by the connection string and the name of the table), and the tables are not the same in the next test
            DbFieldCache.Flush();
        }

        #endregion

        #region Helpers

        private static void DropTables(IDbConnection connection)
        {
            // The foreign keys are not checked while the tables are dropped, so they can be dropped in any order
            connection.ExecuteNonQuery("PRAGMA foreign_keys = OFF");
            var tables = new List<string>();
            using (var reader = connection.ExecuteReader("SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%' AND name NOT LIKE '%turso%internal%'"))
            {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }
            foreach (var table in tables)
            {
                connection.ExecuteNonQuery($"DROP TABLE IF EXISTS \"{table}\"");
            }
            connection.ExecuteNonQuery("PRAGMA foreign_keys = ON");
        }

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
            using (var master = new SqliteConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The source tables are created (with their rows) only once: they are not changed by the tests
                var exists = Convert.ToInt32(master.ExecuteScalar("SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'Empty'"));
                if (exists > 0)
                {
                    return;
                }
                DropTables(master);
            }

            using (var connection = new SqliteConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The foreign keys are not checked while the rows are inserted (the tables are related to each other)
                connection.ExecuteNonQuery("PRAGMA foreign_keys = OFF");

                // The root of the tree: Country <- Person <- Pet <- Toy
                Execute(connection,
                    @"CREATE TABLE ""Country""
                    (
                        ""Id"" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        ""Name"" VARCHAR(100) NOT NULL,
                        CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                    )",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Philippines')",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Japan')",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Germany')",
                    @"CREATE TABLE ""Person""
                    (
                        ""Id"" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        ""Name"" VARCHAR(128) NOT NULL,
                        ""Age"" INTEGER,
                        ""CountryId"" INTEGER,
                        ""Salary"" DECIMAL(18, 2),
                        ""Birthday"" DATE,
                        ""IsActive"" BOOLEAN NOT NULL,
                        ""CreatedDateUtc"" TIMESTAMP NOT NULL,
                        CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                        CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                    )",
                    @"CREATE INDEX ""IX_Person_Name"" ON ""Person"" (""Name"")",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Alice', 30, 1, 1000.50, '1990-01-15', 1, '2026-01-01 10:00:00.123')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Bob', 45, 2, 2500.00, '1975-06-30', 1, '2026-01-02 11:00:00.456')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Carol', NULL, NULL, NULL, NULL, 0, '2026-01-03 12:00:00.789')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Dave', 22, 1, 800.25, '2003-12-01', 1, '2026-01-04 13:00:00.000')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Eve', 37, 3, 3100.75, '1988-03-09', 0, '2026-01-05 14:00:00.321')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Frank', 52, 2, NULL, '1970-11-22', 1, '2026-01-06 15:00:00.654')",
                    @"CREATE TABLE ""Pet""
                    (
                        ""Id"" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        ""Name"" VARCHAR(50) NOT NULL,
                        ""PersonId"" INTEGER NOT NULL,
                        CONSTRAINT ""FK_Pet_Person"" FOREIGN KEY (""PersonId"") REFERENCES ""Person"" (""Id"")
                    )",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Rex', 1)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Tom', 1)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Kiko', 2)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Luna', 4)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Milo', 5)",
                    @"CREATE TABLE ""Toy""
                    (
                        ""Id"" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        ""Name"" VARCHAR(50) NOT NULL,
                        ""PetId"" INTEGER NOT NULL,
                        CONSTRAINT ""FK_Toy_Pet"" FOREIGN KEY (""PetId"") REFERENCES ""Pet"" (""Id"")
                    )",
                    @"INSERT INTO ""Toy"" (""Name"", ""PetId"") VALUES ('Ball', 1)",
                    @"INSERT INTO ""Toy"" (""Name"", ""PetId"") VALUES ('Bone', 1)",
                    @"INSERT INTO ""Toy"" (""Name"", ""PetId"") VALUES ('Mouse', 2)",
                    @"INSERT INTO ""Toy"" (""Name"", ""PetId"") VALUES ('Yarn', 3)",
                    @"INSERT INTO ""Toy"" (""Name"", ""PetId"") VALUES ('Stick', 4)");

                // A table that is not related to any other table
                Execute(connection,
                    @"CREATE TABLE ""Currency""
                    (
                        ""Code"" CHAR(3) NOT NULL,
                        ""Name"" VARCHAR(50) NOT NULL,
                        CONSTRAINT ""PK_Currency"" PRIMARY KEY (""Code"")
                    )",
                    @"INSERT INTO ""Currency"" (""Code"", ""Name"") VALUES ('PHP', 'Philippine Peso')",
                    @"INSERT INTO ""Currency"" (""Code"", ""Name"") VALUES ('JPY', 'Japanese Yen')",
                    @"INSERT INTO ""Currency"" (""Code"", ""Name"") VALUES ('EUR', 'Euro')");

                // One column of each of the common data types, and a row of NULL values
                Execute(connection,
                    @"CREATE TABLE ""Sample""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""BigIntValue"" BIGINT,
                        ""BitValue"" BOOLEAN,
                        ""DecimalValue"" DECIMAL(18, 4),
                        ""FloatValue"" DOUBLE,
                        ""DateTime2Value"" TIMESTAMP,
                        ""DateValue"" DATE,
                        ""GuidValue"" CHAR(36),
                        ""BinaryValue"" BLOB,
                        ""TextValue"" TEXT,
                        ""CharValue"" VARCHAR(50),
                        CONSTRAINT ""PK_Sample"" PRIMARY KEY (""Id"")
                    )",
                    @"INSERT INTO ""Sample"" VALUES (1, 9223372036854775807, 1, 12345.6789, 3.14159265358979, '2026-02-03 04:05:06.123456', '2026-02-03', '6f9619ff-8b86-d011-b42d-00c04fc964ff', x'0102030405', 'A long text with unicode: ñ é 日本語', 'abc')",
                    @"INSERT INTO ""Sample"" VALUES (2, -9223372036854775807, 0, -0.0001, -2.5E-10, '0001-01-01 00:00:00', '9999-12-31', '00000000-0000-0000-0000-000000000000', x'FF', 'text', '')",
                    @"INSERT INTO ""Sample"" VALUES (3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)");

                // A table whose name and column have a space
                Execute(connection,
                    @"CREATE TABLE ""Order Details""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Unit Price"" DECIMAL(10, 2) NOT NULL,
                        CONSTRAINT ""PK_Order Details"" PRIMARY KEY (""Id"")
                    )",
                    @"INSERT INTO ""Order Details"" (""Id"", ""Unit Price"") VALUES (1, 9.99)",
                    @"INSERT INTO ""Order Details"" (""Id"", ""Unit Price"") VALUES (2, 19.50)");

                // A table with many rows, to copy the rows in many batches
                Execute(connection,
                    @"CREATE TABLE ""Number""
                    (
                        ""Value"" INTEGER NOT NULL,
                        ""Label"" VARCHAR(20) NOT NULL,
                        CONSTRAINT ""PK_Number"" PRIMARY KEY (""Value"")
                    )",
                    @"WITH RECURSIVE ""Numbers"" (""N"") AS (SELECT 1 UNION ALL SELECT ""N"" + 1 FROM ""Numbers"" WHERE ""N"" < 2500)
                        INSERT INTO ""Number"" (""Value"", ""Label"") SELECT ""N"", 'Number ' || ""N"" FROM ""Numbers""");

                // A self-referencing foreign key
                Execute(connection,
                    @"CREATE TABLE ""Employee""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Name"" VARCHAR(50) NOT NULL,
                        ""ManagerId"" INTEGER,
                        CONSTRAINT ""PK_Employee"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Employee_Manager"" FOREIGN KEY (""ManagerId"") REFERENCES ""Employee"" (""Id"")
                    )",
                    @"INSERT INTO ""Employee"" (""Id"", ""Name"", ""ManagerId"") VALUES (1, 'CEO', NULL)",
                    @"INSERT INTO ""Employee"" (""Id"", ""Name"", ""ManagerId"") VALUES (2, 'VP', 1)",
                    @"INSERT INTO ""Employee"" (""Id"", ""Name"", ""ManagerId"") VALUES (3, 'Developer', 2)");

                // A cycle of 2 tables
                Execute(connection,
                    @"CREATE TABLE ""CycleA"" (""Id"" INTEGER NOT NULL, ""BId"" INTEGER, CONSTRAINT ""PK_CycleA"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_CycleA_CycleB"" FOREIGN KEY (""BId"") REFERENCES ""CycleB"" (""Id""))",
                    @"CREATE TABLE ""CycleB"" (""Id"" INTEGER NOT NULL, ""AId"" INTEGER, CONSTRAINT ""PK_CycleB"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_CycleB_CycleA"" FOREIGN KEY (""AId"") REFERENCES ""CycleA"" (""Id""))",
                    @"INSERT INTO ""CycleA"" (""Id"", ""BId"") VALUES (1, 1)",
                    @"INSERT INTO ""CycleB"" (""Id"", ""AId"") VALUES (1, 1)");

                // A table without a primary key
                Execute(connection,
                    @"CREATE TABLE ""NoKey"" (""Value"" VARCHAR(50), ""Amount"" INTEGER)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES (NULL, NULL)");

                // The tables that SQLite keeps in the default schema (an invoice, its lines, and a ledger that references it)
                Execute(connection,
                    @"CREATE TABLE ""Invoice""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Total"" DECIMAL(18, 4) NOT NULL,
                        CONSTRAINT ""PK_Invoice"" PRIMARY KEY (""Id"")
                    )",
                    @"CREATE TABLE ""InvoiceLine""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""InvoiceId"" INTEGER NOT NULL,
                        ""Description"" VARCHAR(100) NOT NULL,
                        CONSTRAINT ""PK_InvoiceLine"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_InvoiceLine_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Invoice"" (""Id"")
                    )",
                    @"CREATE TABLE ""Ledger""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""InvoiceId"" INTEGER NOT NULL,
                        CONSTRAINT ""PK_Ledger"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Ledger_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Invoice"" (""Id"")
                    )",
                    @"INSERT INTO ""Invoice"" (""Id"", ""Total"") VALUES (1, 100.0000)",
                    @"INSERT INTO ""Invoice"" (""Id"", ""Total"") VALUES (2, 250.5000)",
                    @"INSERT INTO ""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (1, 1, 'Line 1')",
                    @"INSERT INTO ""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (2, 1, 'Line 2')",
                    @"INSERT INTO ""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (3, 2, 'Line 3')",
                    @"INSERT INTO ""Ledger"" (""Id"", ""InvoiceId"") VALUES (1, 1)",
                    @"INSERT INTO ""Ledger"" (""Id"", ""InvoiceId"") VALUES (2, 2)");

                // A table without rows (it is the last one: it tells that the source tables are all created)
                Execute(connection,
                    @"CREATE TABLE ""Empty""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Name"" VARCHAR(50),
                        CONSTRAINT ""PK_Empty"" PRIMARY KEY (""Id"")
                    )");
            }
        }

        #endregion
    }
}
