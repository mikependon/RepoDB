#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using Vertica.Data.VerticaClient;
using RepoDb.DbSettings;
using RepoDb.Schema.Vertica;

namespace RepoDb.Data.Vertica.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the name of the source schema (the data is read from here).
        /// </summary>
        public const string SourceName = "REPODB_DATA_SRC";

        /// <summary>
        /// Gets the name of the other schema of the source (it plays the role of the <c>Sales</c> schema).
        /// </summary>
        public const string SourceSalesName = "REPODB_DATA_SRC_SALES";

        /// <summary>
        /// Gets the name of the target schema (the data is copied here).
        /// </summary>
        public const string TargetName = "REPODB_DATA_TGT";

        /// <summary>
        /// Gets the name of the other schema of the target (it plays the role of the <c>Sales</c> schema).
        /// </summary>
        public const string TargetSalesName = "REPODB_DATA_TGT_SALES";

        /// <summary>
        /// Gets the connection string to be used for the Vertica administrator.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the source schema (the data is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the target schema (the data is copied here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // The connections (a user searches its own schema first, so the unqualified tables of a connection are the ones of the schema of its user)
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_VERTICA_DATA_CONSTR_MASTER") ??
                "Host=127.0.0.1;Port=5433;Database=RepoDb;User=dbadmin;Password=RepoDB2026;Pooling=false;";
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_VERTICA_DATA_CONSTR_SOURCE") ??
                $"Host=127.0.0.1;Port=5433;Database=RepoDb;User={SourceName};Password=RepoDB2026;Pooling=false;";
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_VERTICA_DATA_CONSTR_TARGET") ??
                $"Host=127.0.0.1;Port=5433;Database=RepoDb;User={TargetName};Password=RepoDB2026;Pooling=false;";

            // Initialize the Vertica
            GlobalConfiguration
                .Setup()
                .UseVertica(new VerticaBulkOperationsDbSetting())
                .UseVerticaSchema();

            // Create the source tables (with their rows) and clean the target schemas
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target schemas are dedicated to the tests, so all of their tables are dropped
            using (var connection = new VerticaConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { TargetName, TargetSalesName })
                {
                    DropTables(connection, name);
                }
            }

            // The fields of the tables are cached (by the connection string and the name of the table), and the tables are not the same in the next test
            DbFieldCache.Flush();
        }

        #endregion

        #region Helpers

        private static void CreateUser(IDbConnection connection, string name)
        {
            // A user searches its own schema first, so the unqualified tables of the connection of the user are created in its schema
            var exists = Convert.ToInt32(connection.ExecuteScalar("SELECT COUNT(*) FROM v_catalog.users WHERE user_name = @Name", new { Name = name }));
            if (exists == 0)
            {
                connection.ExecuteNonQuery($"CREATE SCHEMA IF NOT EXISTS \"{name}\"");
                connection.ExecuteNonQuery($"CREATE USER \"{name}\" IDENTIFIED BY 'RepoDB2026' SEARCH_PATH \"{name}\"");
                connection.ExecuteNonQuery($"GRANT DBADMIN TO \"{name}\"");
                connection.ExecuteNonQuery($"ALTER USER \"{name}\" DEFAULT ROLE DBADMIN");
            }
        }

        private static void DropTables(IDbConnection connection, string schema)
        {
            var tables = new List<string>();
            using (var reader = connection.ExecuteReader("SELECT table_name FROM v_catalog.tables WHERE table_schema = @Schema", new { Schema = schema }))
            {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }
            foreach (var table in tables)
            {
                connection.ExecuteNonQuery($"DROP TABLE IF EXISTS \"{schema}\".\"{table}\" CASCADE");
            }
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
            using (var master = new VerticaConnection(ConnectionStringForMaster).EnsureOpen())
            {
                // The source tables are created (with their rows) only once: they are not changed by the tests
                foreach (var name in new[] { SourceName, SourceSalesName, TargetName, TargetSalesName })
                {
                    CreateUser(master, name);
                }
                var exists = Convert.ToInt32(master.ExecuteScalar($"SELECT COUNT(*) FROM v_catalog.tables WHERE table_schema = '{SourceName}' AND table_name = 'Empty'"));
                if (exists > 0)
                {
                    return;
                }
                DropTables(master, SourceName);
                DropTables(master, SourceSalesName);

                // The tables of the Sales schema
                Execute(master,
                    $@"CREATE TABLE ""{SourceSalesName}"".""Invoice""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Total"" NUMERIC(19, 4) NOT NULL,
                        CONSTRAINT ""PK_Invoice"" PRIMARY KEY (""Id"")
                    )",
                    $@"CREATE TABLE ""{SourceSalesName}"".""InvoiceLine""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""InvoiceId"" INTEGER NOT NULL,
                        ""Description"" VARCHAR(100) NOT NULL,
                        CONSTRAINT ""PK_InvoiceLine"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_InvoiceLine_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""{SourceSalesName}"".""Invoice"" (""Id"")
                    )",
                    $@"INSERT INTO ""{SourceSalesName}"".""Invoice"" (""Id"", ""Total"") VALUES (1, 100.0000)",
                    $@"INSERT INTO ""{SourceSalesName}"".""Invoice"" (""Id"", ""Total"") VALUES (2, 250.5000)",
                    $@"INSERT INTO ""{SourceSalesName}"".""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (1, 1, 'Line 1')",
                    $@"INSERT INTO ""{SourceSalesName}"".""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (2, 1, 'Line 2')",
                    $@"INSERT INTO ""{SourceSalesName}"".""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (3, 2, 'Line 3')");
            }

            using (var connection = new VerticaConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The root of the tree: Country <- Person <- Pet <- Toy
                Execute(connection,
                    @"CREATE TABLE ""Country""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Name"" VARCHAR(100) NOT NULL,
                        CONSTRAINT ""PK_Country"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                    )",
                    @"INSERT INTO ""Country"" (""Id"", ""Name"") VALUES (1, 'Philippines')",
                    @"INSERT INTO ""Country"" (""Id"", ""Name"") VALUES (2, 'Japan')",
                    @"INSERT INTO ""Country"" (""Id"", ""Name"") VALUES (3, 'Germany')",
                    @"CREATE TABLE ""Person""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Name"" VARCHAR(128) NOT NULL,
                        ""Age"" INTEGER,
                        ""CountryId"" INTEGER,
                        ""Salary"" NUMERIC(18, 2),
                        ""Birthday"" DATE,
                        ""IsActive"" BOOLEAN DEFAULT TRUE NOT NULL,
                        ""CreatedDateUtc"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
                        CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                        CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                    )",
                    @"INSERT INTO ""Person"" (""Id"", ""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES (1, 'Alice', 30, 1, 1000.50, DATE '1990-01-15', TRUE, TIMESTAMP '2026-01-01 10:00:00.123')",
                    @"INSERT INTO ""Person"" (""Id"", ""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES (2, 'Bob', 45, 2, 2500.00, DATE '1975-06-30', TRUE, TIMESTAMP '2026-01-02 11:00:00.456')",
                    @"INSERT INTO ""Person"" (""Id"", ""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES (3, 'Carol', NULL, NULL, NULL, NULL, FALSE, TIMESTAMP '2026-01-03 12:00:00.789')",
                    @"INSERT INTO ""Person"" (""Id"", ""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES (4, 'Dave', 22, 1, 800.25, DATE '2003-12-01', TRUE, TIMESTAMP '2026-01-04 13:00:00.000')",
                    @"INSERT INTO ""Person"" (""Id"", ""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES (5, 'Eve', 37, 3, 3100.75, DATE '1988-03-09', FALSE, TIMESTAMP '2026-01-05 14:00:00.321')",
                    @"INSERT INTO ""Person"" (""Id"", ""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES (6, 'Frank', 52, 2, NULL, DATE '1970-11-22', TRUE, TIMESTAMP '2026-01-06 15:00:00.654')",
                    @"CREATE TABLE ""Pet""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Name"" VARCHAR(50) NOT NULL,
                        ""PersonId"" INTEGER NOT NULL,
                        CONSTRAINT ""PK_Pet"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Pet_Person"" FOREIGN KEY (""PersonId"") REFERENCES ""Person"" (""Id"")
                    )",
                    @"INSERT INTO ""Pet"" (""Id"", ""Name"", ""PersonId"") VALUES (1, 'Rex', 1)",
                    @"INSERT INTO ""Pet"" (""Id"", ""Name"", ""PersonId"") VALUES (2, 'Tom', 1)",
                    @"INSERT INTO ""Pet"" (""Id"", ""Name"", ""PersonId"") VALUES (3, 'Kiko', 2)",
                    @"INSERT INTO ""Pet"" (""Id"", ""Name"", ""PersonId"") VALUES (4, 'Luna', 4)",
                    @"INSERT INTO ""Pet"" (""Id"", ""Name"", ""PersonId"") VALUES (5, 'Milo', 5)",
                    @"CREATE TABLE ""Toy""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Name"" VARCHAR(50) NOT NULL,
                        ""PetId"" INTEGER NOT NULL,
                        CONSTRAINT ""PK_Toy"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Toy_Pet"" FOREIGN KEY (""PetId"") REFERENCES ""Pet"" (""Id"")
                    )",
                    @"INSERT INTO ""Toy"" (""Id"", ""Name"", ""PetId"") VALUES (1, 'Ball', 1)",
                    @"INSERT INTO ""Toy"" (""Id"", ""Name"", ""PetId"") VALUES (2, 'Bone', 1)",
                    @"INSERT INTO ""Toy"" (""Id"", ""Name"", ""PetId"") VALUES (3, 'Mouse', 2)",
                    @"INSERT INTO ""Toy"" (""Id"", ""Name"", ""PetId"") VALUES (4, 'Yarn', 3)",
                    @"INSERT INTO ""Toy"" (""Id"", ""Name"", ""PetId"") VALUES (5, 'Stick', 4)");

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
                        ""DecimalValue"" NUMERIC(18, 4),
                        ""FloatValue"" FLOAT,
                        ""DateTime2Value"" TIMESTAMP,
                        ""DateValue"" DATE,
                        ""GuidValue"" CHAR(36),
                        ""BinaryValue"" VARBINARY(100),
                        ""TextValue"" LONG VARCHAR,
                        ""CharValue"" VARCHAR(50),
                        CONSTRAINT ""PK_Sample"" PRIMARY KEY (""Id"")
                    )",
                    @"INSERT INTO ""Sample"" VALUES (1, 9223372036854775807, TRUE, 12345.6789, 3.14159265358979, TIMESTAMP '2026-02-03 04:05:06.123456', DATE '2026-02-03', '6f9619ff-8b86-d011-b42d-00c04fc964ff', X'0102030405', 'A long text with unicode: ñ é 日本語', 'abc')",
                    @"INSERT INTO ""Sample"" VALUES (2, -9223372036854775807, FALSE, -0.0001, -2.5E-10, TIMESTAMP '0001-01-01 00:00:00', DATE '9999-12-31', '00000000-0000-0000-0000-000000000000', X'FF', 'text', '')",
                    @"INSERT INTO ""Sample"" VALUES (3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)");

                // A table whose name and column have a space
                Execute(connection,
                    @"CREATE TABLE ""Order Details""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""Unit Price"" NUMERIC(10, 2) NOT NULL,
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
                    )");
                for (var start = 1; start <= 2500; start += 250)
                {
                    var rows = new List<string>();
                    for (var n = start; n < start + 250; n++)
                    {
                        rows.Add($"({n}, 'Number {n}')");
                    }
                    connection.ExecuteNonQuery($@"INSERT INTO ""Number"" (""Value"", ""Label"") {string.Join(" UNION ALL ", rows.ConvertAll(r => "SELECT " + r.Trim('(', ')')))}");
                }


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
                    @"CREATE TABLE ""CycleA"" (""Id"" INTEGER NOT NULL, ""BId"" INTEGER, CONSTRAINT ""PK_CycleA"" PRIMARY KEY (""Id""))",
                    @"CREATE TABLE ""CycleB"" (""Id"" INTEGER NOT NULL, ""AId"" INTEGER, CONSTRAINT ""PK_CycleB"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_CycleB_CycleA"" FOREIGN KEY (""AId"") REFERENCES ""CycleA"" (""Id""))",
                    @"ALTER TABLE ""CycleA"" ADD CONSTRAINT ""FK_CycleA_CycleB"" FOREIGN KEY (""BId"") REFERENCES ""CycleB"" (""Id"")",
                    @"INSERT INTO ""CycleA"" (""Id"", ""BId"") VALUES (1, NULL)",
                    @"INSERT INTO ""CycleB"" (""Id"", ""AId"") VALUES (1, 1)",
                    @"UPDATE ""CycleA"" SET ""BId"" = 1 WHERE ""Id"" = 1");

                // A table without a primary key
                Execute(connection,
                    @"CREATE TABLE ""NoKey"" (""Value"" VARCHAR(50), ""Amount"" INTEGER)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES (NULL, NULL)");

                // A table of the default schema that references a table of the Sales schema
                Execute(connection,
                    $@"CREATE TABLE ""Ledger""
                    (
                        ""Id"" INTEGER NOT NULL,
                        ""InvoiceId"" INTEGER NOT NULL,
                        CONSTRAINT ""PK_Ledger"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Ledger_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""{SourceSalesName}"".""Invoice"" (""Id"")
                    )",
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
