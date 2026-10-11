#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Npgsql;
using System.Data;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.DbSettings;
using RepoDb.Schema;
using RepoDb.Schema.PostgreSql;

namespace RepoDb.Data.EnterpriseDb.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used for the PostgreSQL maintenance database.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used for the source database (the data is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used for the target database (the data is copied here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Maintenance connection
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_EDB_DATA_CONSTR_MASTER") ??
                "Server=127.0.0.1;Port=5433;Database=edb;User Id=enterprisedb;Password=RepoDB2026;Pooling=false;";

            // Source connection
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_EDB_DATA_CONSTR_SOURCE") ??
                "Server=127.0.0.1;Port=5433;Database=repodb_edb_source;User Id=enterprisedb;Password=RepoDB2026;Pooling=false;";

            // Target connection
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_EDB_DATA_CONSTR_TARGET") ??
                "Server=127.0.0.1;Port=5433;Database=repodb_edb_target;User Id=enterprisedb;Password=RepoDB2026;Pooling=false;";

            // Initialize the PostgreSql
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb(new EDBBulkOperationsDbSetting())
                .UsePostgreSqlSchema();

            // EnterpriseDB is compatible with PostgreSQL, so the schema composer of PostgreSQL composes the tables of its connection
            SchemaComposerMapper.Add<EDBConnection>(new PostgreSqlSchemaComposer(), force: true);

            // Create databases, the source tables (with their rows) and reset the target
            CreateDatabases();
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target database is dedicated to the tests, so everything in it is dropped
            using (var connection = new EDBConnection(ConnectionStringForTarget))
            {
                ResetSchemas(connection);
            }

            // The fields of the tables are cached (by the connection string and the name of the table), and the tables are not the same in the next test
            DbFieldCache.Flush();
        }

        #endregion

        #region Helpers

        private static void ResetSchemas(IDbConnection connection) =>
            connection.ExecuteNonQuery(@"DROP SCHEMA IF EXISTS public CASCADE;
                DROP SCHEMA IF EXISTS ""Sales"" CASCADE;
                CREATE SCHEMA public;
                CREATE SCHEMA ""Sales"";");

        private static void CreateDatabases()
        {
            using (var connection = new EDBConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { "repodb_edb_source", "repodb_edb_target" })
                {
                    var exists = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM pg_database WHERE datname = @Name;", new { Name = name });
                    if (exists == 0)
                    {
                        connection.ExecuteNonQuery($"CREATE DATABASE {name};");
                    }
                }
            }
        }

        #endregion

        #region CreateTables

        private static void CreateSourceTables()
        {
            using (var connection = new EDBConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The source tables are created (with their rows) only once: they are not changed by the tests
                var exists = connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM pg_tables WHERE schemaname = 'public' AND tablename = 'Country';");
                if (exists > 0)
                {
                    return;
                }

                ResetSchemas(connection);

                // The root of the tree: Country <- Person <- Pet <- Toy
                connection.ExecuteNonQuery(@"CREATE TABLE ""Country""
                    (
                        ""Id"" integer GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""Name"" varchar(100) NOT NULL,
                        CONSTRAINT ""PK_Country"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                    );
                    INSERT INTO ""Country"" (""Name"") VALUES ('Philippines'), ('Japan'), ('Germany');

                    CREATE TABLE ""Person""
                    (
                        ""Id"" integer GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""Name"" varchar(128) NOT NULL,
                        ""Age"" integer NULL,
                        ""CountryId"" integer NULL,
                        ""Salary"" numeric(18, 2) NULL,
                        ""Birthday"" date NULL,
                        ""IsActive"" boolean NOT NULL DEFAULT true,
                        ""CreatedDateUtc"" timestamp(3) NOT NULL DEFAULT (now() AT TIME ZONE 'utc'),
                        CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                        CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                    );
                    CREATE INDEX ""IX_Person_Name"" ON ""Person"" (""Name"");
                    INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES
                        ('Alice', 30, 1, 1000.50, '1990-01-15', true, '2026-01-01 10:00:00.123'),
                        ('Bob',   45, 2, 2500.00, '1975-06-30', true, '2026-01-02 11:00:00.456'),
                        ('Carol', NULL, NULL, NULL, NULL, false, '2026-01-03 12:00:00.789'),
                        ('Dave',  22, 1, 800.25,  '2003-12-01', true, '2026-01-04 13:00:00.000'),
                        ('Eve',   37, 3, 3100.75, '1988-03-09', false, '2026-01-05 14:00:00.321'),
                        ('Frank', 52, 2, NULL,    '1970-11-22', true, '2026-01-06 15:00:00.654');

                    CREATE TABLE ""Pet""
                    (
                        ""Id"" integer GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""Name"" varchar(50) NOT NULL,
                        ""PersonId"" integer NOT NULL,
                        CONSTRAINT ""PK_Pet"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Pet_Person"" FOREIGN KEY (""PersonId"") REFERENCES ""Person"" (""Id"")
                    );
                    INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Rex', 1), ('Tom', 1), ('Kiko', 2), ('Luna', 4), ('Milo', 5);

                    CREATE TABLE ""Toy""
                    (
                        ""Id"" integer GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""Name"" varchar(50) NOT NULL,
                        ""PetId"" integer NOT NULL,
                        CONSTRAINT ""PK_Toy"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Toy_Pet"" FOREIGN KEY (""PetId"") REFERENCES ""Pet"" (""Id"")
                    );
                    INSERT INTO ""Toy"" (""Name"", ""PetId"") VALUES ('Ball', 1), ('Bone', 1), ('Mouse', 2), ('Yarn', 3), ('Stick', 4);");

                // A table that is not related to any other table
                connection.ExecuteNonQuery(@"CREATE TABLE ""Currency""
                    (
                        ""Code"" char(3) NOT NULL,
                        ""Name"" varchar(50) NOT NULL,
                        CONSTRAINT ""PK_Currency"" PRIMARY KEY (""Code"")
                    );
                    INSERT INTO ""Currency"" (""Code"", ""Name"") VALUES ('PHP', 'Philippine Peso'), ('JPY', 'Japanese Yen'), ('EUR', 'Euro');");

                // One column of each of the common data types, and a row of NULL values
                connection.ExecuteNonQuery(@"CREATE TABLE ""Sample""
                    (
                        ""Id"" integer NOT NULL,
                        ""BigIntValue"" bigint NULL,
                        ""BitValue"" boolean NULL,
                        ""DecimalValue"" numeric(18, 4) NULL,
                        ""FloatValue"" double precision NULL,
                        ""DateTime2Value"" timestamp(6) NULL,
                        ""DateValue"" date NULL,
                        ""GuidValue"" uuid NULL,
                        ""BinaryValue"" bytea NULL,
                        ""TextValue"" text NULL,
                        ""CharValue"" varchar(50) NULL,
                        CONSTRAINT ""PK_Sample"" PRIMARY KEY (""Id"")
                    );
                    INSERT INTO ""Sample"" VALUES
                        (1, 9223372036854775807, true, 12345.6789, 3.14159265358979, '2026-02-03 04:05:06.123456', '2026-02-03', '6F9619FF-8B86-D011-B42D-00C04FC964FF', '\x0102030405', 'A long text with unicode: ñ é 日本語', 'abc'),
                        (2, -9223372036854775808, false, -0.0001, -2.5E-10, '0001-01-01 00:00:00', '9999-12-31', '00000000-0000-0000-0000-000000000000', '\x', '', ''),
                        (3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);");

                // A table whose name and column have a space
                connection.ExecuteNonQuery(@"CREATE TABLE ""Order Details""
                    (
                        ""Id"" integer NOT NULL,
                        ""Unit Price"" numeric(10, 2) NOT NULL,
                        CONSTRAINT ""PK_OrderDetails"" PRIMARY KEY (""Id"")
                    );
                    INSERT INTO ""Order Details"" (""Id"", ""Unit Price"") VALUES (1, 9.99), (2, 19.50);");

                // Tables of a schema that is not the default one, and a table of the default schema that references it
                connection.ExecuteNonQuery(@"CREATE TABLE ""Sales"".""Invoice""
                    (
                        ""Id"" integer NOT NULL,
                        ""Total"" numeric(19, 4) NOT NULL,
                        CONSTRAINT ""PK_Invoice"" PRIMARY KEY (""Id"")
                    );
                    CREATE TABLE ""Sales"".""InvoiceLine""
                    (
                        ""Id"" integer NOT NULL,
                        ""InvoiceId"" integer NOT NULL,
                        ""Description"" varchar(100) NOT NULL,
                        CONSTRAINT ""PK_InvoiceLine"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_InvoiceLine_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Sales"".""Invoice"" (""Id"")
                    );
                    CREATE TABLE ""Ledger""
                    (
                        ""Id"" integer NOT NULL,
                        ""InvoiceId"" integer NOT NULL,
                        CONSTRAINT ""PK_Ledger"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Ledger_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Sales"".""Invoice"" (""Id"")
                    );
                    INSERT INTO ""Sales"".""Invoice"" (""Id"", ""Total"") VALUES (1, 100.0000), (2, 250.5000);
                    INSERT INTO ""Sales"".""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (1, 1, 'Line 1'), (2, 1, 'Line 2'), (3, 2, 'Line 3');
                    INSERT INTO ""Ledger"" (""Id"", ""InvoiceId"") VALUES (1, 1), (2, 2);");

                // A table with many rows, to copy the rows in many batches
                connection.ExecuteNonQuery(@"CREATE TABLE ""Number""
                    (
                        ""Value"" integer NOT NULL,
                        ""Label"" varchar(20) NOT NULL,
                        CONSTRAINT ""PK_Number"" PRIMARY KEY (""Value"")
                    );
                    INSERT INTO ""Number"" (""Value"", ""Label"") SELECT n, 'Number ' || n FROM generate_series(1, 2500) AS n;");

                // A self-referencing foreign key
                connection.ExecuteNonQuery(@"CREATE TABLE ""Employee""
                    (
                        ""Id"" integer NOT NULL,
                        ""Name"" varchar(50) NOT NULL,
                        ""ManagerId"" integer NULL,
                        CONSTRAINT ""PK_Employee"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Employee_Manager"" FOREIGN KEY (""ManagerId"") REFERENCES ""Employee"" (""Id"")
                    );
                    INSERT INTO ""Employee"" (""Id"", ""Name"", ""ManagerId"") VALUES (1, 'CEO', NULL), (2, 'VP', 1), (3, 'Developer', 2);");

                // A cycle of 2 tables
                connection.ExecuteNonQuery(@"CREATE TABLE ""CycleA""
                    (
                        ""Id"" integer NOT NULL,
                        ""BId"" integer NULL,
                        CONSTRAINT ""PK_CycleA"" PRIMARY KEY (""Id"")
                    );
                    CREATE TABLE ""CycleB""
                    (
                        ""Id"" integer NOT NULL,
                        ""AId"" integer NULL,
                        CONSTRAINT ""PK_CycleB"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_CycleB_CycleA"" FOREIGN KEY (""AId"") REFERENCES ""CycleA"" (""Id"")
                    );
                    ALTER TABLE ""CycleA"" ADD CONSTRAINT ""FK_CycleA_CycleB"" FOREIGN KEY (""BId"") REFERENCES ""CycleB"" (""Id"");
                    INSERT INTO ""CycleA"" (""Id"", ""BId"") VALUES (1, NULL);
                    INSERT INTO ""CycleB"" (""Id"", ""AId"") VALUES (1, 1);
                    UPDATE ""CycleA"" SET ""BId"" = 1 WHERE ""Id"" = 1;");

                // A table without a primary key
                connection.ExecuteNonQuery(@"CREATE TABLE ""NoKey""
                    (
                        ""Value"" varchar(50) NULL,
                        ""Amount"" integer NULL
                    );
                    INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1), ('A', 1), (NULL, NULL);");

                // A table without rows
                connection.ExecuteNonQuery(@"CREATE TABLE ""Empty""
                    (
                        ""Id"" integer NOT NULL,
                        ""Name"" varchar(50) NULL,
                        CONSTRAINT ""PK_Empty"" PRIMARY KEY (""Id"")
                    );");
            }
        }

        #endregion
    }
}
