using System;
using System.Collections.Generic;
using MySqlConnector;
using RepoDb.MySqlConnector.IntegrationTests.Models;

namespace RepoDb.MySqlConnector.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used for sys.
        /// </summary>
        public static string ConnectionStringForSys { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            ConnectionStringForSys =
                Environment.GetEnvironmentVariable("REPODB_MYSQL_CONSTR_SYS")
                ?? Environment.GetEnvironmentVariable("REPODB_CONSTR_SYS")
                // ?? @"Server=127.0.0.1;Port=43306;Database=sys;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;" // Docker test configuration
                ?? @"Server=localhost;Database=sys;Uid=user;Pwd=Password123;";

            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_MYSQL_CONSTR_REPODBTEST")
                ?? Environment.GetEnvironmentVariable("REPODB_CONSTR")
                // ?? @"Server=127.0.0.1;Port=43306;Database=RepoDbTest;User ID=root;Password=ddd53e85-b15e-4da8-91e5-a7d3b00a0ab2;" // Docker test configuration
                ?? @"Server=localhost;Database=RepoDbTest;Uid=user;Pwd=Password123;";

            // Initialize MySql
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector();

            // Create databases
            CreateDatabase();

            // Create tables
            CreateTables();
        }

        public static void Cleanup()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Truncate<CompleteTable>();
                connection.Truncate<NonIdentityCompleteTable>();
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new MySqlConnection(ConnectionString))
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
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var tables = Helper.CreateNonIdentityCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion

        #region CreateDatabases

        private static void CreateDatabase()
        {
            using (var connection = new MySqlConnection(ConnectionStringForSys))
            {
                connection.ExecuteNonQuery(@"CREATE DATABASE IF NOT EXISTS `RepoDbTest`;");
                connection.ExecuteNonQuery(@"GRANT ALL Privileges on RepoDbTest.* to 'root'@'%';");
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
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS `CompleteTable`
                    (
                        `Id` bigint(20) NOT NULL AUTO_INCREMENT,
                        `ColumnVarchar` varchar(256) DEFAULT NULL,
                        `ColumnInt` int(11) DEFAULT NULL,
                        `ColumnDecimal2` decimal(18,2) DEFAULT NULL,
                        `ColumnDateTime` datetime DEFAULT NULL,
                        `ColumnBlob` blob,
                        `ColumnBlobAsArray` blob,
                        `ColumnBinary` binary(255) DEFAULT NULL,
                        `ColumnLongBlob` longblob,
                        `ColumnMediumBlob` mediumblob,
                        `ColumnTinyBlob` tinyblob,
                        `ColumnVarBinary` varbinary(256) DEFAULT NULL,
                        `ColumnDate` date DEFAULT NULL,
                        `ColumnDateTime2` datetime(5) DEFAULT NULL,
                        `ColumnTime` time DEFAULT NULL,
                        `ColumnTimeStamp` timestamp(5) NULL DEFAULT NULL,
                        `ColumnYear` year(4) DEFAULT NULL,
                        `ColumnGeometry` geometry DEFAULT NULL,
                        `ColumnLineString` linestring DEFAULT NULL,
                        `ColumnMultiLineString` multilinestring DEFAULT NULL,
                        `ColumnMultiPoint` multipoint DEFAULT NULL,
                        `ColumnMultiPolygon` multipolygon DEFAULT NULL,
                        `ColumnPoint` point DEFAULT NULL,
                        `ColumnPolygon` polygon DEFAULT NULL,
                        `ColumnBigint` bigint(64) DEFAULT NULL,
                        `ColumnDecimal` decimal(10,0) DEFAULT NULL,
                        `ColumnDouble` double DEFAULT NULL,
                        `ColumnFloat` float DEFAULT NULL,
                        `ColumnInt2` int(32) DEFAULT NULL,
                        `ColumnMediumInt` mediumint(16) DEFAULT NULL,
                        `ColumnReal` double DEFAULT NULL,
                        `ColumnSmallInt` smallint(8) DEFAULT NULL,
                        `ColumnTinyInt` tinyint(4) DEFAULT NULL,
                        `ColumnChar` char(1) DEFAULT NULL,
                        `ColumnJson` json DEFAULT NULL,
                        `ColumnNChar` nchar(16) DEFAULT NULL,
                        `ColumnNVarChar` nvarchar(256) DEFAULT NULL,
                        `ColumnLongText` longtext,
                        `ColumnMediumText` mediumtext,
                        `ColumnText` text,
                        `ColumnTinyText` tinytext,
                        `ColumnBit` bit(1) DEFAULT NULL,
                        PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB;");
            }
        }

        private static void CreateNonIdentityCompleteTable()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS `NonIdentityCompleteTable`
                    (
                        `Id` bigint(20) NOT NULL,
                        `ColumnVarchar` varchar(256) DEFAULT NULL,
                        `ColumnInt` int(11) DEFAULT NULL,
                        `ColumnDecimal2` decimal(18, 2) DEFAULT NULL,
                        `ColumnDateTime` datetime DEFAULT NULL,
                        `ColumnBlob` blob,
                        `ColumnBlobAsArray` blob,
                        `ColumnBinary` binary(255) DEFAULT NULL,
                        `ColumnLongBlob` longblob,
                        `ColumnMediumBlob` mediumblob,
                        `ColumnTinyBlob` tinyblob,
                        `ColumnVarBinary` varbinary(256) DEFAULT NULL,
                        `ColumnDate` date DEFAULT NULL,
                        `ColumnDateTime2` datetime(5) DEFAULT NULL,
                        `ColumnTime` time DEFAULT NULL,
                        `ColumnTimeStamp` timestamp(5) NULL DEFAULT NULL,
                        `ColumnYear` year(4) DEFAULT NULL,
                        `ColumnGeometry` geometry DEFAULT NULL,
                        `ColumnLineString` linestring DEFAULT NULL,
                        `ColumnMultiLineString` multilinestring DEFAULT NULL,
                        `ColumnMultiPoint` multipoint DEFAULT NULL,
                        `ColumnMultiPolygon` multipolygon DEFAULT NULL,
                        `ColumnPoint` point DEFAULT NULL,
                        `ColumnPolygon` polygon DEFAULT NULL,
                        `ColumnBigint` bigint(64) DEFAULT NULL,
                        `ColumnDecimal` decimal(10, 0) DEFAULT NULL,
                        `ColumnDouble` double DEFAULT NULL,
                        `ColumnFloat` float DEFAULT NULL,
                        `ColumnInt2` int(32) DEFAULT NULL,
                        `ColumnMediumInt` mediumint(16) DEFAULT NULL,
                        `ColumnReal` double DEFAULT NULL,
                        `ColumnSmallInt` smallint(8) DEFAULT NULL,
                        `ColumnTinyInt` tinyint(4) DEFAULT NULL,
                        `ColumnChar` char(1) DEFAULT NULL,
                        `ColumnJson` json DEFAULT NULL,
                        `ColumnNChar` nchar(16) DEFAULT NULL,
                        `ColumnNVarChar` nvarchar(256) DEFAULT NULL,
                        `ColumnLongText` longtext,
                        `ColumnMediumText` mediumtext,
                        `ColumnText` text,
                        `ColumnTinyText` tinytext,
                        `ColumnBit` bit(1) DEFAULT NULL,
                        PRIMARY KEY(`Id`)
                    ) ENGINE = InnoDB;");
            }
        }

        #endregion
    }
}
