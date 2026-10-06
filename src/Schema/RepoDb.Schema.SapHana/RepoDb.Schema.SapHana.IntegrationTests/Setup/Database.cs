#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using Sap.Data.Hana;

namespace RepoDb.Schema.SapHana.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the name of the source schema (the schema is read from here).
        /// </summary>
        public const string SourceName = "REPODB_SRC";

        /// <summary>
        /// Gets the name of the target schema (the schema is created here).
        /// </summary>
        public const string TargetName = "REPODB_TGT";

        /// <summary>
        /// Gets the name of the other schema of the source (it plays the role of a non-default schema).
        /// </summary>
        public const string SourceSalesName = "REPODB_SRC_SALES";

        /// <summary>
        /// Gets the name of the other schema of the target (it plays the role of a non-default schema).
        /// </summary>
        public const string TargetSalesName = "REPODB_TGT_SALES";

        /// <summary>
        /// Gets the connection string to be used for the SAP HANA administrator (it is used to create the schemas).
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the source schema (the schema is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the target schema (the schema is created here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // The connections (the current schema of a connection is the schema of the unqualified tables).
            // The port 39041 is the one of the tenant database of the HANA Express container.
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_SAPHANA_SCHEMA_CONSTR_MASTER") ??
                "Server=localhost:39041;UserID=SYSTEM;Password=RepoDB2026;";
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_SAPHANA_SCHEMA_CONSTR_SOURCE") ??
                $"{ConnectionStringForMaster}Current Schema={SourceName};";
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_SAPHANA_SCHEMA_CONSTR_TARGET") ??
                $"{ConnectionStringForMaster}Current Schema={TargetName};";

            // Initialize the SAP HANA
            GlobalConfiguration
                .Setup()
                .UseSapHanaSchema();

            // Create the source tables and clean the target schemas
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target schemas are dedicated to the tests, so all of their tables are dropped
            using (var connection = new HanaConnection(ConnectionStringForMaster).EnsureOpen())
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

        private static void CreateSchema(IDbConnection connection, string name)
        {
            // SAP HANA refuses to open a connection whose current schema does not exist, so the schemas are created over the connection of the administrator
            var exists = Convert.ToInt32(connection.ExecuteScalar("SELECT COUNT(*) FROM SYS.SCHEMAS WHERE SCHEMA_NAME = :Name", new { Name = name }));
            if (exists == 0)
            {
                connection.ExecuteNonQuery($"CREATE SCHEMA \"{name}\"");
            }
        }

        private static void DropTables(IDbConnection connection, string schema)
        {
            var tables = new List<string>();
            using (var reader = connection.ExecuteReader("SELECT TABLE_NAME FROM SYS.TABLES WHERE SCHEMA_NAME = :Schema AND IS_USER_DEFINED_TYPE = 'FALSE'", new { Schema = schema }))
            {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }
            foreach (var table in tables)
            {
                connection.ExecuteNonQuery($"DROP TABLE \"{schema}\".\"{table}\" CASCADE");
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

            using (var master = new HanaConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { SourceName, SourceSalesName, TargetName, TargetSalesName })
                {
                    CreateSchema(master, name);
                }
                DropTables(master, SourceName);
                DropTables(master, SourceSalesName);
            }

            using (var sales = new HanaConnection($"{ConnectionStringForMaster}Current Schema={SourceSalesName};").EnsureOpen())
            {
                CreateSalesTables(sales);
            }

            using (var connection = new HanaConnection(ConnectionStringForSource).EnsureOpen())
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
                    ""Id"" INTEGER GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                    ""Name"" NVARCHAR(100) NOT NULL,
                    CONSTRAINT ""PK_Country"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                )",
                @"CREATE TABLE ""Person"" (
                    ""Id"" BIGINT GENERATED BY DEFAULT AS IDENTITY (START WITH 10 INCREMENT BY 5) NOT NULL,
                    ""Name"" NVARCHAR(128) NOT NULL,
                    ""NameUpper"" NVARCHAR(128) GENERATED ALWAYS AS (UPPER(""Name"")),
                    ""Age"" INTEGER DEFAULT 0,
                    ""CountryId"" INTEGER,
                    ""Salary"" DECIMAL(18, 2),
                    ""CreatedDateUtc"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
                    CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                    CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"") ON DELETE SET NULL
                )",
                @"CREATE INDEX ""IX_Person_Name"" ON ""Person"" (""Name"" DESC, ""Age"")",
                @"CREATE TABLE ""OrderLine"" (
                    ""OrderId"" INTEGER NOT NULL,
                    ""LineNumber"" INTEGER NOT NULL,
                    ""Quantity"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_OrderLine"" PRIMARY KEY (""OrderId"", ""LineNumber"")
                )",
                @"CREATE TABLE ""NoKey"" (
                    ""Value"" NCLOB,
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
                @"CREATE TABLE ""Invoice"" (
                    ""Id"" INTEGER GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                    ""Total"" DECIMAL(19, 4) NOT NULL,
                    CONSTRAINT ""PK_Invoice"" PRIMARY KEY (""Id"")
                )",
                @"CREATE TABLE ""InvoiceLine"" (
                    ""Id"" INTEGER NOT NULL,
                    ""InvoiceId"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_InvoiceLine"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_InvoiceLine_Invoice"" FOREIGN KEY (""InvoiceId"") REFERENCES ""Invoice"" (""Id"") ON DELETE CASCADE
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

                // A cycle of 2 tables
                @"CREATE TABLE ""CycleA"" (""Id"" INTEGER NOT NULL, ""BId"" INTEGER, CONSTRAINT ""PK_CycleA"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""CycleB"" (""Id"" INTEGER NOT NULL, ""AId"" INTEGER, CONSTRAINT ""PK_CycleB"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_CycleB_CycleA"" FOREIGN KEY (""AId"") REFERENCES ""CycleA"" (""Id""))",
                @"ALTER TABLE ""CycleA"" ADD CONSTRAINT ""FK_CycleA_CycleB"" FOREIGN KEY (""BId"") REFERENCES ""CycleB"" (""Id"")",

                // A composite foreign key and a second foreign key (with a cascading rule) in the same table
                @"CREATE TABLE ""Shipment"" (
                    ""Id"" INTEGER NOT NULL,
                    ""OrderId"" INTEGER NOT NULL,
                    ""LineNumber"" INTEGER NOT NULL,
                    ""CountryId"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_Shipment"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_Shipment_OrderLine"" FOREIGN KEY (""OrderId"", ""LineNumber"") REFERENCES ""OrderLine"" (""OrderId"", ""LineNumber""),
                    CONSTRAINT ""FK_Shipment_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"") ON DELETE CASCADE
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

                // A cycle of 3 tables: X -> Z -> Y -> X
                @"CREATE TABLE ""RingX"" (""Id"" INTEGER NOT NULL, ""ZId"" INTEGER, CONSTRAINT ""PK_RingX"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""RingY"" (""Id"" INTEGER NOT NULL, ""XId"" INTEGER, CONSTRAINT ""PK_RingY"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_RingY_RingX"" FOREIGN KEY (""XId"") REFERENCES ""RingX"" (""Id""))",
                @"CREATE TABLE ""RingZ"" (""Id"" INTEGER NOT NULL, ""YId"" INTEGER, CONSTRAINT ""PK_RingZ"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_RingZ_RingY"" FOREIGN KEY (""YId"") REFERENCES ""RingY"" (""Id""))",
                @"ALTER TABLE ""RingX"" ADD CONSTRAINT ""FK_RingX_RingZ"" FOREIGN KEY (""ZId"") REFERENCES ""RingZ"" (""Id"")",

                // A cycle (A <-> B) with a table that the cycle depends on (Root) and a table that depends on the cycle (Leaf)
                @"CREATE TABLE ""LoopRoot"" (""Id"" INTEGER NOT NULL, CONSTRAINT ""PK_LoopRoot"" PRIMARY KEY (""Id""))",
                @"CREATE TABLE ""LoopA"" (""Id"" INTEGER NOT NULL, ""RootId"" INTEGER NOT NULL, ""BId"" INTEGER, CONSTRAINT ""PK_LoopA"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_LoopA_LoopRoot"" FOREIGN KEY (""RootId"") REFERENCES ""LoopRoot"" (""Id""))",
                @"CREATE TABLE ""LoopB"" (""Id"" INTEGER NOT NULL, ""AId"" INTEGER, CONSTRAINT ""PK_LoopB"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_LoopB_LoopA"" FOREIGN KEY (""AId"") REFERENCES ""LoopA"" (""Id""))",
                @"ALTER TABLE ""LoopA"" ADD CONSTRAINT ""FK_LoopA_LoopB"" FOREIGN KEY (""BId"") REFERENCES ""LoopB"" (""Id"")",
                @"CREATE TABLE ""LoopLeaf"" (""Id"" INTEGER NOT NULL, ""BId"" INTEGER NOT NULL, CONSTRAINT ""PK_LoopLeaf"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""FK_LoopLeaf_LoopB"" FOREIGN KEY (""BId"") REFERENCES ""LoopB"" (""Id""))",

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
                    ""Code"" NVARCHAR(20) NOT NULL,
                    ""Sku"" NVARCHAR(30) NOT NULL,
                    ""Name"" NVARCHAR(100) NOT NULL,
                    ""Category"" INTEGER NOT NULL,
                    ""Price"" DECIMAL(18, 2) NOT NULL,
                    ""IsActive"" SMALLINT DEFAULT 1 NOT NULL,
                    CONSTRAINT ""PK_Product"" PRIMARY KEY (""Id"")
                )",
                @"CREATE UNIQUE INDEX ""CIX_Product_Code"" ON ""Product"" (""Code"")",
                @"CREATE UNIQUE INDEX ""UX_Product_Sku"" ON ""Product"" (""Sku"")",
                @"CREATE INDEX ""IX_Product_Category_Price"" ON ""Product"" (""Category"" ASC, ""Price"" DESC)",
                @"CREATE INDEX ""IX_Product_Active_Name"" ON ""Product"" (""Name"")",

                // The types
                @"CREATE TABLE ""AllTypes"" (
                    ""CTinyInt"" TINYINT,
                    ""CSmallInt"" SMALLINT,
                    ""CInteger"" INTEGER NOT NULL,
                    ""CBigInt"" BIGINT,
                    ""CDecimal"" DECIMAL(10, 3),
                    ""CDecimalFloating"" DECIMAL,
                    ""CSmallDecimal"" SMALLDECIMAL,
                    ""CReal"" REAL,
                    ""CDouble"" DOUBLE,
                    ""CBoolean"" BOOLEAN,
                    ""CVarChar"" VARCHAR(20),
                    ""CNVarChar"" NVARCHAR(30),
                    ""CAlphaNum"" ALPHANUM(8),
                    ""CShortText"" SHORTTEXT(12),
                    ""CClob"" CLOB,
                    ""CNClob"" NCLOB,
                    ""CBlob"" BLOB,
                    ""CVarBinary"" VARBINARY(16),
                    ""CDate"" DATE,
                    ""CTime"" TIME,
                    ""CSecondDate"" SECONDDATE,
                    ""CTimestamp"" TIMESTAMP
                )");

        private static void CreateOddNameTables(IDbConnection connection) =>
            Execute(connection,
                // Tables whose names contain a dot, a space and a double quote
                @"CREATE TABLE ""Odd.Name"" (
                    ""Id"" INTEGER NOT NULL,
                    ""Value"" NVARCHAR(50),
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
