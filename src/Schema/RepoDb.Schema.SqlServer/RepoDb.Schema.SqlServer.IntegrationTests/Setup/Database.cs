#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.Data.SqlClient;

namespace RepoDb.Schema.SqlServer.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used for the SQL Server master database.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used for the source database (the schema is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used for the target database (the schema is created here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Master connection
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_SQLSVR_SCHEMA_CONSTR_MASTER") ??
                @"Server=tcp:127.0.0.1,1433;Database=master;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;";

            // Source connection
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_SQLSVR_SCHEMA_CONSTR_SOURCE") ??
                @"Server=tcp:127.0.0.1,1433;Database=RepoDb_Schema_Source;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;";

            // Target connection
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_SQLSVR_SCHEMA_CONSTR_TARGET") ??
                @"Server=tcp:127.0.0.1,1433;Database=RepoDb_Schema_Target;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;";

            // Initialize the SqlServer
            GlobalConfiguration
                .Setup()
                .UseSqlServer();

            // Initialize the SqlServer schema objects
            SqlServerSchemaBootstrap.Initialize();

            // Create databases
            CreateDatabases();

            // Create the source tables and the target schemas
            CreateSourceTables();
            CreateTargetSchemas();
        }

        public static void Cleanup()
        {
            // The target database is dedicated to the tests, so everything in it is dropped (the foreign keys first,
            // so the order of the tables and the circular references do not matter)
            using (var connection = new SqlConnection(ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(@"DECLARE @Sql NVARCHAR(MAX) = N'';
                    SELECT @Sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.tables t ON t.object_id = fk.parent_object_id;
                    EXEC sys.sp_executesql @Sql;

                    SET @Sql = N'';
                    SELECT @Sql += N'DROP TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name) + N';'
                    FROM sys.tables t;
                    EXEC sys.sp_executesql @Sql;");
            }
        }

        #endregion

        #region CreateDatabases

        private static void CreateDatabases()
        {
            using (var connection = new SqlConnection(ConnectionStringForMaster).EnsureOpen())
            {
                connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RepoDb_Schema_Source'))
                    BEGIN
                        CREATE DATABASE [RepoDb_Schema_Source];
                    END");
                connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RepoDb_Schema_Target'))
                    BEGIN
                        CREATE DATABASE [RepoDb_Schema_Target];
                    END");
            }
        }

        #endregion

        #region CreateTables

        private static void CreateSourceTables()
        {
            using (var connection = new SqlConnection(ConnectionStringForSource))
            {
                CreateCountryTable(connection);
                CreatePersonTable(connection);
                CreateOrderLineTable(connection);
                CreateNoKeyTable(connection);
                CreateInvoiceTable(connection);
                CreateDependencyTables(connection);
                CreateCycleTables(connection);
                CreateProductTable(connection);
                CreateShipmentTable(connection);
                CreateEmployeeTable(connection);
                CreatePreferenceTable(connection);
                CreateInvoiceReferencingTables(connection);
                CreateDiamondTables(connection);
                CreateTransferTables(connection);
                CreateRingTables(connection);
                CreateLoopTables(connection);
                CreateFanTables(connection);
                CreateItemTables(connection);
                CreateOddNameTables(connection);
            }
        }

        private static void CreateCountryTable(SqlConnection connection)
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Country'))
                BEGIN
                    CREATE TABLE [dbo].[Country]
                    (
                        [Id] INT IDENTITY(1, 1) NOT NULL,
                        [Name] NVARCHAR(100) NOT NULL,
                        CONSTRAINT [PK_Country] PRIMARY KEY ([Id]),
                        CONSTRAINT [UQ_Country_Name] UNIQUE ([Name])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreatePersonTable(SqlConnection connection)
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Person'))
                BEGIN
                    CREATE TABLE [dbo].[Person]
                    (
                        [Id] BIGINT IDENTITY(10, 5) NOT NULL,
                        [Name] NVARCHAR(128) COLLATE Latin1_General_CI_AS NOT NULL,
                        [NameUpper] AS (UPPER([Name])),
                        [Age] INT NULL CONSTRAINT [DF_Person_Age] DEFAULT ((0)),
                        [CountryId] INT NULL,
                        [Salary] DECIMAL(18, 2) NULL,
                        [CreatedDateUtc] DATETIME2(3) NOT NULL CONSTRAINT [DF_Person_CreatedDateUtc] DEFAULT (SYSUTCDATETIME()),
                        CONSTRAINT [PK_Person] PRIMARY KEY ([Id]),
                        CONSTRAINT [CK_Person_Age] CHECK ([Age] >= 0),
                        CONSTRAINT [FK_Person_Country] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country] ([Id]) ON DELETE SET NULL ON UPDATE CASCADE
                    );
                    CREATE INDEX [IX_Person_Name] ON [dbo].[Person] ([Name]) INCLUDE ([Age]);
                    EXEC sys.sp_addextendedproperty
                        @name = N'MS_Description',
                        @value = N'The name of the person.',
                        @level0type = N'SCHEMA', @level0name = N'dbo',
                        @level1type = N'TABLE', @level1name = N'Person',
                        @level2type = N'COLUMN', @level2name = N'Name';
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateOrderLineTable(SqlConnection connection)
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'OrderLine'))
                BEGIN
                    CREATE TABLE [dbo].[OrderLine]
                    (
                        [OrderId] INT NOT NULL,
                        [LineNumber] INT NOT NULL,
                        [Quantity] INT NOT NULL,
                        CONSTRAINT [PK_OrderLine] PRIMARY KEY ([OrderId], [LineNumber])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateNoKeyTable(SqlConnection connection)
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'NoKey'))
                BEGIN
                    CREATE TABLE [dbo].[NoKey]
                    (
                        [Value] NVARCHAR(MAX) NULL,
                        [Payload] VARBINARY(MAX) NULL
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateInvoiceTable(SqlConnection connection)
        {
            var commandText = @"IF (SCHEMA_ID(N'Sales') IS NULL)
                BEGIN
                    EXEC(N'CREATE SCHEMA [Sales]');
                END";
            connection.ExecuteNonQuery(commandText);

            commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Invoice'))
                BEGIN
                    CREATE TABLE [Sales].[Invoice]
                    (
                        [Id] INT IDENTITY(1, 1) NOT NULL,
                        [Total] DECIMAL(19, 4) NOT NULL,
                        CONSTRAINT [PK_Invoice] PRIMARY KEY ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDependencyTables(SqlConnection connection)
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Parent'))
                BEGIN
                    CREATE TABLE [dbo].[Parent]
                    (
                        [Id] INT NOT NULL,
                        CONSTRAINT [PK_Parent] PRIMARY KEY ([Id])
                    );
                    CREATE TABLE [dbo].[Child]
                    (
                        [Id] INT NOT NULL,
                        [ParentId] INT NOT NULL,
                        CONSTRAINT [PK_Child] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Child_Parent] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Parent] ([Id])
                    );
                    CREATE TABLE [dbo].[GrandChild]
                    (
                        [Id] INT NOT NULL,
                        [ChildId] INT NOT NULL,
                        CONSTRAINT [PK_GrandChild] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_GrandChild_Child] FOREIGN KEY ([ChildId]) REFERENCES [dbo].[Child] ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateCycleTables(SqlConnection connection)
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'CycleA'))
                BEGIN
                    CREATE TABLE [dbo].[CycleA]
                    (
                        [Id] INT NOT NULL,
                        [BId] INT NULL,
                        CONSTRAINT [PK_CycleA] PRIMARY KEY ([Id])
                    );
                    CREATE TABLE [dbo].[CycleB]
                    (
                        [Id] INT NOT NULL,
                        [AId] INT NULL,
                        CONSTRAINT [PK_CycleB] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_CycleB_CycleA] FOREIGN KEY ([AId]) REFERENCES [dbo].[CycleA] ([Id])
                    );
                    ALTER TABLE [dbo].[CycleA]
                        ADD CONSTRAINT [FK_CycleA_CycleB] FOREIGN KEY ([BId]) REFERENCES [dbo].[CycleB] ([Id]);
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateProductTable(SqlConnection connection)
        {
            // A nonclustered primary key, a unique clustered index, a unique nonclustered index,
            // a multi-column index with a descending key and an included column, and a filtered index
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Product'))
                BEGIN
                    CREATE TABLE [dbo].[Product]
                    (
                        [Id] INT NOT NULL,
                        [Code] NVARCHAR(20) NOT NULL,
                        [Sku] NVARCHAR(30) NOT NULL,
                        [Name] NVARCHAR(100) NOT NULL,
                        [Category] INT NOT NULL,
                        [Price] DECIMAL(18, 2) NOT NULL,
                        [IsActive] BIT NOT NULL CONSTRAINT [DF_Product_IsActive] DEFAULT ((1)),
                        CONSTRAINT [PK_Product] PRIMARY KEY NONCLUSTERED ([Id])
                    );
                    CREATE UNIQUE CLUSTERED INDEX [CIX_Product_Code] ON [dbo].[Product] ([Code]);
                    CREATE UNIQUE NONCLUSTERED INDEX [UX_Product_Sku] ON [dbo].[Product] ([Sku]);
                    CREATE NONCLUSTERED INDEX [IX_Product_Category_Price] ON [dbo].[Product] ([Category] ASC, [Price] DESC) INCLUDE ([Name]);
                    CREATE NONCLUSTERED INDEX [IX_Product_Active_Name] ON [dbo].[Product] ([Name]) WHERE ([IsActive] = 1);
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateShipmentTable(SqlConnection connection)
        {
            // A composite foreign key and a second foreign key (with a cascading rule) in the same table
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Shipment'))
                BEGIN
                    CREATE TABLE [dbo].[Shipment]
                    (
                        [Id] INT NOT NULL,
                        [OrderId] INT NOT NULL,
                        [LineNumber] INT NOT NULL,
                        [CountryId] INT NOT NULL,
                        CONSTRAINT [PK_Shipment] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Shipment_OrderLine] FOREIGN KEY ([OrderId], [LineNumber]) REFERENCES [dbo].[OrderLine] ([OrderId], [LineNumber]),
                        CONSTRAINT [FK_Shipment_Country] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country] ([Id]) ON DELETE CASCADE
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateEmployeeTable(SqlConnection connection)
        {
            // A self-referencing foreign key
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Employee'))
                BEGIN
                    CREATE TABLE [dbo].[Employee]
                    (
                        [Id] INT NOT NULL,
                        [ManagerId] INT NULL,
                        CONSTRAINT [PK_Employee] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Employee_Manager] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[Employee] ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreatePreferenceTable(SqlConnection connection)
        {
            // A foreign key with the SET DEFAULT rules
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Preference'))
                BEGIN
                    CREATE TABLE [dbo].[Preference]
                    (
                        [Id] INT NOT NULL,
                        [CountryId] INT NOT NULL CONSTRAINT [DF_Preference_CountryId] DEFAULT ((1)),
                        CONSTRAINT [PK_Preference] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Preference_Country] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country] ([Id]) ON DELETE SET DEFAULT ON UPDATE SET DEFAULT
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateInvoiceReferencingTables(SqlConnection connection)
        {
            // A foreign key within the same (non-default) schema, and a foreign key across the schemas
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'InvoiceLine'))
                BEGIN
                    CREATE TABLE [Sales].[InvoiceLine]
                    (
                        [Id] INT NOT NULL,
                        [InvoiceId] INT NOT NULL,
                        CONSTRAINT [PK_InvoiceLine] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_InvoiceLine_Invoice] FOREIGN KEY ([InvoiceId]) REFERENCES [Sales].[Invoice] ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);

            commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Ledger'))
                BEGIN
                    CREATE TABLE [dbo].[Ledger]
                    (
                        [Id] INT NOT NULL,
                        [InvoiceId] INT NOT NULL,
                        CONSTRAINT [PK_Ledger] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Ledger_Invoice] FOREIGN KEY ([InvoiceId]) REFERENCES [Sales].[Invoice] ([Id]) ON DELETE CASCADE
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDiamondTables(SqlConnection connection)
        {
            // A table with 2 parents that share the same parent: A <- B, A <- C, B <- D, C <- D
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'DiamondA'))
                BEGIN
                    CREATE TABLE [dbo].[DiamondA]
                    (
                        [Id] INT NOT NULL,
                        CONSTRAINT [PK_DiamondA] PRIMARY KEY ([Id])
                    );
                    CREATE TABLE [dbo].[DiamondB]
                    (
                        [Id] INT NOT NULL,
                        [AId] INT NOT NULL,
                        CONSTRAINT [PK_DiamondB] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_DiamondB_DiamondA] FOREIGN KEY ([AId]) REFERENCES [dbo].[DiamondA] ([Id])
                    );
                    CREATE TABLE [dbo].[DiamondC]
                    (
                        [Id] INT NOT NULL,
                        [AId] INT NOT NULL,
                        CONSTRAINT [PK_DiamondC] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_DiamondC_DiamondA] FOREIGN KEY ([AId]) REFERENCES [dbo].[DiamondA] ([Id])
                    );
                    CREATE TABLE [dbo].[DiamondD]
                    (
                        [Id] INT NOT NULL,
                        [BId] INT NOT NULL,
                        [CId] INT NOT NULL,
                        CONSTRAINT [PK_DiamondD] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_DiamondD_DiamondB] FOREIGN KEY ([BId]) REFERENCES [dbo].[DiamondB] ([Id]),
                        CONSTRAINT [FK_DiamondD_DiamondC] FOREIGN KEY ([CId]) REFERENCES [dbo].[DiamondC] ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateTransferTables(SqlConnection connection)
        {
            // A table with 2 foreign keys to the same parent table
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Account'))
                BEGIN
                    CREATE TABLE [dbo].[Account]
                    (
                        [Id] INT NOT NULL,
                        CONSTRAINT [PK_Account] PRIMARY KEY ([Id])
                    );
                    CREATE TABLE [dbo].[Transfer]
                    (
                        [Id] INT NOT NULL,
                        [FromAccountId] INT NOT NULL,
                        [ToAccountId] INT NOT NULL,
                        CONSTRAINT [PK_Transfer] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Transfer_FromAccount] FOREIGN KEY ([FromAccountId]) REFERENCES [dbo].[Account] ([Id]),
                        CONSTRAINT [FK_Transfer_ToAccount] FOREIGN KEY ([ToAccountId]) REFERENCES [dbo].[Account] ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateRingTables(SqlConnection connection)
        {
            // A cycle of 3 tables: X -> Z -> Y -> X
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'RingX'))
                BEGIN
                    CREATE TABLE [dbo].[RingX]
                    (
                        [Id] INT NOT NULL,
                        [ZId] INT NULL,
                        CONSTRAINT [PK_RingX] PRIMARY KEY ([Id])
                    );
                    CREATE TABLE [dbo].[RingY]
                    (
                        [Id] INT NOT NULL,
                        [XId] INT NULL,
                        CONSTRAINT [PK_RingY] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_RingY_RingX] FOREIGN KEY ([XId]) REFERENCES [dbo].[RingX] ([Id])
                    );
                    CREATE TABLE [dbo].[RingZ]
                    (
                        [Id] INT NOT NULL,
                        [YId] INT NULL,
                        CONSTRAINT [PK_RingZ] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_RingZ_RingY] FOREIGN KEY ([YId]) REFERENCES [dbo].[RingY] ([Id])
                    );
                    ALTER TABLE [dbo].[RingX]
                        ADD CONSTRAINT [FK_RingX_RingZ] FOREIGN KEY ([ZId]) REFERENCES [dbo].[RingZ] ([Id]);
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateLoopTables(SqlConnection connection)
        {
            // A cycle (A <-> B) with a table that the cycle depends on (Root) and a table that depends on the cycle (Leaf)
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'LoopRoot'))
                BEGIN
                    CREATE TABLE [dbo].[LoopRoot]
                    (
                        [Id] INT NOT NULL,
                        CONSTRAINT [PK_LoopRoot] PRIMARY KEY ([Id])
                    );
                    CREATE TABLE [dbo].[LoopA]
                    (
                        [Id] INT NOT NULL,
                        [RootId] INT NOT NULL,
                        [BId] INT NULL,
                        CONSTRAINT [PK_LoopA] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_LoopA_LoopRoot] FOREIGN KEY ([RootId]) REFERENCES [dbo].[LoopRoot] ([Id])
                    );
                    CREATE TABLE [dbo].[LoopB]
                    (
                        [Id] INT NOT NULL,
                        [AId] INT NULL,
                        CONSTRAINT [PK_LoopB] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_LoopB_LoopA] FOREIGN KEY ([AId]) REFERENCES [dbo].[LoopA] ([Id])
                    );
                    ALTER TABLE [dbo].[LoopA]
                        ADD CONSTRAINT [FK_LoopA_LoopB] FOREIGN KEY ([BId]) REFERENCES [dbo].[LoopB] ([Id]);
                    CREATE TABLE [dbo].[LoopLeaf]
                    (
                        [Id] INT NOT NULL,
                        [BId] INT NOT NULL,
                        CONSTRAINT [PK_LoopLeaf] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_LoopLeaf_LoopB] FOREIGN KEY ([BId]) REFERENCES [dbo].[LoopB] ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateFanTables(SqlConnection connection)
        {
            // A table with many children
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'FanRoot'))
                BEGIN
                    CREATE TABLE [dbo].[FanRoot]
                    (
                        [Id] INT NOT NULL,
                        CONSTRAINT [PK_FanRoot] PRIMARY KEY ([Id])
                    );
                    CREATE TABLE [dbo].[FanChild1] ([Id] INT NOT NULL, [RootId] INT NOT NULL, CONSTRAINT [PK_FanChild1] PRIMARY KEY ([Id]), CONSTRAINT [FK_FanChild1_FanRoot] FOREIGN KEY ([RootId]) REFERENCES [dbo].[FanRoot] ([Id]));
                    CREATE TABLE [dbo].[FanChild2] ([Id] INT NOT NULL, [RootId] INT NOT NULL, CONSTRAINT [PK_FanChild2] PRIMARY KEY ([Id]), CONSTRAINT [FK_FanChild2_FanRoot] FOREIGN KEY ([RootId]) REFERENCES [dbo].[FanRoot] ([Id]));
                    CREATE TABLE [dbo].[FanChild3] ([Id] INT NOT NULL, [RootId] INT NOT NULL, CONSTRAINT [PK_FanChild3] PRIMARY KEY ([Id]), CONSTRAINT [FK_FanChild3_FanRoot] FOREIGN KEY ([RootId]) REFERENCES [dbo].[FanRoot] ([Id]));
                    CREATE TABLE [dbo].[FanChild4] ([Id] INT NOT NULL, [RootId] INT NOT NULL, CONSTRAINT [PK_FanChild4] PRIMARY KEY ([Id]), CONSTRAINT [FK_FanChild4_FanRoot] FOREIGN KEY ([RootId]) REFERENCES [dbo].[FanRoot] ([Id]));
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateItemTables(SqlConnection connection)
        {
            // 2 tables with the same name in different schemas, and a table that references the one in the non-default schema
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Item' AND SCHEMA_NAME(schema_id) = 'dbo'))
                BEGIN
                    CREATE TABLE [dbo].[Item]
                    (
                        [Id] INT NOT NULL,
                        CONSTRAINT [PK_dbo_Item] PRIMARY KEY ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);

            commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Item' AND SCHEMA_NAME(schema_id) = 'Sales'))
                BEGIN
                    CREATE TABLE [Sales].[Item]
                    (
                        [Id] INT NOT NULL,
                        CONSTRAINT [PK_Sales_Item] PRIMARY KEY ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);

            commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'ItemRef'))
                BEGIN
                    CREATE TABLE [dbo].[ItemRef]
                    (
                        [Id] INT NOT NULL,
                        [ItemId] INT NOT NULL,
                        CONSTRAINT [PK_ItemRef] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ItemRef_Item] FOREIGN KEY ([ItemId]) REFERENCES [Sales].[Item] ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateOddNameTables(SqlConnection connection)
        {
            // Tables whose names contain a dot, a space and a closing bracket
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Odd.Name'))
                BEGIN
                    CREATE TABLE [dbo].[Odd.Name]
                    (
                        [Id] INT NOT NULL,
                        [Value] NVARCHAR(50) NULL,
                        CONSTRAINT [PK_OddName] PRIMARY KEY ([Id])
                    );
                    CREATE INDEX [IX_OddName_Value] ON [dbo].[Odd.Name] ([Value]);
                    CREATE TABLE [dbo].[Odd.Child]
                    (
                        [Id] INT NOT NULL,
                        [ParentId] INT NOT NULL,
                        CONSTRAINT [PK_OddChild] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_OddChild_OddName] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Odd.Name] ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);

            commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Order Details'))
                BEGIN
                    CREATE TABLE [dbo].[Order Details]
                    (
                        [Id] INT NOT NULL,
                        [Unit Price] DECIMAL(10, 2) NOT NULL,
                        CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);

            commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Weird]Name'))
                BEGIN
                    CREATE TABLE [dbo].[Weird]]Name]
                    (
                        [Id] INT NOT NULL,
                        CONSTRAINT [PK_WeirdName] PRIMARY KEY ([Id])
                    );
                END";
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateTargetSchemas()
        {
            using (var connection = new SqlConnection(ConnectionStringForTarget).EnsureOpen())
            {
                connection.ExecuteNonQuery(@"IF (SCHEMA_ID(N'Sales') IS NULL)
                    BEGIN
                        EXEC(N'CREATE SCHEMA [Sales]');
                    END");
            }
        }

        #endregion
    }
}
