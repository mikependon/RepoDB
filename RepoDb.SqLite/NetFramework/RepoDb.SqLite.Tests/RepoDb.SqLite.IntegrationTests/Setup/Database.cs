using RepoDb.SqLite.IntegrationTests.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace RepoDb.SqLite.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        public static string ConnectionString = @"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;";

        #endregion

        #region Methods

        public static void Initialize()
        {
            Bootstrap.Initialize();
            // Create Database
            // Create Tables
        }

        public static void Cleanup()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.DeleteAll<CompleteTable>();
            }
        }

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                var tables = Helper.CreateCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion
    }
}
