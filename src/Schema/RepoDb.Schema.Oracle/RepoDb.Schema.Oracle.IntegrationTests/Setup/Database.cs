#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Schema.Oracle.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the name of the source user (the schema is read from here).
        /// </summary>
        public const string SourceName = "REPODB_SRC";

        /// <summary>
        /// Gets the name of the target user (the schema is created here).
        /// </summary>
        public const string TargetName = "REPODB_TGT";

        /// <summary>
        /// Gets the name of the other user of the source (a user is the schema of Oracle, so it plays the role of a non-default schema).
        /// </summary>
        public const string SourceSalesName = "REPODB_SRC_SALES";

        /// <summary>
        /// Gets the name of the other user of the target (it plays the role of a non-default schema).
        /// </summary>
        public const string TargetSalesName = "REPODB_TGT_SALES";

        /// <summary>
        /// Gets the connection string to be used for the Oracle system user.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the source user (the schema is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the target user (the schema is created here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // The connections
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_ORACLE_SCHEMA_CONSTR_MASTER") ??
                "User Id=system;Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;";
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_ORACLE_SCHEMA_CONSTR_SOURCE") ??
                $"User Id={SourceName};Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;";
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_ORACLE_SCHEMA_CONSTR_TARGET") ??
                $"User Id={TargetName};Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;";

            // Initialize the Oracle
            GlobalConfiguration
                .Setup()
                .UseOracleSchema();

            // Create the source tables and clean the target users
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
        }

        #endregion

        #region Helpers

        private static bool _sourceTablesCreated;

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
            // The target user creates the tables of the other target user
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

        private static void CreateSourceTables()
        {
            if (_sourceTablesCreated)
            {
                return;
            }

            using (var master = new OracleConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { SourceName, SourceSalesName, TargetName, TargetSalesName })
                {
                    CreateUser(master, name);
                }
                GrantPrivileges(master);
                DropTables(master, SourceName);
                DropTables(master, SourceSalesName);
            }

            using (var sales = new OracleConnection($"User Id={SourceSalesName};Password=RepoDB2026;Data Source=localhost:1521/FREEPDB1;").EnsureOpen())
            {
                CreateSalesTables(sales);
            }

            using (var connection = new OracleConnection(ConnectionStringForSource).EnsureOpen())
            {
                CreateMainTables(connection);
                CreateRelationshipTables(connection);
                CreateIndexTables(connection);
                CreateOddNameTables(connection);
            }

            _sourceTablesCreated = true;
        }

        private static void CreateMainTables(IDbConnection connection) =>
            Execute(connection,
                @"CREATE TABLE ""Country"" (
                    ""Id"" NUMBER(10) GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                    ""Name"" VARCHAR2(100) NOT NULL,
                    CONSTRAINT ""PK_Country"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                )",
                @"CREATE TABLE ""Person"" (
                    ""Id"" NUMBER(19) GENERATED BY DEFAULT AS IDENTITY (START WITH 10 INCREMENT BY 5) NOT NULL,
                    ""Name"" VARCHAR2(128) NOT NULL,
                    ""NameUpper"" VARCHAR2(128) GENERATED ALWAYS AS (UPPER(""Name"")) VIRTUAL,
                    ""Age"" NUMBER(10) DEFAULT 0,
                    ""CountryId"" NUMBER(10),
                    ""Salary"" NUMBER(18, 2),
                    ""CreatedDateUtc"" TIMESTAMP(3) DEFAULT SYS_EXTRACT_UTC(SYSTIMESTAMP) NOT NULL,
                    CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                    CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"") ON DELETE SET NULL
                )",
                @"CREATE INDEX ""IX_Person_Name"" ON ""Person"" (""Name"" DESC, ""Age"")",
                @"CREATE TABLE ""OrderLine"" (
                    ""OrderId"" NUMBER(10) NOT NULL,
                    ""LineNumber"" NUMBER(10) NOT NULL,
                    ""Quantity"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_OrderLine"" PRIMARY KEY (""OrderId"", ""LineNumber"")
                )",
                @"CREATE TABLE ""NoKey"" (
                    ""Value"" CLOB,
                    ""Payload"" BLOB
                )",
                @"CREATE TABLE ""Ledger"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""InvoiceId"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_Ledger"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""Item"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_Item"" PRIMARY KEY (""Id"")
                )");

        private static void CreateSalesTables(IDbConnection connection) =>
            Execute(connection,
                @"CREATE TABLE ""Invoice"" (
                    ""Id"" NUMBER(10) GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                    ""Total"" NUMBER(19, 4) NOT NULL,
                    CONSTRAINT ""PK_Invoice"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""InvoiceLine"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""InvoiceId"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_InvoiceLine"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_InvoiceLine_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Invoice"" (""Id"") ON DELETE CASCADE
                )",
                @"CREATE TABLE ""Item"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_SalesItem"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""ItemRef"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""ItemId"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_ItemRef"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_ItemRef_Item"" FOREIGN KEY (""ItemId"") REFERENCES ""Item"" (""Id"")
                )");

        private static void CreateRelationshipTables(IDbConnection connection) =>
            Execute(connection,
                // A dependency chain
                @"CREATE TABLE ""Parent"" (""Id"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_Parent"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""Child"" (""Id"" NUMBER(10) NOT NULL, ""ParentId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_Child"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Child_Parent"" FOREIGN KEY (""ParentId"") REFERENCES ""Parent"" (""Id""))",
                @"CREATE TABLE ""GrandChild"" (""Id"" NUMBER(10) NOT NULL, ""ChildId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_GrandChild"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_GrandChild_Child"" FOREIGN KEY (""ChildId"") REFERENCES ""Child"" (""Id""))",

                // A cycle of 2 tables
                @"CREATE TABLE ""CycleA"" (""Id"" NUMBER(10) NOT NULL, ""BId"" NUMBER(10), CONSTRAINT ""PK_CycleA"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""CycleB"" (""Id"" NUMBER(10) NOT NULL, ""AId"" NUMBER(10), CONSTRAINT ""PK_CycleB"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_CycleB_CycleA"" FOREIGN KEY (""AId"") REFERENCES ""CycleA"" (""Id""))",
                @"ALTER TABLE ""CycleA"" ADD CONSTRAINT ""FK_CycleA_CycleB"" FOREIGN KEY (""BId"") REFERENCES ""CycleB"" (""Id"")",

                // A composite foreign key and a second foreign key (with a cascading rule) in the same table
                @"CREATE TABLE ""Shipment"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""OrderId"" NUMBER(10) NOT NULL,
                    ""LineNumber"" NUMBER(10) NOT NULL,
                    ""CountryId"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_Shipment"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Shipment_OrderLine"" FOREIGN KEY (""OrderId"", ""LineNumber"") REFERENCES ""OrderLine"" (""OrderId"", ""LineNumber""),
                    CONSTRAINT ""FK_Shipment_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"") ON DELETE CASCADE
                )",

                // A self-referencing foreign key
                @"CREATE TABLE ""Employee"" (""Id"" NUMBER(10) NOT NULL, ""ManagerId"" NUMBER(10), CONSTRAINT ""PK_Employee"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Employee_Manager"" FOREIGN KEY (""ManagerId"") REFERENCES ""Employee"" (""Id""))",

                // A foreign key with the default (no action) delete rule
                @"CREATE TABLE ""Preference"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""CountryId"" NUMBER(10) DEFAULT 1 NOT NULL,
                    CONSTRAINT ""PK_Preference"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Preference_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"")
                )",

                // A table with 2 parents that share the same parent: A <- B, A <- C, B <- D, C <- D
                @"CREATE TABLE ""DiamondA"" (""Id"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_DiamondA"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""DiamondB"" (""Id"" NUMBER(10) NOT NULL, ""AId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_DiamondB"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_DiamondB_DiamondA"" FOREIGN KEY (""AId"") REFERENCES ""DiamondA"" (""Id""))",
                @"CREATE TABLE ""DiamondC"" (""Id"" NUMBER(10) NOT NULL, ""AId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_DiamondC"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_DiamondC_DiamondA"" FOREIGN KEY (""AId"") REFERENCES ""DiamondA"" (""Id""))",
                @"CREATE TABLE ""DiamondD"" (""Id"" NUMBER(10) NOT NULL, ""BId"" NUMBER(10) NOT NULL, ""CId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_DiamondD"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_DiamondD_DiamondB"" FOREIGN KEY (""BId"") REFERENCES ""DiamondB"" (""Id""),
                    CONSTRAINT ""FK_DiamondD_DiamondC"" FOREIGN KEY (""CId"") REFERENCES ""DiamondC"" (""Id""))",

                // A table with 2 foreign keys to the same parent table
                @"CREATE TABLE ""Account"" (""Id"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_Account"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""Transfer"" (""Id"" NUMBER(10) NOT NULL, ""FromAccountId"" NUMBER(10) NOT NULL, ""ToAccountId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_Transfer"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Transfer_FromAccount"" FOREIGN KEY (""FromAccountId"") REFERENCES ""Account"" (""Id""),
                    CONSTRAINT ""FK_Transfer_ToAccount"" FOREIGN KEY (""ToAccountId"") REFERENCES ""Account"" (""Id""))",

                // A cycle of 3 tables: X -> Z -> Y -> X
                @"CREATE TABLE ""RingX"" (""Id"" NUMBER(10) NOT NULL, ""ZId"" NUMBER(10), CONSTRAINT ""PK_RingX"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""RingY"" (""Id"" NUMBER(10) NOT NULL, ""XId"" NUMBER(10), CONSTRAINT ""PK_RingY"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_RingY_RingX"" FOREIGN KEY (""XId"") REFERENCES ""RingX"" (""Id""))",
                @"CREATE TABLE ""RingZ"" (""Id"" NUMBER(10) NOT NULL, ""YId"" NUMBER(10), CONSTRAINT ""PK_RingZ"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_RingZ_RingY"" FOREIGN KEY (""YId"") REFERENCES ""RingY"" (""Id""))",
                @"ALTER TABLE ""RingX"" ADD CONSTRAINT ""FK_RingX_RingZ"" FOREIGN KEY (""ZId"") REFERENCES ""RingZ"" (""Id"")",

                // A cycle (A <-> B) with a table that the cycle depends on (Root) and a table that depends on the cycle (Leaf)
                @"CREATE TABLE ""LoopRoot"" (""Id"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_LoopRoot"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""LoopA"" (""Id"" NUMBER(10) NOT NULL, ""RootId"" NUMBER(10) NOT NULL, ""BId"" NUMBER(10), CONSTRAINT ""PK_LoopA"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_LoopA_LoopRoot"" FOREIGN KEY (""RootId"") REFERENCES ""LoopRoot"" (""Id""))",
                @"CREATE TABLE ""LoopB"" (""Id"" NUMBER(10) NOT NULL, ""AId"" NUMBER(10), CONSTRAINT ""PK_LoopB"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_LoopB_LoopA"" FOREIGN KEY (""AId"") REFERENCES ""LoopA"" (""Id""))",
                @"ALTER TABLE ""LoopA"" ADD CONSTRAINT ""FK_LoopA_LoopB"" FOREIGN KEY (""BId"") REFERENCES ""LoopB"" (""Id"")",
                @"CREATE TABLE ""LoopLeaf"" (""Id"" NUMBER(10) NOT NULL, ""BId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_LoopLeaf"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_LoopLeaf_LoopB"" FOREIGN KEY (""BId"") REFERENCES ""LoopB"" (""Id""))",

                // A table with many children
                @"CREATE TABLE ""FanRoot"" (""Id"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_FanRoot"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""FanChild1"" (""Id"" NUMBER(10) NOT NULL, ""RootId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_FanChild1"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_FanChild1_FanRoot"" FOREIGN KEY (""RootId"") REFERENCES ""FanRoot"" (""Id""))",
                @"CREATE TABLE ""FanChild2"" (""Id"" NUMBER(10) NOT NULL, ""RootId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_FanChild2"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_FanChild2_FanRoot"" FOREIGN KEY (""RootId"") REFERENCES ""FanRoot"" (""Id""))",
                @"CREATE TABLE ""FanChild3"" (""Id"" NUMBER(10) NOT NULL, ""RootId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_FanChild3"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_FanChild3_FanRoot"" FOREIGN KEY (""RootId"") REFERENCES ""FanRoot"" (""Id""))",
                @"CREATE TABLE ""FanChild4"" (""Id"" NUMBER(10) NOT NULL, ""RootId"" NUMBER(10) NOT NULL, CONSTRAINT ""PK_FanChild4"" PRIMARY KEY (""Id""), CONSTRAINT ""FK_FanChild4_FanRoot"" FOREIGN KEY (""RootId"") REFERENCES ""FanRoot"" (""Id""))");

        private static void CreateIndexTables(IDbConnection connection) =>
            Execute(connection,
                // 2 unique indexes, a multi-column index with a descending key and an index of one column
                @"CREATE TABLE ""Product"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""Code"" VARCHAR2(20) NOT NULL,
                    ""Sku"" VARCHAR2(30) NOT NULL,
                    ""Name"" VARCHAR2(100) NOT NULL,
                    ""Category"" NUMBER(10) NOT NULL,
                    ""Price"" NUMBER(18, 2) NOT NULL,
                    ""IsActive"" NUMBER(1) DEFAULT 1 NOT NULL,
                    CONSTRAINT ""PK_Product"" PRIMARY KEY (""Id"")
                )",
                @"CREATE UNIQUE INDEX ""CIX_Product_Code"" ON ""Product"" (""Code"")",
                @"CREATE UNIQUE INDEX ""UX_Product_Sku"" ON ""Product"" (""Sku"")",
                @"CREATE INDEX ""IX_Product_Category_Price"" ON ""Product"" (""Category"" ASC, ""Price"" DESC)",
                @"CREATE INDEX ""IX_Product_Active_Name"" ON ""Product"" (""Name"")",

                // The types
                @"CREATE TABLE ""AllTypes"" (
                    ""CNumber"" NUMBER,
                    ""CNumberInt"" NUMBER(10) NOT NULL,
                    ""CNumberScale"" NUMBER(10, 3),
                    ""CFloat"" FLOAT(126),
                    ""CBinaryFloat"" BINARY_FLOAT,
                    ""CBinaryDouble"" BINARY_DOUBLE,
                    ""CChar"" CHAR(5),
                    ""CVarChar"" VARCHAR2(20),
                    ""CNChar"" NCHAR(5),
                    ""CNVarChar"" NVARCHAR2(30),
                    ""CClob"" CLOB,
                    ""CNClob"" NCLOB,
                    ""CRaw"" RAW(16),
                    ""CBlob"" BLOB,
                    ""CDate"" DATE,
                    ""CTimestamp"" TIMESTAMP(4),
                    ""CTimestampTz"" TIMESTAMP(3) WITH TIME ZONE,
                    ""CTimestampLtz"" TIMESTAMP(2) WITH LOCAL TIME ZONE,
                    ""CIntervalYm"" INTERVAL YEAR(2) TO MONTH,
                    ""CIntervalDs"" INTERVAL DAY(3) TO SECOND(4),
                    ""CJson"" JSON,
                    ""CBoolean"" BOOLEAN
                )");

        private static void CreateOddNameTables(IDbConnection connection) =>
            Execute(connection,
                // Tables whose names contain a dot, a space and a double quote
                @"CREATE TABLE ""Odd.Name"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""Value"" VARCHAR2(50),
                    CONSTRAINT ""PK_OddName"" PRIMARY KEY (""Id"")
                )",
                @"CREATE INDEX ""IX_OddName_Value"" ON ""Odd.Name"" (""Value"")",
                @"CREATE TABLE ""Odd.Child"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""ParentId"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_OddChild"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_OddChild_OddName"" FOREIGN KEY (""ParentId"") REFERENCES ""Odd.Name"" (""Id"")
                )",
                @"CREATE TABLE ""Order Details"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    ""Unit Price"" NUMBER(10, 2) NOT NULL,
                    CONSTRAINT ""PK_OrderDetails"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""Weird]Name"" (
                    ""Id"" NUMBER(10) NOT NULL,
                    CONSTRAINT ""PK_WeirdName"" PRIMARY KEY (""Id"")
                )");

        #endregion
    }
}
