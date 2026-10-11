#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using RepoDb.DbSettings;
using RepoDb.Schema.Oracle;

namespace RepoDb.Data.Oracle.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the name of the source user (the data is read from here).
        /// </summary>
        public const string SourceName = "REPODB_DATA_SRC";

        /// <summary>
        /// Gets the name of the other user of the source (a user is the schema of Oracle, so it plays the role of the <c>Sales</c> schema).
        /// </summary>
        public const string SourceSalesName = "REPODB_DATA_SRC_SALES";

        /// <summary>
        /// Gets the name of the target user (the data is copied here).
        /// </summary>
        public const string TargetName = "REPODB_DATA_TGT";

        /// <summary>
        /// Gets the name of the other user of the target (it plays the role of the <c>Sales</c> schema).
        /// </summary>
        public const string TargetSalesName = "REPODB_DATA_TGT_SALES";

        /// <summary>
        /// Gets the connection string to be used for the Oracle system user.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the source user (the data is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the target user (the data is copied here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // The connections
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_ORACLE_DATA_CONSTR_MASTER") ??
                "User Id=system;Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;Pooling=false;";
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_ORACLE_DATA_CONSTR_SOURCE") ??
                $"User Id={SourceName};Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;Pooling=false;";
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_ORACLE_DATA_CONSTR_TARGET") ??
                $"User Id={TargetName};Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;Pooling=false;";

            // Initialize the Oracle
            GlobalConfiguration
                .Setup()
                .UseOracle(new OracleBulkOperationsDbSetting())
                .UseOracleSchema();

            // Create the source tables (with their rows) and clean the target users
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target users are dedicated to the tests, so all of their tables are dropped
            using (var connection = new OracleConnection(ConnectionStringForMaster).EnsureOpen())
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
            var exists = Convert.ToInt32(connection.ExecuteScalar($"SELECT COUNT(*) FROM ALL_USERS WHERE USERNAME = '{name}'"));
            if (exists == 0)
            {
                connection.ExecuteNonQuery($"CREATE USER {name} IDENTIFIED BY \"RepoDB2026\" QUOTA UNLIMITED ON USERS");
                connection.ExecuteNonQuery($"GRANT CONNECT, RESOURCE, UNLIMITED TABLESPACE TO {name}");
            }
        }

        private static void GrantPrivileges(IDbConnection connection)
        {
            // The source user reads the tables of its other user (the foreign keys reference them), and the target user creates the tables of the other target user
            connection.ExecuteNonQuery($"GRANT SELECT ANY TABLE TO {SourceName}");
            connection.ExecuteNonQuery($"GRANT CREATE ANY TABLE, CREATE ANY SEQUENCE, SELECT ANY SEQUENCE, READ ANY TABLE, CREATE ANY INDEX, ALTER ANY TABLE, DROP ANY TABLE, SELECT ANY TABLE, INSERT ANY TABLE, DELETE ANY TABLE TO {TargetName}");
        }

        private static void DropTables(IDbConnection connection, string owner) =>
            connection.ExecuteNonQuery($@"BEGIN
                    FOR t IN (SELECT TABLE_NAME FROM ALL_TABLES WHERE OWNER = '{owner}' AND TABLE_NAME NOT LIKE 'BIN$%') LOOP
                        EXECUTE IMMEDIATE 'DROP TABLE ""{owner}"".""' || t.TABLE_NAME || '"" CASCADE CONSTRAINTS PURGE';
                    END LOOP;
                END;");

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
            using (var master = new OracleConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { SourceName, SourceSalesName, TargetName, TargetSalesName })
                {
                    CreateUser(master, name);
                }
                GrantPrivileges(master);

                // The source tables are created (with their rows) only once: they are not changed by the tests
                var exists = Convert.ToInt32(master.ExecuteScalar($"SELECT COUNT(*) FROM ALL_TABLES WHERE OWNER = '{SourceName}' AND TABLE_NAME = 'Empty'"));
                if (exists > 0)
                {
                    return;
                }
                DropTables(master, SourceName);
                DropTables(master, SourceSalesName);

                // The tables of the Sales schema (another user)
                Execute(master,
                    $@"CREATE TABLE ""{SourceSalesName}"".""Invoice""
                    (
                        ""Id"" NUMBER(10) NOT NULL,
                        ""Total"" NUMBER(19, 4) NOT NULL,
                        CONSTRAINT ""PK_Invoice"" PRIMARY KEY (""Id"")
                    )",
                    $@"CREATE TABLE ""{SourceSalesName}"".""InvoiceLine""
                    (
                        ""Id"" NUMBER(10) NOT NULL,
                        ""InvoiceId"" NUMBER(10) NOT NULL,
                        ""Description"" VARCHAR2(100) NOT NULL,
                        CONSTRAINT ""PK_InvoiceLine"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_InvoiceLine_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""{SourceSalesName}"".""Invoice"" (""Id"")
                    )",
                    $@"INSERT INTO ""{SourceSalesName}"".""Invoice"" (""Id"", ""Total"") VALUES (1, 100.0000)",
                    $@"INSERT INTO ""{SourceSalesName}"".""Invoice"" (""Id"", ""Total"") VALUES (2, 250.5000)",
                    $@"INSERT INTO ""{SourceSalesName}"".""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (1, 1, 'Line 1')",
                    $@"INSERT INTO ""{SourceSalesName}"".""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (2, 1, 'Line 2')",
                    $@"INSERT INTO ""{SourceSalesName}"".""InvoiceLine"" (""Id"", ""InvoiceId"", ""Description"") VALUES (3, 2, 'Line 3')",
                    $@"GRANT SELECT, REFERENCES ON ""{SourceSalesName}"".""Invoice"" TO {SourceName}",
                    $@"GRANT SELECT, REFERENCES ON ""{SourceSalesName}"".""Invoice"" TO {TargetName}",
                    "COMMIT");
            }

            using (var connection = new OracleConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The root of the tree: Country <- Person <- Pet <- Toy
                Execute(connection,
                    @"CREATE TABLE ""Country""
                    (
                        ""Id"" NUMBER(10) GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""Name"" VARCHAR2(100) NOT NULL,
                        CONSTRAINT ""PK_Country"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                    )",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Philippines')",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Japan')",
                    @"INSERT INTO ""Country"" (""Name"") VALUES ('Germany')",
                    @"CREATE TABLE ""Person""
                    (
                        ""Id"" NUMBER(10) GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""Name"" VARCHAR2(128) NOT NULL,
                        ""Age"" NUMBER(10),
                        ""CountryId"" NUMBER(10),
                        ""Salary"" NUMBER(18, 2),
                        ""Birthday"" DATE,
                        ""IsActive"" NUMBER(1) DEFAULT 1 NOT NULL,
                        ""CreatedDateUtc"" TIMESTAMP(3) DEFAULT SYS_EXTRACT_UTC(SYSTIMESTAMP) NOT NULL,
                        CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                        CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                    )",
                    @"CREATE INDEX ""IX_Person_Name"" ON ""Person"" (""Name"")",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Alice', 30, 1, 1000.50, DATE '1990-01-15', 1, TIMESTAMP '2026-01-01 10:00:00.123')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Bob', 45, 2, 2500.00, DATE '1975-06-30', 1, TIMESTAMP '2026-01-02 11:00:00.456')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Carol', NULL, NULL, NULL, NULL, 0, TIMESTAMP '2026-01-03 12:00:00.789')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Dave', 22, 1, 800.25, DATE '2003-12-01', 1, TIMESTAMP '2026-01-04 13:00:00.000')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Eve', 37, 3, 3100.75, DATE '1988-03-09', 0, TIMESTAMP '2026-01-05 14:00:00.321')",
                    @"INSERT INTO ""Person"" (""Name"", ""Age"", ""CountryId"", ""Salary"", ""Birthday"", ""IsActive"", ""CreatedDateUtc"") VALUES ('Frank', 52, 2, NULL, DATE '1970-11-22', 1, TIMESTAMP '2026-01-06 15:00:00.654')",
                    @"CREATE TABLE ""Pet""
                    (
                        ""Id"" NUMBER(10) GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""Name"" VARCHAR2(50) NOT NULL,
                        ""PersonId"" NUMBER(10) NOT NULL,
                        CONSTRAINT ""PK_Pet"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Pet_Person"" FOREIGN KEY (""PersonId"") REFERENCES ""Person"" (""Id"")
                    )",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Rex', 1)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Tom', 1)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Kiko', 2)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Luna', 4)",
                    @"INSERT INTO ""Pet"" (""Name"", ""PersonId"") VALUES ('Milo', 5)",
                    @"CREATE TABLE ""Toy""
                    (
                        ""Id"" NUMBER(10) GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""Name"" VARCHAR2(50) NOT NULL,
                        ""PetId"" NUMBER(10) NOT NULL,
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
                        ""Name"" VARCHAR2(50) NOT NULL,
                        CONSTRAINT ""PK_Currency"" PRIMARY KEY (""Code"")
                    )",
                    @"INSERT INTO ""Currency"" (""Code"", ""Name"") VALUES ('PHP', 'Philippine Peso')",
                    @"INSERT INTO ""Currency"" (""Code"", ""Name"") VALUES ('JPY', 'Japanese Yen')",
                    @"INSERT INTO ""Currency"" (""Code"", ""Name"") VALUES ('EUR', 'Euro')");

                // One column of each of the common data types, and a row of NULL values
                Execute(connection,
                    @"CREATE TABLE ""Sample""
                    (
                        ""Id"" NUMBER(10) NOT NULL,
                        ""BigIntValue"" NUMBER(19),
                        ""BitValue"" NUMBER(1),
                        ""DecimalValue"" NUMBER(18, 4),
                        ""FloatValue"" BINARY_DOUBLE,
                        ""DateTime2Value"" TIMESTAMP(6),
                        ""DateValue"" DATE,
                        ""GuidValue"" CHAR(36),
                        ""BinaryValue"" BLOB,
                        ""TextValue"" CLOB,
                        ""CharValue"" VARCHAR2(50),
                        CONSTRAINT ""PK_Sample"" PRIMARY KEY (""Id"")
                    )",
                    @"INSERT INTO ""Sample"" VALUES (1, 9223372036854775807, 1, 12345.6789, 3.14159265358979, TIMESTAMP '2026-02-03 04:05:06.123456', DATE '2026-02-03', '6f9619ff-8b86-d011-b42d-00c04fc964ff', HEXTORAW('0102030405'), 'A long text with unicode: ñ é 日本語', 'abc')",
                    @"INSERT INTO ""Sample"" VALUES (2, -9223372036854775807, 0, -0.0001, -2.5E-10, TIMESTAMP '0001-01-01 00:00:00', DATE '9999-12-31', '00000000-0000-0000-0000-000000000000', HEXTORAW('FF'), 'text', 'z')",
                    @"INSERT INTO ""Sample"" VALUES (3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)");

                // A table whose name and column have a space
                Execute(connection,
                    @"CREATE TABLE ""Order Details""
                    (
                        ""Id"" NUMBER(10) NOT NULL,
                        ""Unit Price"" NUMBER(10, 2) NOT NULL,
                        CONSTRAINT ""PK_Order Details"" PRIMARY KEY (""Id"")
                    )",
                    @"INSERT INTO ""Order Details"" (""Id"", ""Unit Price"") VALUES (1, 9.99)",
                    @"INSERT INTO ""Order Details"" (""Id"", ""Unit Price"") VALUES (2, 19.50)");

                // A table with many rows, to copy the rows in many batches
                Execute(connection,
                    @"CREATE TABLE ""Number""
                    (
                        ""Value"" NUMBER(10) NOT NULL,
                        ""Label"" VARCHAR2(20) NOT NULL,
                        CONSTRAINT ""PK_Number"" PRIMARY KEY (""Value"")
                    )",
                    @"INSERT INTO ""Number"" (""Value"", ""Label"") SELECT LEVEL, 'Number ' || LEVEL FROM DUAL CONNECT BY LEVEL <= 2500");

                // A self-referencing foreign key
                Execute(connection,
                    @"CREATE TABLE ""Employee""
                    (
                        ""Id"" NUMBER(10) NOT NULL,
                        ""Name"" VARCHAR2(50) NOT NULL,
                        ""ManagerId"" NUMBER(10),
                        CONSTRAINT ""PK_Employee"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Employee_Manager"" FOREIGN KEY (""ManagerId"") REFERENCES ""Employee"" (""Id"")
                    )",
                    @"INSERT INTO ""Employee"" (""Id"", ""Name"", ""ManagerId"") VALUES (1, 'CEO', NULL)",
                    @"INSERT INTO ""Employee"" (""Id"", ""Name"", ""ManagerId"") VALUES (2, 'VP', 1)",
                    @"INSERT INTO ""Employee"" (""Id"", ""Name"", ""ManagerId"") VALUES (3, 'Developer', 2)");

                // A cycle of 2 tables
                Execute(connection,
                    @"CREATE TABLE ""CycleA"" (""Id"" NUMBER(10) NOT NULL, ""BId"" NUMBER(10), CONSTRAINT ""PK_CycleA"" PRIMARY KEY (""Id""))",
                    @"CREATE TABLE ""CycleB"" (""Id"" NUMBER(10) NOT NULL, ""AId"" NUMBER(10), CONSTRAINT ""PK_CycleB"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_CycleB_CycleA"" FOREIGN KEY (""AId"") REFERENCES ""CycleA"" (""Id""))",
                    @"ALTER TABLE ""CycleA"" ADD CONSTRAINT ""FK_CycleA_CycleB"" FOREIGN KEY (""BId"") REFERENCES ""CycleB"" (""Id"")",
                    @"INSERT INTO ""CycleA"" (""Id"", ""BId"") VALUES (1, NULL)",
                    @"INSERT INTO ""CycleB"" (""Id"", ""AId"") VALUES (1, 1)",
                    @"UPDATE ""CycleA"" SET ""BId"" = 1 WHERE ""Id"" = 1");

                // A table without a primary key
                Execute(connection,
                    @"CREATE TABLE ""NoKey"" (""Value"" VARCHAR2(50), ""Amount"" NUMBER(10))",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES ('A', 1)",
                    @"INSERT INTO ""NoKey"" (""Value"", ""Amount"") VALUES (NULL, NULL)");

                // A table of the default schema that references a table of the Sales schema (another user)
                Execute(connection,
                    $@"CREATE TABLE ""Ledger""
                    (
                        ""Id"" NUMBER(10) NOT NULL,
                        ""InvoiceId"" NUMBER(10) NOT NULL,
                        CONSTRAINT ""PK_Ledger"" PRIMARY KEY (""Id""),
                        CONSTRAINT ""FK_Ledger_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""{SourceSalesName}"".""Invoice"" (""Id"")
                    )",
                    @"INSERT INTO ""Ledger"" (""Id"", ""InvoiceId"") VALUES (1, 1)",
                    @"INSERT INTO ""Ledger"" (""Id"", ""InvoiceId"") VALUES (2, 2)");

                // A table without rows (it is the last one: it tells that the source tables are all created)
                Execute(connection,
                    @"CREATE TABLE ""Empty""
                    (
                        ""Id"" NUMBER(10) NOT NULL,
                        ""Name"" VARCHAR2(50),
                        CONSTRAINT ""PK_Empty"" PRIMARY KEY (""Id"")
                    )",
                    "COMMIT");
            }
        }

        #endregion
    }
}
