using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class AverageAllTest
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
        public void TestSqlServerConnectionAverageAll()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll<IdentityCompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionAverageAllWithHints()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll<IdentityCompleteTable>(e => e.ColumnInt,
                    SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionAverageAllAsync()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync<IdentityCompleteTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionAverageAllAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync<IdentityCompleteTable>(e => e.ColumnInt,
                    SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionAverageAllViaTableName()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    Field.Parse<IdentityCompleteTable>(e => e.ColumnInt).First());

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionAverageAllViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.AverageAll(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    Field.Parse<IdentityCompleteTable>(e => e.ColumnInt).First(),
                    SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionAverageAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    Field.Parse<IdentityCompleteTable>(e => e.ColumnInt).First());

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionAverageAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    Field.Parse<IdentityCompleteTable>(e => e.ColumnInt).First(),
                    SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #endregion
    }
}
