using RepoDb.SqLite.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace RepoDb.SqLite.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used.
        /// </summary>
        public static string ConnectionString { get; private set; } = @"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;";

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Check the connection string
            var environment = Environment.GetEnvironmentVariable("REPODB_ENVIRONMENT", EnvironmentVariableTarget.User);

            // Server connection
            if (environment != "DEVELOPMENT")
            {
                ConnectionString = @"";
            }

            // Initialize SqLite
            Bootstrap.Initialize();

            // Create tables
            CreateTables();
        }

        public static void Cleanup()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.DeleteAll<CompleteTable>();
                connection.DeleteAll<NonIdentityCompleteTable>();
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
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
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var tables = Helper.CreateNonIdentityCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
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
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var result = connection.ExecuteScalar<string>("SELECT sql FROM [sqlite_master] WHERE name = 'CompleteTable' AND type = 'table';");
                if (string.IsNullOrEmpty(result))
                {
                    connection.ExecuteNonQuery(@"CREATE TABLE [CompleteTable] 
                        (
                           Id INTEGER PRIMARY KEY AUTOINCREMENT
                           , ColumnBigInt BIGINT
                           , ColumnBlob BLOB
                           , ColumnBoolean BOOLEAN
                           , ColumnChar CHAR
                           , ColumnDate DATE
                           , ColumnDateTime DATETIME
                           , ColumnDecimal DECIMAL
                           , ColumnDouble DOUBLE
                           , ColumnInteger INTEGER
                           , ColumnInt INT
                           , ColumnNone NONE
                           , ColumnNumeric NUMERIC
                           , ColumnReal REAL
                           , ColumnString STRING
                           , ColumnText TEXT
                           , ColumnTime TIME
                           , ColumnVarChar VARCHAR
                        );");
                }
            }
        }

        private static void CreateNonIdentityCompleteTable()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var result = connection.ExecuteScalar<string>("SELECT sql FROM [sqlite_master] WHERE name = 'NonIdentityCompleteTable' AND type = 'table';");
                if (string.IsNullOrEmpty(result))
                {
                    connection.ExecuteNonQuery(@"CREATE TABLE [NonIdentityCompleteTable] 
                        (
                           Id INTEGER PRIMARY KEY
                           , ColumnBigInt BIGINT
                           , ColumnBlob BLOB
                           , ColumnBoolean BOOLEAN
                           , ColumnChar CHAR
                           , ColumnDate DATE
                           , ColumnDateTime DATETIME
                           , ColumnDecimal DECIMAL
                           , ColumnDouble DOUBLE
                           , ColumnInteger INTEGER
                           , ColumnInt INT
                           , ColumnNone NONE
                           , ColumnNumeric NUMERIC
                           , ColumnReal REAL
                           , ColumnString STRING
                           , ColumnText TEXT
                           , ColumnTime TIME
                           , ColumnVarChar VARCHAR
                        );");
                }
            }
        }

        #endregion
    }
}
