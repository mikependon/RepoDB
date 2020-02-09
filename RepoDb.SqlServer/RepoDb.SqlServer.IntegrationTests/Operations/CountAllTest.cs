using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class CountAllTest
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
        public void TestSqlServerConnectionCountAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionCountAllWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAll<CompleteTable>(SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionCountAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAllAsync<CompleteTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionCountAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAllAsync<CompleteTable>(SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionCountAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<CompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionCountAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAll(ClassMappedNameCache.Get<CompleteTable>(),
                    SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionCountAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionCountAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #endregion
    }
}
