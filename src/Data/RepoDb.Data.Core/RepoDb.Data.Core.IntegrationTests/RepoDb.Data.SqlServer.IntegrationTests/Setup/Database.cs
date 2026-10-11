#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.Data.SqlClient;
using RepoDb.DbSettings;
using RepoDb.Schema.SqlServer;

namespace RepoDb.Data.SqlServer.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used for the SQL Server master database.
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
            // Master connection
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_SQLSVR_DATA_CONSTR_MASTER") ??
                @"Server=tcp:127.0.0.1,1433;Database=master;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;";

            // Source connection
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_SQLSVR_DATA_CONSTR_SOURCE") ??
                @"Server=tcp:127.0.0.1,1433;Database=RepoDb_Data_Source;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;";

            // Target connection
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_SQLSVR_DATA_CONSTR_TARGET") ??
                @"Server=tcp:127.0.0.1,1433;Database=RepoDb_Data_Target;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;";

            // Initialize the SqlServer
            GlobalConfiguration
                .Setup()
                .UseSqlServer(new SqlServerBulkOperationsDbSetting())
                .UseSqlServerSchema();

            // Create databases
            CreateDatabases();

            // Create the source tables (with their rows) and the target schemas
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

            // The fields of the tables are cached (by the connection string and the name of the table), and the tables are not the same in the next test
            DbFieldCache.Flush();
        }

        #endregion

        #region CreateDatabases

        private static void CreateDatabases()
        {
            using (var connection = new SqlConnection(ConnectionStringForMaster).EnsureOpen())
            {
                connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RepoDb_Data_Source'))
                    BEGIN
                        CREATE DATABASE [RepoDb_Data_Source];
                    END");
                connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RepoDb_Data_Target'))
                    BEGIN
                        CREATE DATABASE [RepoDb_Data_Target];
                    END");
            }
        }

        #endregion

        #region CreateTables

        private static void CreateSourceTables()
        {
            using (var connection = new SqlConnection(ConnectionStringForSource))
            {
                CreateSalesSchema(connection);
                CreateCountryTable(connection);
                CreatePersonTable(connection);
                CreatePetTable(connection);
                CreateToyTable(connection);
                CreateCurrencyTable(connection);
                CreateSampleTable(connection);
                CreateOrderDetailsTable(connection);
                CreateInvoiceTables(connection);
                CreateNumberTable(connection);
                CreateEmployeeTable(connection);
                CreateCycleTables(connection);
                CreateNoKeyTable(connection);
                CreateEmptyTable(connection);
            }
        }

        private static void CreateSalesSchema(SqlConnection connection)
        {
            connection.ExecuteNonQuery(@"IF (SCHEMA_ID(N'Sales') IS NULL)
                BEGIN
                    EXEC(N'CREATE SCHEMA [Sales]');
                END");
        }

        private static void CreateCountryTable(SqlConnection connection)
        {
            // The root of the tree: Country <- Person <- Pet <- Toy
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Country'))
                BEGIN
                    CREATE TABLE [dbo].[Country]
                    (
                        [Id] INT IDENTITY(1, 1) NOT NULL,
                        [Name] NVARCHAR(100) NOT NULL,
                        CONSTRAINT [PK_Country] PRIMARY KEY ([Id]),
                        CONSTRAINT [UQ_Country_Name] UNIQUE ([Name])
                    );
                    INSERT INTO [dbo].[Country] ([Name]) VALUES (N'Philippines'), (N'Japan'), (N'Germany');
                END");
        }

        private static void CreatePersonTable(SqlConnection connection)
        {
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Person'))
                BEGIN
                    CREATE TABLE [dbo].[Person]
                    (
                        [Id] INT IDENTITY(1, 1) NOT NULL,
                        [Name] NVARCHAR(128) NOT NULL,
                        [Age] INT NULL,
                        [CountryId] INT NULL,
                        [Salary] DECIMAL(18, 2) NULL,
                        [Birthday] DATE NULL,
                        [IsActive] BIT NOT NULL CONSTRAINT [DF_Person_IsActive] DEFAULT ((1)),
                        [CreatedDateUtc] DATETIME2(3) NOT NULL CONSTRAINT [DF_Person_CreatedDateUtc] DEFAULT (SYSUTCDATETIME()),
                        CONSTRAINT [PK_Person] PRIMARY KEY ([Id]),
                        CONSTRAINT [CK_Person_Age] CHECK ([Age] >= 0),
                        CONSTRAINT [FK_Person_Country] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country] ([Id])
                    );
                    CREATE INDEX [IX_Person_Name] ON [dbo].[Person] ([Name]);
                    INSERT INTO [dbo].[Person] ([Name], [Age], [CountryId], [Salary], [Birthday], [IsActive], [CreatedDateUtc]) VALUES
                        (N'Alice', 30, 1, 1000.50, '1990-01-15', 1, '2026-01-01T10:00:00.123'),
                        (N'Bob',   45, 2, 2500.00, '1975-06-30', 1, '2026-01-02T11:00:00.456'),
                        (N'Carol', NULL, NULL, NULL, NULL, 0, '2026-01-03T12:00:00.789'),
                        (N'Dave',  22, 1, 800.25,  '2003-12-01', 1, '2026-01-04T13:00:00.000'),
                        (N'Eve',   37, 3, 3100.75, '1988-03-09', 0, '2026-01-05T14:00:00.321'),
                        (N'Frank', 52, 2, NULL,    '1970-11-22', 1, '2026-01-06T15:00:00.654');
                END");
        }

        private static void CreatePetTable(SqlConnection connection)
        {
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Pet'))
                BEGIN
                    CREATE TABLE [dbo].[Pet]
                    (
                        [Id] INT IDENTITY(1, 1) NOT NULL,
                        [Name] NVARCHAR(50) NOT NULL,
                        [PersonId] INT NOT NULL,
                        CONSTRAINT [PK_Pet] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Pet_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([Id])
                    );
                    INSERT INTO [dbo].[Pet] ([Name], [PersonId]) VALUES (N'Rex', 1), (N'Tom', 1), (N'Kiko', 2), (N'Luna', 4), (N'Milo', 5);
                END");
        }

        private static void CreateToyTable(SqlConnection connection)
        {
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Toy'))
                BEGIN
                    CREATE TABLE [dbo].[Toy]
                    (
                        [Id] INT IDENTITY(1, 1) NOT NULL,
                        [Name] NVARCHAR(50) NOT NULL,
                        [PetId] INT NOT NULL,
                        CONSTRAINT [PK_Toy] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Toy_Pet] FOREIGN KEY ([PetId]) REFERENCES [dbo].[Pet] ([Id])
                    );
                    INSERT INTO [dbo].[Toy] ([Name], [PetId]) VALUES (N'Ball', 1), (N'Bone', 1), (N'Mouse', 2), (N'Yarn', 3), (N'Stick', 4);
                END");
        }

        private static void CreateCurrencyTable(SqlConnection connection)
        {
            // A table that is not related to any other table
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Currency'))
                BEGIN
                    CREATE TABLE [dbo].[Currency]
                    (
                        [Code] CHAR(3) NOT NULL,
                        [Name] NVARCHAR(50) NOT NULL,
                        CONSTRAINT [PK_Currency] PRIMARY KEY ([Code])
                    );
                    INSERT INTO [dbo].[Currency] ([Code], [Name]) VALUES ('PHP', N'Philippine Peso'), ('JPY', N'Japanese Yen'), ('EUR', N'Euro');
                END");
        }

        private static void CreateSampleTable(SqlConnection connection)
        {
            // One column of each of the common data types, and a row of NULL values
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Sample'))
                BEGIN
                    CREATE TABLE [dbo].[Sample]
                    (
                        [Id] INT NOT NULL,
                        [BigIntValue] BIGINT NULL,
                        [BitValue] BIT NULL,
                        [DecimalValue] DECIMAL(18, 4) NULL,
                        [FloatValue] FLOAT NULL,
                        [DateTime2Value] DATETIME2(7) NULL,
                        [DateValue] DATE NULL,
                        [GuidValue] UNIQUEIDENTIFIER NULL,
                        [BinaryValue] VARBINARY(MAX) NULL,
                        [TextValue] NVARCHAR(MAX) NULL,
                        [CharValue] NVARCHAR(50) NULL,
                        CONSTRAINT [PK_Sample] PRIMARY KEY ([Id])
                    );
                    INSERT INTO [dbo].[Sample] ([Id], [BigIntValue], [BitValue], [DecimalValue], [FloatValue], [DateTime2Value], [DateValue], [GuidValue], [BinaryValue], [TextValue], [CharValue]) VALUES
                        (1, 9223372036854775807, 1, 12345.6789, 3.14159265358979, '2026-02-03T04:05:06.1234567', '2026-02-03', '6F9619FF-8B86-D011-B42D-00C04FC964FF', 0x0102030405, N'A long text with unicode: ñ é 日本語', N'abc'),
                        (2, -9223372036854775808, 0, -0.0001, -2.5E-10, '0001-01-01T00:00:00', '9999-12-31', '00000000-0000-0000-0000-000000000000', 0x, N'', N''),
                        (3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
                END");
        }

        private static void CreateOrderDetailsTable(SqlConnection connection)
        {
            // A table whose name and column have a space
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Order Details'))
                BEGIN
                    CREATE TABLE [dbo].[Order Details]
                    (
                        [Id] INT NOT NULL,
                        [Unit Price] DECIMAL(10, 2) NOT NULL,
                        CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([Id])
                    );
                    INSERT INTO [dbo].[Order Details] ([Id], [Unit Price]) VALUES (1, 9.99), (2, 19.50);
                END");
        }

        private static void CreateInvoiceTables(SqlConnection connection)
        {
            // Tables of a schema that is not the default one, and a table of the default schema that references it
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Invoice'))
                BEGIN
                    CREATE TABLE [Sales].[Invoice]
                    (
                        [Id] INT NOT NULL,
                        [Total] DECIMAL(19, 4) NOT NULL,
                        CONSTRAINT [PK_Invoice] PRIMARY KEY ([Id])
                    );
                    CREATE TABLE [Sales].[InvoiceLine]
                    (
                        [Id] INT NOT NULL,
                        [InvoiceId] INT NOT NULL,
                        [Description] NVARCHAR(100) NOT NULL,
                        CONSTRAINT [PK_InvoiceLine] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_InvoiceLine_Invoice] FOREIGN KEY ([InvoiceId]) REFERENCES [Sales].[Invoice] ([Id])
                    );
                    CREATE TABLE [dbo].[Ledger]
                    (
                        [Id] INT NOT NULL,
                        [InvoiceId] INT NOT NULL,
                        CONSTRAINT [PK_Ledger] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Ledger_Invoice] FOREIGN KEY ([InvoiceId]) REFERENCES [Sales].[Invoice] ([Id])
                    );
                    INSERT INTO [Sales].[Invoice] ([Id], [Total]) VALUES (1, 100.0000), (2, 250.5000);
                    INSERT INTO [Sales].[InvoiceLine] ([Id], [InvoiceId], [Description]) VALUES (1, 1, N'Line 1'), (2, 1, N'Line 2'), (3, 2, N'Line 3');
                    INSERT INTO [dbo].[Ledger] ([Id], [InvoiceId]) VALUES (1, 1), (2, 2);
                END");
        }

        private static void CreateNumberTable(SqlConnection connection)
        {
            // A table with many rows, to copy the rows in many batches
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Number'))
                BEGIN
                    CREATE TABLE [dbo].[Number]
                    (
                        [Value] INT NOT NULL,
                        [Label] NVARCHAR(20) NOT NULL,
                        CONSTRAINT [PK_Number] PRIMARY KEY ([Value])
                    );
                    WITH [Numbers] AS (SELECT TOP (2500) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [N] FROM sys.all_objects a CROSS JOIN sys.all_objects b)
                    INSERT INTO [dbo].[Number] ([Value], [Label]) SELECT [N], N'Number ' + CAST([N] AS NVARCHAR(10)) FROM [Numbers];
                END");
        }

        private static void CreateEmployeeTable(SqlConnection connection)
        {
            // A self-referencing foreign key
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Employee'))
                BEGIN
                    CREATE TABLE [dbo].[Employee]
                    (
                        [Id] INT NOT NULL,
                        [Name] NVARCHAR(50) NOT NULL,
                        [ManagerId] INT NULL,
                        CONSTRAINT [PK_Employee] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Employee_Manager] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[Employee] ([Id])
                    );
                    INSERT INTO [dbo].[Employee] ([Id], [Name], [ManagerId]) VALUES (1, N'CEO', NULL), (2, N'VP', 1), (3, N'Developer', 2);
                END");
        }

        private static void CreateCycleTables(SqlConnection connection)
        {
            // A cycle of 2 tables
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'CycleA'))
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
                    INSERT INTO [dbo].[CycleA] ([Id], [BId]) VALUES (1, NULL);
                    INSERT INTO [dbo].[CycleB] ([Id], [AId]) VALUES (1, 1);
                    UPDATE [dbo].[CycleA] SET [BId] = 1 WHERE [Id] = 1;
                END");
        }

        private static void CreateNoKeyTable(SqlConnection connection)
        {
            // A table without a primary key
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'NoKey'))
                BEGIN
                    CREATE TABLE [dbo].[NoKey]
                    (
                        [Value] NVARCHAR(50) NULL,
                        [Amount] INT NULL
                    );
                    INSERT INTO [dbo].[NoKey] ([Value], [Amount]) VALUES (N'A', 1), (N'A', 1), (NULL, NULL);
                END");
        }

        private static void CreateEmptyTable(SqlConnection connection)
        {
            // A table without rows
            connection.ExecuteNonQuery(@"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Empty'))
                BEGIN
                    CREATE TABLE [dbo].[Empty]
                    (
                        [Id] INT NOT NULL,
                        [Name] NVARCHAR(50) NULL,
                        CONSTRAINT [PK_Empty] PRIMARY KEY ([Id])
                    );
                END");
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
