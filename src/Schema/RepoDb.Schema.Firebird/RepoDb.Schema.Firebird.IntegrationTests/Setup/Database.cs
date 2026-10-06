#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Schema.Firebird.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the connection string to be used for the Firebird administrator (it is used to create the databases).
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
            // The connections (Firebird has no schema, so the source and the target are 2 databases)
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_FIREBIRD_SCHEMA_CONSTR_MASTER") ??
                "DataSource=127.0.0.1;Port=3050;Database=/firebird/data/repodb.fdb;User=SYSDBA;Password=RepoDB2026;Charset=UTF8;Pooling=false;";
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_FIREBIRD_SCHEMA_CONSTR_SOURCE") ??
                "DataSource=127.0.0.1;Port=3050;Database=/firebird/data/repodb_schema_src.fdb;User=SYSDBA;Password=RepoDB2026;Charset=UTF8;Pooling=false;";
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_FIREBIRD_SCHEMA_CONSTR_TARGET") ??
                "DataSource=127.0.0.1;Port=3050;Database=/firebird/data/repodb_schema_tgt.fdb;User=SYSDBA;Password=RepoDB2026;Charset=UTF8;Pooling=false;";

            // Initialize the Firebird
            GlobalConfiguration
                .Setup()
                .UseFirebirdSchema();

            // Create the source tables and clean the target database
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target database is dedicated to the tests, so all of its tables are dropped
            using (var connection = new FbConnection(ConnectionStringForTarget).EnsureOpen())
            {
                DropTables(connection);
            }
        }

        #endregion

        #region Helpers

        private static bool _sourceTablesCreated;

        private static void DropTables(IDbConnection connection)
        {
            // The foreign keys are dropped first, so the tables can be dropped in any order
            foreach (var (table, name) in Read(connection, @"SELECT TRIM(c.RDB$RELATION_NAME), TRIM(c.RDB$CONSTRAINT_NAME) FROM RDB$RELATION_CONSTRAINTS c
                INNER JOIN RDB$RELATIONS r ON r.RDB$RELATION_NAME = c.RDB$RELATION_NAME
                WHERE c.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY' AND r.RDB$VIEW_BLR IS NULL AND COALESCE(r.RDB$SYSTEM_FLAG, 0) = 0"))
            {
                connection.ExecuteNonQuery($"ALTER TABLE \"{table}\" DROP CONSTRAINT \"{name}\"");
            }
            foreach (var (table, _) in Read(connection, "SELECT TRIM(RDB$RELATION_NAME), '' FROM RDB$RELATIONS WHERE RDB$VIEW_BLR IS NULL AND COALESCE(RDB$SYSTEM_FLAG, 0) = 0"))
            {
                connection.ExecuteNonQuery($"DROP TABLE \"{table}\"");
            }
        }

        private static List<(string First, string Second)> Read(IDbConnection connection, string sql)
        {
            var list = new List<(string, string)>();
            using (var reader = connection.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    list.Add((reader.GetString(0), reader.GetString(1)));
                }
            }
            return list;
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
            FbConnection.CreateDatabase(ConnectionStringForSource, 8192, true, true);
            FbConnection.CreateDatabase(ConnectionStringForTarget, 8192, true, true);

            using (var connection = new FbConnection(ConnectionStringForSource).EnsureOpen())
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
                    ""Name"" VARCHAR(100) NOT NULL,
                    CONSTRAINT ""PK_Country"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""UQ_Country_Name"" UNIQUE (""Name"")
                )",
                @"CREATE TABLE ""Person"" (
                    ""Id"" BIGINT GENERATED BY DEFAULT AS IDENTITY (START WITH 10 INCREMENT BY 5) NOT NULL,
                    ""Name"" VARCHAR(128) NOT NULL,
                    ""NameUpper"" VARCHAR(128) GENERATED ALWAYS AS (UPPER(""Name"")),
                    ""Age"" INTEGER DEFAULT 0,
                    ""CountryId"" INTEGER,
                    ""Salary"" DECIMAL(18, 2),
                    ""CreatedDateUtc"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
                    CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id""),
                    CONSTRAINT ""CK_Person_Age"" CHECK (""Age"" >= 0),
                    CONSTRAINT ""FK_Person_Country"" FOREIGN KEY (""CountryId"") REFERENCES ""Country"" (""Id"") ON DELETE SET NULL
                )",
                @"CREATE DESCENDING INDEX ""IX_Person_Name"" ON ""Person"" (""Name"", ""Age"")",
                @"CREATE TABLE ""OrderLine"" (
                    ""OrderId"" INTEGER NOT NULL,
                    ""LineNumber"" INTEGER NOT NULL,
                    ""Quantity"" INTEGER NOT NULL,
                    CONSTRAINT ""PK_OrderLine"" PRIMARY KEY (""OrderId"", ""LineNumber"")
                )",
                @"CREATE TABLE ""NoKey"" (
                    ""Value"" BLOB SUB_TYPE TEXT,
                    ""Payload"" BLOB SUB_TYPE BINARY
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
                @"CREATE DESCENDING INDEX ""IX_Product_Category_Price"" ON ""Product"" (""Category"", ""Price"")",
                @"CREATE INDEX ""IX_Product_Active_Name"" ON ""Product"" (""Name"")",

                // The types
                @"CREATE TABLE ""AllTypes"" (
                    ""CSmallInt"" SMALLINT,
                    ""CInteger"" INTEGER NOT NULL,
                    ""CBigInt"" BIGINT,
                    ""CInt128"" INT128,
                    ""CNumeric"" NUMERIC(10, 3),
                    ""CDecimal"" DECIMAL(18, 4),
                    ""CFloat"" FLOAT,
                    ""CDouble"" DOUBLE PRECISION,
                    ""CDecFloat16"" DECFLOAT(16),
                    ""CDecFloat34"" DECFLOAT(34),
                    ""CChar"" CHAR(5),
                    ""CVarChar"" VARCHAR(20),
                    ""CBinary"" BINARY(16),
                    ""CVarBinary"" VARBINARY(20),
                    ""CBlobText"" BLOB SUB_TYPE TEXT,
                    ""CBlob"" BLOB SUB_TYPE BINARY,
                    ""CDate"" DATE,
                    ""CTime"" TIME,
                    ""CTimestamp"" TIMESTAMP,
                    ""CTimeTz"" TIME WITH TIME ZONE,
                    ""CTimestampTz"" TIMESTAMP WITH TIME ZONE,
                    ""CBoolean"" BOOLEAN
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
