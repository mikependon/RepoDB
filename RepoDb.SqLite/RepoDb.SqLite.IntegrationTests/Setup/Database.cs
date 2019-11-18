using RepoDb.SqLite.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace RepoDb.SqLite.IntegrationTests.Setup
{
    public static class Database
    {
        static Database()
        {
            // Check the connection string
            var environment = Environment.GetEnvironmentVariable("REPODB_ENVIRONMENT", EnvironmentVariableTarget.User);

            // Set the property
            IsInMemory = (environment != "DEVELOPMENT");
        }

        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used.
        /// </summary>
        public static string ConnectionString { get; private set; } = @"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;";

        /// <summary>
        /// Gets the value that indicates whether to use the in-memory database.
        /// </summary>
        public static bool IsInMemory { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Initialize SqLite
            Bootstrap.Initialize();

            // Check the type of database
            if (IsInMemory == true)
            {
                // Memory
                ConnectionString = @"Data Source=:memory:;Version=3;";
            }
            else
            {
                // Local
                ConnectionString = @"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;";

                // Create tables
                CreateTables();
            }
        }

        public static void Cleanup()
        {
            if (IsInMemory == true)
            {
                return;
            }
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.DeleteAll<CompleteTable>();
                connection.DeleteAll<NonIdentityCompleteTable>();
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count,
            SQLiteConnection connection = null)
        {
            var hasConnection = (connection != null);
            if (hasConnection == false)
            {
                connection = new SQLiteConnection(ConnectionString);
            }
            try
            {
                var tables = Helper.CreateCompleteTables(count);
                CreateCompleteTable(connection);
                connection.InsertAll(tables);
                return tables;
            }
            finally
            {
                if (hasConnection == false)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion

        #region NonIdentityCompleteTable

        public static IEnumerable<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count,
            SQLiteConnection connection = null)
        {
            var hasConnection = (connection != null);
            if (hasConnection == false)
            {
                connection = new SQLiteConnection(ConnectionString);
            }
            try
            {
                var tables = Helper.CreateNonIdentityCompleteTables(count);
                CreateNonIdentityCompleteTable(connection);
                connection.InsertAll(tables);
                return tables;
            }
            finally
            {
                if (hasConnection == false)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion

        #region CreateTables

        public static void CreateTables(SQLiteConnection connection = null)
        {
            CreateCompleteTable(connection);
            CreateNonIdentityCompleteTable(connection);
        }

        public static void CreateCompleteTable(SQLiteConnection connection = null)
        {
            var hasConnection = (connection != null);
            if (hasConnection == false)
            {
                connection = new SQLiteConnection(ConnectionString);
            }
            try
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [CompleteTable] 
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
            finally
            {
                if (hasConnection == false)
                {
                    connection.Dispose();
                }
            }
        }

        public static void CreateNonIdentityCompleteTable(SQLiteConnection connection = null)
        {
            var hasConnection = (connection != null);
            if (hasConnection == false)
            {
                connection = new SQLiteConnection(ConnectionString);
            }
            try
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [NonIdentityCompleteTable] 
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
            finally
            {
                if (hasConnection == false)
                {
                    connection.Dispose();
                }
            }
        }

        #endregion
    }
}
