using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Models;

namespace RepoDb.SqlServer.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used for SQL Server database.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Master connection
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_SQLSERVER_CONSTR_MASTER")
                ?? Environment.GetEnvironmentVariable("REPODB_CONSTR_MASTER")
                ?? @"Server=tcp:127.0.0.1,41433;Database=master;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;" // Docker Test Configuration
                ?? @"Server=(local);Database=master;Integrated Security=SSPI;TrustServerCertificate=True;";

            // RepoDb connection
            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_SQLSERVER_CONSTR_REPODBTEST")
                ?? Environment.GetEnvironmentVariable("REPODB_CONSTR")
                ?? @"Server=tcp:127.0.0.1,41433;Database=RepoDbTest;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;" // Docker Test Configuration
                ?? @"Server=(local);Database=RepoDbTest;Integrated Security=SSPI;TrustServerCertificate=True;";

            // Initialize the SqlServer
            GlobalConfiguration
                .Setup()
                .UseSqlServer();

            // Set the DateTime type
            TypeMapper.Add(typeof(DateTime), DbType.DateTime2, true);

            // Create databases
            CreateDatabase();

            // Create tables
            CreateTables();
        }

        public static void Cleanup()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Truncate<CompleteTable>();
                connection.Truncate<NonIdentityCompleteTable>();
            }
        }

        #endregion

        #region CreateDatabases

        private static void CreateDatabase()
        {
            var commandText = @"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RepoDbTest'))
                BEGIN
	                CREATE DATABASE [RepoDbTest];
                END";
            using (var connection = new SqlConnection(ConnectionStringForMaster).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        #endregion

        #region CreateTables

        private static void CreateTables()
        {
            CreateCompleteTable();
            CreateNonIdentityCompleteTable();
        }

        private static void CreateCompleteTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'CompleteTable'))
                BEGIN
	                CREATE TABLE [dbo].[CompleteTable]
	                (
                        [Id] INT IDENTITY(1, 1),
		                [SessionId] UNIQUEIDENTIFIER NOT NULL,
		                [ColumnBigInt] BIGINT NULL,
		                [ColumnBinary] BINARY(4000) NULL,
		                [ColumnBit] BIT NULL,
		                [ColumnChar] CHAR(1) NULL,
		                [ColumnDate] DATE NULL,
		                [ColumnDateTime] DATETIME NULL,
		                [ColumnDateTime2] DATETIME2(7) NULL,
		                [ColumnDateTimeOffset] DATETIMEOFFSET(7) NULL,
		                [ColumnDecimal] DECIMAL(18, 2) NULL,
		                [ColumnFloat] FLOAT NULL,
		                [ColumnGeography] GEOGRAPHY NULL,
		                [ColumnGeometry] GEOMETRY NULL,
		                [ColumnHierarchyId] HIERARCHYID NULL,
		                [ColumnImage] IMAGE NULL,
		                [ColumnInt] INT NULL,
		                [ColumnMoney] MONEY NULL,
		                [ColumnNChar] NCHAR(1) NULL,
		                [ColumnNText] NTEXT NULL,
		                [ColumnNumeric] NUMERIC(18, 2) NULL,
		                [ColumnNVarChar] NVARCHAR(MAX) NULL,
		                [ColumnReal] REAL NULL,
		                [ColumnSmallDateTime] SMALLDATETIME NULL,
		                [ColumnSmallInt] SMALLINT NULL,
		                [ColumnSmallMoney] SMALLMONEY NULL,
		                [ColumnSqlVariant] SQL_VARIANT NULL,
		                [ColumnText] TEXT NULL,
		                [ColumnTime] TIME(7) NULL,
		                [ColumnTimestamp] TIMESTAMP NULL,
		                [ColumnTinyInt] TINYINT NULL,
		                [ColumnUniqueIdentifier] UNIQUEIDENTIFIER NULL,
		                [ColumnVarBinary] VARBINARY(MAX) NULL,
		                [ColumnVarChar] VARCHAR(MAX) NULL,
		                [ColumnXml] XML NULL,
		                CONSTRAINT [CompleteTable_Id] PRIMARY KEY 
		                (
			                [Id] ASC
		                )
	                ) ON [PRIMARY];
                END";
            using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        private static void CreateNonIdentityCompleteTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'NonIdentityCompleteTable'))
                BEGIN
	                CREATE TABLE [dbo].[NonIdentityCompleteTable]
	                (
                        [Id] INT NOT NULL,
		                [SessionId] UNIQUEIDENTIFIER NOT NULL,
		                [ColumnBigInt] BIGINT NULL,
		                [ColumnBinary] BINARY(4000) NULL,
		                [ColumnBit] BIT NULL,
		                [ColumnChar] CHAR(1) NULL,
		                [ColumnDate] DATE NULL,
		                [ColumnDateTime] DATETIME NULL,
		                [ColumnDateTime2] DATETIME2(7) NULL,
		                [ColumnDateTimeOffset] DATETIMEOFFSET(7) NULL,
		                [ColumnDecimal] DECIMAL(18, 2) NULL,
		                [ColumnFloat] FLOAT NULL,
		                [ColumnGeography] GEOGRAPHY NULL,
		                [ColumnGeometry] GEOMETRY NULL,
		                [ColumnHierarchyId] HIERARCHYID NULL,
		                [ColumnImage] IMAGE NULL,
		                [ColumnInt] INT NULL,
		                [ColumnMoney] MONEY NULL,
		                [ColumnNChar] NCHAR(1) NULL,
		                [ColumnNText] NTEXT NULL,
		                [ColumnNumeric] NUMERIC(18, 2) NULL,
		                [ColumnNVarChar] NVARCHAR(MAX) NULL,
		                [ColumnReal] REAL NULL,
		                [ColumnSmallDateTime] SMALLDATETIME NULL,
		                [ColumnSmallInt] SMALLINT NULL,
		                [ColumnSmallMoney] SMALLMONEY NULL,
		                [ColumnSqlVariant] SQL_VARIANT NULL,
		                [ColumnText] TEXT NULL,
		                [ColumnTime] TIME(7) NULL,
		                [ColumnTimestamp] TIMESTAMP NULL,
		                [ColumnTinyInt] TINYINT NULL,
		                [ColumnUniqueIdentifier] UNIQUEIDENTIFIER NULL,
		                [ColumnVarBinary] VARBINARY(MAX) NULL,
		                [ColumnVarChar] VARCHAR(MAX) NULL,
		                [ColumnXml] XML NULL,
		                CONSTRAINT [NonIdentityCompleteTable_Id] PRIMARY KEY 
		                (
			                [Id] ASC
		                )
	                ) ON [PRIMARY];
                END";
            using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var tables = Helper.CreateCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion

        #region NonIdentityCompleteTable

        public static IEnumerable<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var tables = Helper.CreateNonIdentityCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion
    }
}
