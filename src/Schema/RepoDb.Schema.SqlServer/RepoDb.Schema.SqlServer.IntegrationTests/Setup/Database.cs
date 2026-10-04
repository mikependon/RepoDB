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
            // Drop the tables that were created in the target by the tests (the referencing tables are dropped first)
            using (var connection = new SqlConnection(ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(@"DROP TABLE IF EXISTS [dbo].[Person];
                    DROP TABLE IF EXISTS [dbo].[Country];
                    DROP TABLE IF EXISTS [dbo].[OrderLine];
                    DROP TABLE IF EXISTS [dbo].[NoKey];
                    DROP TABLE IF EXISTS [Sales].[Invoice];");
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
