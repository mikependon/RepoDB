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
using RepoDb.DbSettings;
using RepoDb.Schema.DuckDb;

namespace RepoDb.Data.DuckDb.IntegrationTests.Setup
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
            // The connections (DuckDB has no server: the source and the target are 2 files)
            SourcePath = Environment.GetEnvironmentVariable("REPODB_DUCKDB_DATA_SOURCE") ?? Path.Combine(Path.GetTempPath(), "repodb_data_duckdb_source.duckdb");
            TargetPath = Environment.GetEnvironmentVariable("REPODB_DUCKDB_DATA_TARGET") ?? Path.Combine(Path.GetTempPath(), "repodb_data_duckdb_target.duckdb");
            ConnectionStringForSource = $"Data Source={SourcePath}";
            ConnectionStringForTarget = $"Data Source={TargetPath}";

            // Initialize the DuckDB
            GlobalConfiguration
                .Setup()
                .UseDuckDb(new DuckDbBulkOperationsDbSetting())
                .UseDuckDbSchema();

            // Create the source tables (with their rows) and clean the target database
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target database is dedicated to the tests, so all of its tables and sequences are dropped
            using (var connection = new DuckDBConnection(ConnectionStringForTarget).EnsureOpen())
            {
                DropObjects(connection);
            }

            // The fields of the tables are cached (by the connection string and the name of the table), and the tables are not the same in the next test
            DbFieldCache.Flush();
        }

        #endregion

        #region Helpers

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

            // The schema of the tests (the tables of the schema are dropped, but the schema itself stays)
            connection.ExecuteNonQuery("CREATE SCHEMA IF NOT EXISTS \"Sales\"");
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
            using (var master = new DuckDBConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The source tables are created (with their rows) only once: they are not changed by the tests
                var exists = Convert.ToInt32(master.ExecuteScalar("SELECT COUNT(*) FROM duckdb_tables() WHERE database_name = current_database() AND ((schema_name = 'main' AND table_name = 'Empty') OR (schema_name = 'Sales' AND table_name = 'Invoice'))")) == 2 ? 1 : 0;
                if (exists > 0)
                {
                    return;
                }
                DropObjects(master);
            }

            using (var connection = new DuckDBConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The root of the tree: Country <- Person <- Pet <- Toy
                Execute(connection,
                    @"CREATE SEQUENCE ""Country_Id_seq""",
                    @"CREATE TABLE ""Country""
                    (
                        ""Id"" INTEGER DEFAULT nextval('""Country_Id_seq""') NOT NULL,
                        ""Name"" VARCHAR(100) NOT NULL,
                        CONSTRAINT ""PK_Country"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                    )",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Philippines')",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Japan')",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Germany')",
                    @"CREATE SEQUENCE ""Person_Id_seq""",
                    @"CREATE TABLE ""Person""
                    (
                        ""Id"" INTEGER DEFAULT nextval('""Person_Id_seq""') NOT NULL,
                        ""Name"" VARCHAR(128) NOT NULL,
                        ""Age"" INTEGER,
                        ""CountryId"" INTEGER,
                        ""Salary"" DECIMAL(18, 2),
                        ""Birthday"" DATE,
                        ""IsActive"" BOOLEAN NOT NULL,
                        ""CreatedDateUtc"" TIMESTAMP NOT NULL,
                        CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                        CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                    )",
                    @"CREATE INDEX ""IX_Person_Name"" ON ""Person"" (""Name"")",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Alice', 30, 1, 1000.50, '1990-01-15', true, '2026-01-01 10:00:00.123')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Bob', 45, 2, 2500.00, '1975-06-30', true, '2026-01-02 11:00:00.456')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Carol', NULL, NULL, NULL, NULL, false, '2026-01-03 12:00:00.789')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Dave', 22, 1, 800.25, '2003-12-01', true, '2026-01-04 13:00:00.000')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Eve', 37, 3, 3100.75, '1988-03-09', false, '2026-01-05 14:00:00.321')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Frank', 52, 2, NULL, '1970-11-22', true, '2026-01-06 15:00:00.654')",
                    @"CREATE SEQUENCE ""Pet_Id_seq""",
                    @"CREATE TABLE ""Pet""
                    (
                        ""Id"" INTEGER DEFAULT nextval('""Pet_Id_seq""') NOT NULL,
                        ""Name"" VARCHAR(50) NOT NULL,
                        ""PersonId"" INTEGER NOT NULL,
                        CONSTRAINT ""PK_Pet"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Pet_Person"" FOREIGN KEY (""PersonId"") REFERENCES ""Person"" (""Id"")
                    )",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Rex', 1)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Tom', 1)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Kiko', 2)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Luna', 4)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Milo', 5)",
                    @"CREATE SEQUENCE ""Toy_Id_seq""",
                    @"CREATE TABLE ""Toy""
                    (
                        ""Id"" INTEGER DEFAULT nextval('""Toy_Id_seq""') NOT NULL,
                        ""Name"" VARCHAR(50) NOT NULL,
                        ""PetId"" INTEGER NOT NULL,
                        CONSTRAINT ""PK_Toy"" PRIMARY KEY (""Id""),
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
                        ""GuidValue"" UUID,
                        ""BinaryValue"" BLOB,
                        ""TextValue"" TEXT,
                        ""CharValue"" VARCHAR(50),
                        CONSTRAINT ""PK_Sample"" PRIMARY KEY (""Id"")
                    )",
                    @"INSERT INTO ""Sample"" VALUES (1, 9223372036854775807, true, 12345.6789, 3.14159265358979, '2026-02-03 04:05:06.123456', '2026-02-03', '6f9619ff-8b86-d011-b42d-00c04fc964ff', '\x01\x02\x03\x04\x05'::BLOB, 'A long text with unicode: ñ é 日本語', 'abc')",
                    @"INSERT INTO ""Sample"" VALUES (2, -9223372036854775807, false, -0.0001, -2.5E-10, '0001-01-01 00:00:00', '9999-12-31', '00000000-0000-0000-0000-000000000000', ''::BLOB, '', '')",
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
                    @"INSERT INTO ""Number"" (""Value"", ""Label"") SELECT i, 'Number ' || i FROM range(1, 2501) t(i)");

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

                // A table without a primary key
                Execute(connection,
                    @"CREATE TABLE ""NoKey"" (""Value"" VARCHAR(50), ""Amount"" INTEGER)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES (NULL, NULL)");

                // Tables of a schema that is not the default one (DuckDB can not create a foreign key across the schemas)
                Execute(connection,
                    @"CREATE TABLE ""Sales"".""Invoice""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Total"" DECIMAL(19, 4) NOT NULL,
                        CONSTRAINT ""PK_Invoice"" PRIMARY KEY (""Id"")
                    )",
                    @"CREATE TABLE ""Sales"".""InvoiceLine""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""InvoiceId"" INTEGER NOT NULL,
                        ""Description"" VARCHAR(100) NOT NULL,
                        CONSTRAINT ""PK_InvoiceLine"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_InvoiceLine_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Sales"".""Invoice"" (""Id"")
                    )",
                    @"CREATE TABLE ""Sales"".""Ledger""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""InvoiceId"" INTEGER NOT NULL,
                        CONSTRAINT ""PK_Ledger"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Ledger_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Sales"".""Invoice"" (""Id"")
                    )",
                    @"INSERT INTO ""Sales"".""Invoice"" (""Id"", ""Total"") VALUES (1, 100.0000), (2, 250.5000)",
                    @"INSERT INTO ""Sales"".""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (1, 1, 'Line 1'), (2, 1, 'Line 2'), (3, 2, 'Line 3')",
                    @"INSERT INTO ""Sales"".""Ledger"" (""Id"", ""InvoiceId"") VALUES (1, 1), (2, 2)");

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
