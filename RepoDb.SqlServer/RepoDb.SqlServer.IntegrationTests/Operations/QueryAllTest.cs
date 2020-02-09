using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class QueryAllTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionQueryAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll<CompleteTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionQueryAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll<CompleteTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionQueryAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<CompleteTable>());

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAll(ClassMappedNameCache.Get<CompleteTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionQueryAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAllAsync(ClassMappedNameCache.Get<CompleteTable>()).Result;

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var queryResult = connection.QueryAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion
    }
}
