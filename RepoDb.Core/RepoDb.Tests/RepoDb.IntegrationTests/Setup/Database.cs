using System;
using System.Data.SqlClient;

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
            // Check the connection string
            var environment = Environment.GetEnvironmentVariable("REPODB_ENVIRONMENT", EnvironmentVariableTarget.User);

            // Master connection
            ConnectionForMaster = (environment == "DEVELOPMENT") ?
                @"Server=(local);Database=master;Integrated Security=True;" :
                @"Server=(local)\SQL2017;Database=master;User ID=sa;Password=Password12!;Persist Security Info=True;";

            // RepoDb connection
            ConnectionStringForRepoDb = (environment == "DEVELOPMENT") ?
                @"Server=(local);Database=RepoDb;Integrated Security=True;" :
                @"Server=(local)\SQL2017;Database=RepoDb;User ID=sa;Password=Password12!;Persist Security Info=True;";

            // Set the proper values for type mapper
            TypeMapper.Map(typeof(DateTime), System.Data.DbType.DateTime2, true);

            // Create the database first
            CreateDatabase();

            // Create the tables
            CreateTables();

            // Create the stored procedures
            CreateStoredProcedures();
        }

        /// <summary>
        /// Gets the connection string for master.
        /// </summary>
        public static string ConnectionForMaster { get; private set; }

        /// <summary>
        /// Gets the connection string for RepoDb.
        /// </summary>
        public static string ConnectionStringForRepoDb { get; private set; }

        /// <summary>
        /// Creates a test database for RepoDb.
        /// </summary>
        public static void CreateDatabase()
        {
            var commandText = @"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RepoDb'))
                BEGIN
	                CREATE DATABASE [RepoDb];
                END";
            using (var connection = new SqlConnection(ConnectionForMaster).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        /// <summary>
        /// Create the necessary tables for testing.
        /// </summary>
        public static void CreateTables()
        {
            CreateCompleteTable();
            CreateSimpleTable();
        }

        /// <summary>
        /// Create the necessary stored procedure for testing.
        /// </summary>
        public static void CreateStoredProcedures()
        {
            CreateGetSimpleTablesStoredProcedure();
            CreateGetSimpleTableByIdStoredProcedure();
            CreateMultiplyStoredProcedure();
            CreateGetDatabaseDateTimeStoredProcedure();
        }

        /// <summary>
        /// Clean up the target table.
        /// </summary>
        public static void Cleanup()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb))
            {
                var commandText = "DELETE FROM [dbo].[CompleteTable]; DELETE FROM [dbo].[SimpleTable];";
                connection.ExecuteNonQuery(commandText);
            }
        }

        #region CreateTables

        /// <summary>
        /// Creates a simple table that has some important fields. All fields are nullable.
        /// </summary>
        public static void CreateSimpleTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'SimpleTable'))
                BEGIN
	                CREATE TABLE [dbo].[SimpleTable]
	                (
		                [Id] BIGINT NOT NULL IDENTITY(1, 1),
		                [ColumnBit] BIT NULL,
		                [ColumnDateTime] DATETIME NULL,
		                [ColumnDateTime2] DATETIME2(7) NULL,
		                [ColumnDecimal] DECIMAL(18, 2) NULL,
		                [ColumnFloat] FLOAT NULL,
		                [ColumnInt] INT NULL,
		                [ColumnNVarChar] NVARCHAR(MAX) NULL,
		                CONSTRAINT [Id] PRIMARY KEY 
		                (
			                [Id] ASC
		                )
	                ) ON [PRIMARY];
                END";
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        /// <summary>
        /// Creates a table that has a complete fields. All fields are nullable.
        /// </summary>
        public static void CreateCompleteTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'CompleteTable'))
                BEGIN
	                CREATE TABLE [dbo].[CompleteTable]
	                (
		                [SessionId] UNIQUEIDENTIFIER NOT NULL,
		                [ColumnBigInt] BIGINT NULL,
		                [ColumnBinary] BINARY(4000) NULL,
		                [ColumnBit] BIT NULL,
		                [ColumnChar] CHAR(32) NULL,
		                [ColumnDate] DATE NULL,
		                [ColumnDateTime] DATETIME NULL,
		                [ColumnDateTime2] DATETIME2(7) NULL,
		                [ColumnDateTimeOffset] DATETIMEOFFSET(7) NULL,
		                [ColumnDecimal] DECIMAL(18, 2) NULL,
		                [ColumnFloat] FLOAT NULL,
		                [ColumnGeography] GEOGRAPHY NULL,
		                [ColumnGeometry] GEOMETRY NULL,
		                [ColumnHierarchyid] HIERARCHYID NULL,
		                [ColumnImage] IMAGE NULL,
		                [ColumnInt] INT NULL,
		                [ColumnMoney] MONEY NULL,
		                [ColumnNChar] NCHAR(32) NULL,
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
		                [ColumnVarchar] VARCHAR(MAX) NULL,
		                [ColumnXml] XML NULL,
		                CONSTRAINT [SessionId] PRIMARY KEY 
		                (
			                [SessionId] ASC
		                )
	                ) ON [PRIMARY];
	                ALTER TABLE [dbo].[CompleteTable] ADD CONSTRAINT [DF_CompleteTable_SessionId] DEFAULT (NEWID()) FOR [SessionId];
                END";
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        #endregion

        #region CreateStoredProcedures

        /// <summary>
        /// Create a stored procedure that is used to return all records from SimpleTable.
        /// </summary>
        public static void CreateGetSimpleTablesStoredProcedure()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[objects] WHERE type = 'P' AND name = 'sp_get_simple_tables';");
                if (exists == null)
                {
                    var commandText = @"CREATE PROCEDURE [dbo].[sp_get_simple_tables]
	                    AS
                        BEGIN
                            SELECT * FROM [dbo].[SimpleTable];
                        END";
                    connection.ExecuteNonQuery(commandText);
                }
            }
        }

        /// <summary>
        /// Create a stored procedure that is used to return a SimpleTable record by id.
        /// </summary>
        public static void CreateGetSimpleTableByIdStoredProcedure()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[objects] WHERE type = 'P' AND name = 'sp_get_simple_table_by_id';");
                if (exists == null)
                {
                    var commandText = @"CREATE PROCEDURE [dbo].[sp_get_simple_table_by_id]
                        (
                            @Id INT
                        )
	                    AS
                        BEGIN
                            SELECT * FROM [dbo].[SimpleTable] WHERE Id = @Id;
                        END";
                    connection.ExecuteNonQuery(commandText);
                }
            }
        }

        /// <summary>
        /// Create a stored procedure that will return a scalar value of date time.
        /// </summary>
        public static void CreateGetDatabaseDateTimeStoredProcedure()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[objects] WHERE type = 'P' AND name = 'sp_get_database_date_time';");
                if (exists == null)
                {
                    var commandText = @"CREATE PROCEDURE [dbo].[sp_get_database_date_time]
	                    AS
                        BEGIN
                            SELECT GETUTCDATE() AS [Value];
                        END";
                    connection.ExecuteNonQuery(commandText);
                }
            }
        }

        /// <summary>
        /// Create a stored procedure that is used for multiplication.
        /// </summary>
        public static void CreateMultiplyStoredProcedure()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[objects] WHERE type = 'P' AND name = 'sp_multiply';");
                if (exists == null)
                {
                    var commandText = @"CREATE PROCEDURE [dbo].[sp_multiply]
                        (
                            @Value1 INT,
                            @Value2 INT
                        )
	                    AS
                        BEGIN
                            SELECT @Value1 * @Value2 AS [Value];
                        END";
                    connection.ExecuteNonQuery(commandText);
                }
            }
        }
        
        #endregion
    }
}
