using RepoDb.Connector.MariaDb;
using RepoDb.MariaDb.BulkOperations.IntegrationTests.Models;
using System;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// A class used as a startup setup for the RepoDb MariaDb bulk-operations test database.
    /// </summary>
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the connection string used to reach the MariaDB server itself (the <c>sys</c> database) -
        /// used only to create the <c>RepoDb</c> database if it does not already exist.
        /// </summary>
        public static string ConnectionStringForSystem { get; private set; }

        /// <summary>
        /// Gets the connection string to be used for the MariaDb database.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the creation of the database.
        /// </summary>
        public static void Initialize()
        {
            ConnectionStringForSystem =
                Environment.GetEnvironmentVariable("REPODB_MARIADB_CONSTR_SYSTEM") ??
                "Server=127.0.0.1;Port=3307;Database=sys;User ID=root;Password=RepoDB2026;";

            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_MARIADB_CONSTR") ??
                "Server=127.0.0.1;Port=3307;Database=RepoDb;User ID=root;Password=RepoDB2026;AllowLoadLocalInfile=True;AllowUserVariables=True;";

            // Initialize MariaDb
            GlobalConfiguration
                .Setup()
                .UseMariaDb();

            // Enable server side local in file for bulk
            EnableServerLocalInfile();

            // Create the database first
            CreateDatabase();

            // Create the tables
            CreateTables();
        }

        /// <summary>
        /// Creates the <c>RepoDb</c> database if it does not already exist.
        /// </summary>
        public static void CreateDatabase()
        {
            using var connection = new MariaDbConnection(ConnectionStringForSystem);
            connection.ExecuteNonQuery("CREATE DATABASE IF NOT EXISTS `RepoDb`;");
        }

        /// <summary>
        /// Enables the server-side <c>local_infile</c> global variable, which MariaDB disables by default.
        /// Required for <c>LOAD DATA LOCAL INFILE</c> - the mechanism <c>MariaDbBulkCopy</c> uses under the
        /// hood for every bulk operation in this package - to work; the client-side counterpart is the
        /// "AllowLoadLocalInfile=True" flag on <see cref="ConnectionString"/>. Requires a user with
        /// SUPER/SYSTEM_VARIABLES_ADMIN privilege (root has it by default), and takes effect immediately
        /// for new connections - no server restart needed.
        /// </summary>
        public static void EnableServerLocalInfile()
        {
            using var connection = new MariaDbConnection(ConnectionStringForSystem);
            connection.ExecuteNonQuery("SET GLOBAL local_infile = 1;");
        }

        /// <summary>
        /// Clean up all the table.
        /// </summary>
        public static void Cleanup()
        {
            using var connection = new MariaDbConnection(ConnectionString);
            connection.Truncate<BulkOperationIdentityTable>();
            connection.Truncate<BulkOperationNonIdentityTable>();
        }

        #endregion

        #region CreateTables

        /// <summary>
        /// Create the necessary tables for testing.
        /// </summary>
        public static void CreateTables()
        {
            CreateBulkOperationIdentityTable();
            CreateBulkOperationNonIdentityTable();
        }

        /// <summary>
        /// Creates an identity table that has some important fields. All fields are nullable except
        /// <c>Id</c> and <c>RowGuid</c>. <c>Id</c> is <c>BIGINT AUTO_INCREMENT</c>; <c>RowGuid</c> is a
        /// <c>CHAR(36)</c> column that MariaDb binds directly to/from <see cref="Guid"/> using its
        /// default <c>GuidFormat=Char36</c> behavior - no property handler required.
        /// </summary>
        public static void CreateBulkOperationIdentityTable()
        {
            // MariaDB supports "CREATE TABLE IF NOT EXISTS" directly - no guard block needed.
            var commandText = @"
                CREATE TABLE IF NOT EXISTS `BulkOperationIdentityTable`
                (
                    `Id` BIGINT NOT NULL AUTO_INCREMENT,
                    `RowGuid` CHAR(36) NOT NULL,
                    `ColumnBit` TINYINT UNSIGNED NULL,
                    `ColumnDateTime` DATETIME NULL,
                    `ColumnDateTime2` DATETIME(6) NULL,
                    `ColumnDecimal` DECIMAL(18,2) NULL,
                    `ColumnFloat` DOUBLE NULL,
                    `ColumnInt` INT NULL,
                    `ColumnNVarChar` NVARCHAR(2000) NULL,
                    PRIMARY KEY (`Id`)
                ) ENGINE=InnoDB;";
            using var connection = new MariaDbConnection(ConnectionString);
            connection.ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// Creates a non-identity table that has some important fields. All fields are nullable except
        /// the primary key. Unlike <see cref="CreateBulkOperationIdentityTable"/>, <c>Id</c> here is a
        /// plain <c>BIGINT</c> primary key (no <c>AUTO_INCREMENT</c>) - the caller's value is stored
        /// as-is. Used by tests that need to know a row's <c>Id</c> ahead of time (e.g. matching against
        /// a separately-built anonymous/expando object by primary key), which a MariaDB-generated
        /// auto-increment value can't support.
        /// </summary>
        public static void CreateBulkOperationNonIdentityTable()
        {
            var commandText = @"
                CREATE TABLE IF NOT EXISTS `BulkOperationNonIdentityTable`
                (
                    `Id` BIGINT NOT NULL,
                    `RowGuid` CHAR(36) NOT NULL,
                    `ColumnBit` TINYINT UNSIGNED NULL,
                    `ColumnDateTime` DATETIME NULL,
                    `ColumnDateTime2` DATETIME(6) NULL,
                    `ColumnDecimal` DECIMAL(18,2) NULL,
                    `ColumnFloat` DOUBLE NULL,
                    `ColumnInt` INT NULL,
                    `ColumnNVarChar` NVARCHAR(2000) NULL,
                    PRIMARY KEY (`Id`)
                ) ENGINE=InnoDB;";
            using var connection = new MariaDbConnection(ConnectionString);
            connection.ExecuteNonQuery(commandText);
        }

        #endregion
    }
}
