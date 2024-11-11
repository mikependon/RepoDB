using System;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// A class used as a startup setup for for RepoDb test database.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Initialize the creation of the database.
        /// </summary>
        public static void Initialize()
        {
            // Master connection
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_SQLSERVER_CONSTR_MASTER")
                ?? Environment.GetEnvironmentVariable("REPODB_CONSTR_MASTER")
                // ?? @"Server=tcp:127.0.0.1,41433;Database=master;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;" // Docker Test Configuration
                ?? @"Server=(local);Database=master;Integrated Security=SSPI;TrustServerCertificate=True;";

            // RepoDb connection
            ConnectionStringForRepoDb =
                Environment.GetEnvironmentVariable("REPODB_SQLSERVER_CONSTR_REPODBTEST")
                ?? Environment.GetEnvironmentVariable("REPODB_CONSTR")
                // ?? @"Server=tcp:127.0.0.1,41433;Database=RepoDbTest;User ID=sa;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;TrustServerCertificate=True;" // Docker Test Configuration
                ?? @"Server=(local);Database=RepoDbTest;Integrated Security=SSPI;TrustServerCertificate=True;";

            // Initialize the SqlServer
            GlobalConfiguration
                .Setup()
                .UseSqlServer();

            // Create the database first
            CreateDatabase();

            // Create the tables
            CreateTables();
        }

        /// <summary>
        /// Gets the connection string for master.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets the connection string for RepoDb.
        /// </summary>
        public static string ConnectionStringForRepoDb { get; private set; }

        #region Methods

        /// <summary>
        /// Creates a test database for RepoDb.
        /// </summary>
        public static void CreateDatabase()
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

        /// <summary>
        /// Create the necessary tables for testing.
        /// </summary>
        public static void CreateTables()
        {
            CreateBulkOperationIdentityTable();
        }

        /// <summary>
        /// Clean up all the table.
        /// </summary>
        public static void Cleanup()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb))
            {
                connection.Truncate<BulkOperationIdentityTable>();
            }
        }

        #endregion

        #region CreateTables

        /// <summary>
        /// Creates an identity table that has some important fields. All fields are nullable.
        /// </summary>
        public static void CreateBulkOperationIdentityTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'BulkOperationIdentityTable'))
                BEGIN
	                CREATE TABLE [dbo].[BulkOperationIdentityTable]
	                (
		                [Id] BIGINT NOT NULL IDENTITY(1, 1),
                        [RowGuid] UNIQUEIDENTIFIER NOT NULL,
		                [ColumnBit] BIT NULL,
		                [ColumnDateTime] DATETIME NULL,
		                [ColumnDateTime2] DATETIME2(7) NULL,
		                [ColumnDecimal] DECIMAL(18, 2) NULL,
		                [ColumnFloat] FLOAT NULL,
		                [ColumnInt] INT NULL,
		                [ColumnNVarChar] NVARCHAR(MAX) NULL,
                        CONSTRAINT [PK_BulkOperationIdentityTable] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )
                        WITH (FILLFACTOR = 90) ON [PRIMARY]
	                ) ON [PRIMARY];
                END";
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        #endregion
    }
}
