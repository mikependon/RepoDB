using System;
using Microsoft.Data.SqlClient;

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
            // Get the connection string
            var connectionStringForMaster = Environment.GetEnvironmentVariable("REPODB_CONSTR_MASTER", EnvironmentVariableTarget.Process);
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            // Master connection
            ConnectionStringForMaster = (connectionStringForMaster ?? @"Server=(local);Database=master;Integrated Security=False;User Id=michael;Password=Password123;");

            // RepoDb connection
            ConnectionStringForRepoDb = (connectionString ?? @"Server=(local);Database=RepoDb;Integrated Security=False;User Id=michael;Password=Password123;");

            // Set the proper values for type mapper
            TypeMapper.Add(typeof(DateTime), System.Data.DbType.DateTime2, true);

            // Initialize the SqlServer
            SqlServerBootstrap.Initialize();

            // Create the database first
            CreateDatabase();

            // Create the schemas
            CreateSchemas();

            // Create the types
            CreateTypes();

            // Create the tables
            CreateTables();

            // Create the stored procedures
            CreateStoredProcedures();
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
        private static void CreateDatabase()
        {
            var commandText = @"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RepoDb'))
                BEGIN
	                CREATE DATABASE [RepoDb];
                END";
            using (var connection = new SqlConnection(ConnectionStringForMaster).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        /// <summary>
        /// Create the necessary schemas for testing.
        /// </summary>
        private static void CreateSchemas()
        {
            CreateScSchema();
        }

        /// <summary>
        /// Create the necessary types for testing.
        /// </summary>
        private static void CreateTypes()
        {
            CreateIdentityTableType();
        }

        /// <summary>
        /// Create the necessary tables for testing.
        /// </summary>
        private static void CreateTables()
        {
            CreateCompleteTable();
            CreateIdentityTable();
            CreateNonIdentityTable();
            CreateUnorganizedTable();
            CreateDottedTable();
            CreatePropertyHandlerTable();
            CreateNonKeyedTable();
        }

        /// <summary>
        /// Create the necessary stored procedures for testing.
        /// </summary>
        private static void CreateStoredProcedures()
        {
            CreateGetIdentityTablesStoredProcedure();
            CreateGetIdentityTableByIdStoredProcedure();
            CreateMultiplyStoredProcedure();
            CreateMultiplyWithOutputStoredProcedure();
            CreateGetDatabaseDateTimeStoredProcedure();
            CreateIdentityTableTypeStoredProcedure();
        }

        /// <summary>
        /// Clean up all the table.
        /// </summary>
        public static void Cleanup()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb))
            {
                connection.Truncate("[dbo].[CompleteTable]");
                connection.Truncate("[sc].[IdentityTable]");
                connection.Truncate("[dbo].[NonIdentityTable]");
                connection.Truncate("[dbo].[Unorganized Table]");
                connection.Truncate("[dbo].[Dotted.Table]");
                connection.Truncate("[dbo].[PropertyHandler]");
                connection.Truncate("[dbo].[NonKeyedTable]");
            }
        }

        #endregion

        #region CreateSchemas

        /// <summary>
        /// Creates the 'sc' schema.
        /// </summary>
        private static void CreateScSchema()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[schemas] WHERE name = 'sc';");
                if (exists == null)
                {
                    connection.ExecuteNonQuery("CREATE SCHEMA [sc];");
                }
            }
        }

        #endregion

        #region CreateTypes

        /// <summary>
        /// Creates the 'IdentityTableType' type.
        /// </summary>
        private static void CreateIdentityTableType()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[types] WHERE name = 'IdentityTableType';");
                if (exists == null)
                {
                    connection.ExecuteNonQuery(@"CREATE TYPE IdentityTableType AS TABLE
                        (
                            [Id] BIGINT NOT NULL,
                            [RowGuid] UNIQUEIDENTIFIER NOT NULL,
		                    [ColumnBit] BIT NULL,
		                    [ColumnDateTime] DATETIME NULL,
		                    [ColumnDateTime2] DATETIME2(7) NULL,
		                    [ColumnDecimal] DECIMAL(18, 2) NULL,
		                    [ColumnFloat] FLOAT NULL,
		                    [ColumnInt] INT NULL,
		                    [ColumnNVarChar] NVARCHAR(MAX) NULL
                        );");
                }
            }
        }

        #endregion

        #region CreateTables

        /// <summary>
        /// Creates an identity table that has some important fields. All fields are nullable.
        /// </summary>
        private static void CreateIdentityTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'IdentityTable'))
                BEGIN
	                CREATE TABLE [sc].[IdentityTable]
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
	                ) ON [PRIMARY];
                END";
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        /// <summary>
        /// Creates an non-identity table that has some important fields. All fields are nullable.
        /// </summary>
        private static void CreateNonIdentityTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'NonIdentityTable'))
                BEGIN
	                CREATE TABLE [dbo].[NonIdentityTable]
	                (
		                [Id] UNIQUEIDENTIFIER NOT NULL,
		                [ColumnBit] BIT NULL,
		                [ColumnDateTime] DATETIME NULL,
		                [ColumnDateTime2] DATETIME2(7) NULL,
		                [ColumnDecimal] DECIMAL(18, 2) NULL,
		                [ColumnFloat] FLOAT NULL,
		                [ColumnInt] INT NULL,
		                [ColumnNVarChar] NVARCHAR(MAX) NULL,
		                CONSTRAINT [NonIdentityTable_$Id] PRIMARY KEY 
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
        /// Creates an unorganized table that has some non-alphanumeric fields. All fields are nullable.
        /// </summary>
        private static void CreateUnorganizedTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Unorganized Table'))
                BEGIN
	                CREATE TABLE [dbo].[Unorganized Table]
	                (
		                [Id] BIGINT NOT NULL IDENTITY(1, 1),
                        [Session Id] UNIQUEIDENTIFIER NOT NULL,
		                [Column Int] INT NULL,
		                [Column/NVarChar] NVARCHAR(128) NULL,
		                [Column.DateTime] DATETIME2(7) NULL
	                ) ON [PRIMARY];
                END";
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        /// <summary>
        /// Creates a dotted table that has some dots on the name. All fields are nullable.
        /// </summary>
        private static void CreateDottedTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Dotted.Table'))
                BEGIN
	                CREATE TABLE [dbo].[Dotted.Table]
	                (
		                [Id] BIGINT NOT NULL IDENTITY(1, 1),
                        [SessionId] UNIQUEIDENTIFIER NOT NULL,
		                [Column.Int] INT NULL,
		                [Column.NVarChar] NVARCHAR(128) NULL,
		                [Column.DateTime] DATETIME2(7) NULL
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
        private static void CreateCompleteTable()
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
		                [ColumnHierarchyId] HIERARCHYID NULL,
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
		                [ColumnVarChar] VARCHAR(MAX) NULL,
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

        /// <summary>
        /// Creates a table that has a complete fields. All fields are nullable.
        /// </summary>
        private static void CreatePropertyHandlerTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'PropertyHandler'))
                BEGIN
	                CREATE TABLE [dbo].[PropertyHandler]
	                (
		                [Id] BIGINT NOT NULL IDENTITY(1, 1),
		                [ColumnNVarChar] NVARCHAR(MAX) NULL,
		                [ColumnInt] INT NULL,
		                [ColumnIntNotNull] INT NOT NULL,
		                [ColumnDecimal] DECIMAL(18, 2) NULL,
		                [ColumnDecimalNotNull] DECIMAL(18, 2) NOT NULL,
		                [ColumnFloat] FLOAT NULL,
		                [ColumnFloatNotNull] FLOAT NOT NULL,
		                [ColumnDateTime] DATETIME NULL,
		                [ColumnDateTimeNotNull] DATETIME NOT NULL,
		                [ColumnDateTime2] DATETIME2(7) NULL,
		                [ColumnDateTime2NotNull] DATETIME2(7) NOT NULL,
                        CONSTRAINT [pk_PropertyHandler] PRIMARY KEY CLUSTERED 
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
        /// Creates a table that has no keys.
        /// </summary>
        private static void CreateNonKeyedTable()
        {
            var commandText = @"IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'NonKeyedTable'))
                BEGIN
	                CREATE TABLE [dbo].[NonKeyedTable]
	                (
		                [ColumnDateTime2] DATETIME2(7) NULL,
		                [ColumnInt] INT NULL,
		                [ColumnNVarChar] NVARCHAR(MAX) NULL,
	                ) ON [PRIMARY];
                END";
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                connection.ExecuteNonQuery(commandText);
            }
        }

        #endregion

        #region CreateStoredProcedures

        /// <summary>
        /// Create a stored procedure that is used to return all records from IdentityTable.
        /// </summary>
        private static void CreateGetIdentityTablesStoredProcedure()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[objects] WHERE type = 'P' AND name = 'sp_get_identity_tables';");
                if (exists == null)
                {
                    var commandText = @"CREATE PROCEDURE [dbo].[sp_get_identity_tables]
	                    AS
                        BEGIN
                            SELECT * FROM [sc].[IdentityTable];
                        END";
                    connection.ExecuteNonQuery(commandText);
                }
            }
        }

        /// <summary>
        /// Create a stored procedure that is used to return a IdentityTable record by id.
        /// </summary>
        private static void CreateGetIdentityTableByIdStoredProcedure()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[objects] WHERE type = 'P' AND name = 'sp_get_identity_table_by_id';");
                if (exists == null)
                {
                    var commandText = @"CREATE PROCEDURE [dbo].[sp_get_identity_table_by_id]
                        (
                            @Id INT
                        )
	                    AS
                        BEGIN
                            SELECT * FROM [sc].[IdentityTable] WHERE Id = @Id;
                        END";
                    connection.ExecuteNonQuery(commandText);
                }
            }
        }

        /// <summary>
        /// Create a stored procedure that will return a scalar value of date time.
        /// </summary>
        private static void CreateGetDatabaseDateTimeStoredProcedure()
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
        private static void CreateMultiplyStoredProcedure()
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

        /// <summary>
        /// Create a stored procedure that is used for multiplication.
        /// </summary>
        private static void CreateMultiplyWithOutputStoredProcedure()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[objects] WHERE type = 'P' AND name = 'sp_multiply_with_output';");
                if (exists == null)
                {
                    var commandText = @"CREATE PROCEDURE [dbo].[sp_multiply_with_output]
                        (
                            @Value1 INT,
                            @Value2 INT,
                            @Output INT OUTPUT
                        )
	                    AS
                        BEGIN
                            SET @Output = (@Value1 * @Value2);
                            SELECT @Output;
                        END";
                    connection.ExecuteNonQuery(commandText);
                }
            }
        }

        /// <summary>
        /// Creates the 'IdentityTableType' type.
        /// </summary>
        private static void CreateIdentityTableTypeStoredProcedure()
        {
            using (var connection = new SqlConnection(ConnectionStringForRepoDb).EnsureOpen())
            {
                var exists = connection.ExecuteScalar("SELECT 1 FROM [sys].[objects] WHERE type = 'P' AND name = 'sp_identity_table_type';");
                if (exists == null)
                {
                    var commandText = @"CREATE PROCEDURE [dbo].[sp_identity_table_type]
                        (
                            @Table IdentityTableType READONLY
                        )
	                    AS
                        BEGIN
                            INSERT INTO [sc].[IdentityTable]
                            (
                                [RowGuid],
                                [ColumnBit],
                                [ColumnDateTime],
                                [ColumnDateTime2],
                                [ColumnDecimal],
                                [ColumnFloat],
                                [ColumnInt],
                                [ColumnNVarChar]
                            )
                            OUTPUT INSERTED.*
                            SELECT [RowGuid],
                                [ColumnBit],
                                [ColumnDateTime],
                                [ColumnDateTime2],
                                [ColumnDecimal],
                                [ColumnFloat],
                                [ColumnInt],
                                [ColumnNVarChar]
                            FROM @Table;
                        END";
                    connection.ExecuteNonQuery(commandText);
                }
            }
        }

        #endregion
    }
}
