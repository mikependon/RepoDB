#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Turso.Data.Sqlite;

namespace RepoDb.Schema.Sqlite.Turso.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

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
            // The databases are files of the temporary directory
            var directory = Environment.GetEnvironmentVariable("REPODB_SQLITE_SCHEMA_DIRECTORY") ??
                Path.Combine(Path.GetTempPath(), "repodb_schema_sqlite");
            Directory.CreateDirectory(directory);
            SourcePath = Path.Combine(directory, "source.db");
            TargetPath = Path.Combine(directory, "target.db");
            ConnectionStringForSource = $"Data Source={SourcePath};Pooling=False;";
            ConnectionStringForTarget = $"Data Source={TargetPath};Pooling=False;";

            // Initialize the SQLite
            GlobalConfiguration
                .Setup()
                .UseTursoSchema();

            // Create the source tables and clean the target databases
            CreateSourceTables();
            Cleanup();
        }

        /// <summary>
        /// Creates and opens a connection to the source database. The connection enforces the foreign keys.
        /// </summary>
        /// <returns>The opened <see cref="SqliteConnection"/>.</returns>
        public static SqliteConnection CreateSource() =>
            Open(ConnectionStringForSource, true);

        /// <summary>
        /// Creates and opens a connection to the target database. The connection enforces the foreign keys.
        /// </summary>
        /// <returns>The opened <see cref="SqliteConnection"/>.</returns>
        public static SqliteConnection CreateTarget() =>
            Open(ConnectionStringForTarget, true);

        public static void Cleanup()
        {
            // The target databases are dedicated to the tests, so all of their tables are dropped (the foreign keys are not enforced here)
            using (var connection = Open(ConnectionStringForTarget, false))
            {
                foreach (var schema in new[] { "main" })
                {
                    var tables = new List<string>();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT name FROM \"{schema}\".sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite\\_%' ESCAPE '\\' AND name NOT LIKE '\\_\\_turso\\_internal%' ESCAPE '\\';";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(reader.GetString(0));
                            }
                        }
                    }
                    foreach (var table in tables)
                    {
                        connection.ExecuteNonQuery($"DROP TABLE \"{schema}\".\"{table.Replace("\"", "\"\"")}\";");
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private static string SourcePath;
        private static string TargetPath;
        private static bool _sourceTablesCreated;

        private static SqliteConnection Open(string connectionString,
            bool enforceForeignKeys)
        {
            var connection = new SqliteConnection(connectionString);
            connection.Open();
            connection.ExecuteNonQuery($"PRAGMA foreign_keys = {(enforceForeignKeys ? "ON" : "OFF")};");
            return connection;
        }

        private static void CreateSourceTables()
        {
            if (_sourceTablesCreated)
            {
                return;
            }

            foreach (var path in new[] { SourcePath })
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            using (var connection = Open(ConnectionStringForSource, false))
            {
                connection.ExecuteNonQuery(@"
                    CREATE TABLE Country (
                        Id INTEGER NOT NULL CONSTRAINT PK_Country PRIMARY KEY AUTOINCREMENT,
                        Name VARCHAR(100) NOT NULL,
                        CONSTRAINT UQ_Country_Name UNIQUE (Name)
                    );

                    CREATE TABLE Person (
                        Id INTEGER NOT NULL CONSTRAINT PK_Person PRIMARY KEY AUTOINCREMENT,
                        Name VARCHAR(128) NOT NULL COLLATE NOCASE,
                        Age INT NULL DEFAULT 0,
                        CountryId INT NULL,
                        Salary DECIMAL(18, 2) NULL,
                        CreatedDateUtc DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        CONSTRAINT CK_Person_Age CHECK (Age >= 0),
                        CONSTRAINT FK_Person_Country FOREIGN KEY (CountryId) REFERENCES Country (Id) ON DELETE SET NULL ON UPDATE CASCADE
                    );
                    CREATE INDEX IX_Person_Name ON Person (Name DESC, Age);

                    CREATE TABLE OrderLine (
                        OrderId INT NOT NULL,
                        LineNumber INT NOT NULL,
                        Quantity INT NOT NULL,
                        CONSTRAINT PK_OrderLine PRIMARY KEY (OrderId, LineNumber)
                    );

                    CREATE TABLE NoKey (
                        Value TEXT NULL,
                        Payload BLOB NULL
                    );");

                CreateRelationshipTables(connection);
                CreateIndexTables(connection);
                CreateOddNameTables(connection);
            }

            _sourceTablesCreated = true;
        }

        private static void CreateRelationshipTables(IDbConnection connection) =>
            connection.ExecuteNonQuery(@"
                -- A dependency chain
                CREATE TABLE Parent (Id INT NOT NULL, CONSTRAINT PK_Parent PRIMARY KEY (Id));
                CREATE TABLE Child (Id INT NOT NULL, ParentId INT NOT NULL, CONSTRAINT PK_Child PRIMARY KEY (Id),
                    CONSTRAINT FK_Child_Parent FOREIGN KEY (ParentId) REFERENCES Parent (Id));
                CREATE TABLE GrandChild (Id INT NOT NULL, ChildId INT NOT NULL, CONSTRAINT PK_GrandChild PRIMARY KEY (Id),
                    CONSTRAINT FK_GrandChild_Child FOREIGN KEY (ChildId) REFERENCES Child (Id));

                -- A cycle of 2 tables
                CREATE TABLE CycleA (Id INT NOT NULL, BId INT NULL, CONSTRAINT PK_CycleA PRIMARY KEY (Id),
                    CONSTRAINT FK_CycleA_CycleB FOREIGN KEY (BId) REFERENCES CycleB (Id));
                CREATE TABLE CycleB (Id INT NOT NULL, AId INT NULL, CONSTRAINT PK_CycleB PRIMARY KEY (Id),
                    CONSTRAINT FK_CycleB_CycleA FOREIGN KEY (AId) REFERENCES CycleA (Id));

                -- A composite foreign key and a second foreign key (with a cascading rule) in the same table
                CREATE TABLE Shipment (
                    Id INT NOT NULL,
                    OrderId INT NOT NULL,
                    LineNumber INT NOT NULL,
                    CountryId INT NOT NULL,
                    CONSTRAINT PK_Shipment PRIMARY KEY (Id),
                    CONSTRAINT FK_Shipment_OrderLine FOREIGN KEY (OrderId, LineNumber) REFERENCES OrderLine (OrderId, LineNumber),
                    CONSTRAINT FK_Shipment_Country FOREIGN KEY (CountryId) REFERENCES Country (Id) ON DELETE CASCADE
                );

                -- A self-referencing foreign key
                CREATE TABLE Employee (Id INT NOT NULL, ManagerId INT NULL, CONSTRAINT PK_Employee PRIMARY KEY (Id),
                    CONSTRAINT FK_Employee_Manager FOREIGN KEY (ManagerId) REFERENCES Employee (Id));

                -- A foreign key with the SET DEFAULT rules
                CREATE TABLE Preference (
                    Id INT NOT NULL,
                    CountryId INT NOT NULL DEFAULT 1,
                    CONSTRAINT PK_Preference PRIMARY KEY (Id),
                    CONSTRAINT FK_Preference_Country FOREIGN KEY (CountryId) REFERENCES Country (Id) ON DELETE SET DEFAULT ON UPDATE SET DEFAULT
                );

                -- A table with 2 parents that share the same parent: A <- B, A <- C, B <- D, C <- D
                CREATE TABLE DiamondA (Id INT NOT NULL, CONSTRAINT PK_DiamondA PRIMARY KEY (Id));
                CREATE TABLE DiamondB (Id INT NOT NULL, AId INT NOT NULL, CONSTRAINT PK_DiamondB PRIMARY KEY (Id),
                    CONSTRAINT FK_DiamondB_DiamondA FOREIGN KEY (AId) REFERENCES DiamondA (Id));
                CREATE TABLE DiamondC (Id INT NOT NULL, AId INT NOT NULL, CONSTRAINT PK_DiamondC PRIMARY KEY (Id),
                    CONSTRAINT FK_DiamondC_DiamondA FOREIGN KEY (AId) REFERENCES DiamondA (Id));
                CREATE TABLE DiamondD (Id INT NOT NULL, BId INT NOT NULL, CId INT NOT NULL, CONSTRAINT PK_DiamondD PRIMARY KEY (Id),
                    CONSTRAINT FK_DiamondD_DiamondB FOREIGN KEY (BId) REFERENCES DiamondB (Id),
                    CONSTRAINT FK_DiamondD_DiamondC FOREIGN KEY (CId) REFERENCES DiamondC (Id));

                -- A table with 2 foreign keys to the same parent table
                CREATE TABLE Account (Id INT NOT NULL, CONSTRAINT PK_Account PRIMARY KEY (Id));
                CREATE TABLE Transfer (Id INT NOT NULL, FromAccountId INT NOT NULL, ToAccountId INT NOT NULL, CONSTRAINT PK_Transfer PRIMARY KEY (Id),
                    CONSTRAINT FK_Transfer_FromAccount FOREIGN KEY (FromAccountId) REFERENCES Account (Id),
                    CONSTRAINT FK_Transfer_ToAccount FOREIGN KEY (ToAccountId) REFERENCES Account (Id));

                -- A cycle of 3 tables: X -> Z -> Y -> X
                CREATE TABLE RingX (Id INT NOT NULL, ZId INT NULL, CONSTRAINT PK_RingX PRIMARY KEY (Id),
                    CONSTRAINT FK_RingX_RingZ FOREIGN KEY (ZId) REFERENCES RingZ (Id));
                CREATE TABLE RingY (Id INT NOT NULL, XId INT NULL, CONSTRAINT PK_RingY PRIMARY KEY (Id),
                    CONSTRAINT FK_RingY_RingX FOREIGN KEY (XId) REFERENCES RingX (Id));
                CREATE TABLE RingZ (Id INT NOT NULL, YId INT NULL, CONSTRAINT PK_RingZ PRIMARY KEY (Id),
                    CONSTRAINT FK_RingZ_RingY FOREIGN KEY (YId) REFERENCES RingY (Id));

                -- A cycle (A <-> B) with a table that the cycle depends on (Root) and a table that depends on the cycle (Leaf)
                CREATE TABLE LoopRoot (Id INT NOT NULL, CONSTRAINT PK_LoopRoot PRIMARY KEY (Id));
                CREATE TABLE LoopA (Id INT NOT NULL, RootId INT NOT NULL, BId INT NULL, CONSTRAINT PK_LoopA PRIMARY KEY (Id),
                    CONSTRAINT FK_LoopA_LoopRoot FOREIGN KEY (RootId) REFERENCES LoopRoot (Id),
                    CONSTRAINT FK_LoopA_LoopB FOREIGN KEY (BId) REFERENCES LoopB (Id));
                CREATE TABLE LoopB (Id INT NOT NULL, AId INT NULL, CONSTRAINT PK_LoopB PRIMARY KEY (Id),
                    CONSTRAINT FK_LoopB_LoopA FOREIGN KEY (AId) REFERENCES LoopA (Id));
                CREATE TABLE LoopLeaf (Id INT NOT NULL, BId INT NOT NULL, CONSTRAINT PK_LoopLeaf PRIMARY KEY (Id),
                    CONSTRAINT FK_LoopLeaf_LoopB FOREIGN KEY (BId) REFERENCES LoopB (Id));

                -- A table with many children
                CREATE TABLE FanRoot (Id INT NOT NULL, CONSTRAINT PK_FanRoot PRIMARY KEY (Id));
                CREATE TABLE FanChild1 (Id INT NOT NULL, RootId INT NOT NULL, CONSTRAINT PK_FanChild1 PRIMARY KEY (Id), CONSTRAINT FK_FanChild1_FanRoot FOREIGN KEY (RootId) REFERENCES FanRoot (Id));
                CREATE TABLE FanChild2 (Id INT NOT NULL, RootId INT NOT NULL, CONSTRAINT PK_FanChild2 PRIMARY KEY (Id), CONSTRAINT FK_FanChild2_FanRoot FOREIGN KEY (RootId) REFERENCES FanRoot (Id));
                CREATE TABLE FanChild3 (Id INT NOT NULL, RootId INT NOT NULL, CONSTRAINT PK_FanChild3 PRIMARY KEY (Id), CONSTRAINT FK_FanChild3_FanRoot FOREIGN KEY (RootId) REFERENCES FanRoot (Id));
                CREATE TABLE FanChild4 (Id INT NOT NULL, RootId INT NOT NULL, CONSTRAINT PK_FanChild4 PRIMARY KEY (Id), CONSTRAINT FK_FanChild4_FanRoot FOREIGN KEY (RootId) REFERENCES FanRoot (Id));");

        private static void CreateIndexTables(IDbConnection connection) =>
            connection.ExecuteNonQuery(@"
                -- A unique index, a unique index and a multi-column index with a descending key, and a partial index
                CREATE TABLE Product (
                    Id INT NOT NULL,
                    Code VARCHAR(20) NOT NULL,
                    Sku VARCHAR(30) NOT NULL,
                    Name VARCHAR(100) NOT NULL,
                    Category INT NOT NULL,
                    Price DECIMAL(18, 2) NOT NULL,
                    IsActive BOOLEAN NOT NULL DEFAULT 1,
                    CONSTRAINT PK_Product PRIMARY KEY (Id)
                );
                CREATE UNIQUE INDEX CIX_Product_Code ON Product (Code);
                CREATE UNIQUE INDEX UX_Product_Sku ON Product (Sku);
                CREATE INDEX IX_Product_Category_Price ON Product (Category ASC, Price DESC);
                CREATE INDEX IX_Product_Active_Name ON Product (Name) WHERE IsActive = 1;

                -- The types
                CREATE TABLE AllTypes (
                    CTinyInt TINYINT NOT NULL,
                    CSmallInt SMALLINT NOT NULL,
                    CMediumInt MEDIUMINT NULL,
                    CInt INT NOT NULL,
                    CBigInt BIGINT NULL,
                    CUnsignedBigInt UNSIGNED BIG INT NULL,
                    CDecimal DECIMAL(10, 3) NULL,
                    CNumeric NUMERIC NULL,
                    CFloat FLOAT NULL,
                    CDouble DOUBLE PRECISION NULL,
                    CReal REAL NULL,
                    CBoolean BOOLEAN NULL,
                    CChar CHAR(5) NULL,
                    CVarChar VARCHAR(20) NULL,
                    CNVarChar NVARCHAR(30) NULL,
                    CText TEXT NULL,
                    CClob CLOB NULL,
                    CBlob BLOB NULL,
                    CDate DATE NULL,
                    CTime TIME NULL,
                    CDateTime DATETIME NULL,
                    CTimestamp TIMESTAMP NULL,
                    CJson JSON NULL,
                    CNoType NULL
                );");

        private static void CreateOddNameTables(IDbConnection connection) =>
            connection.ExecuteNonQuery(@"
                -- Tables whose names contain a dot and a space
                CREATE TABLE ""Odd.Name"" (
                    Id INT NOT NULL,
                    Value VARCHAR(50) NULL,
                    CONSTRAINT PK_OddName PRIMARY KEY (Id)
                );
                CREATE INDEX IX_OddName_Value ON ""Odd.Name"" (Value);
                CREATE TABLE ""Odd.Child"" (
                    Id INT NOT NULL,
                    ParentId INT NOT NULL,
                    CONSTRAINT PK_OddChild PRIMARY KEY (Id),
                    CONSTRAINT FK_OddChild_OddName FOREIGN KEY (ParentId) REFERENCES ""Odd.Name"" (Id)
                );
                CREATE TABLE ""Order Details"" (
                    Id INT NOT NULL,
                    ""Unit Price"" DECIMAL(10, 2) NOT NULL,
                    CONSTRAINT PK_OrderDetails PRIMARY KEY (Id)
                );");

        #endregion
    }
}
