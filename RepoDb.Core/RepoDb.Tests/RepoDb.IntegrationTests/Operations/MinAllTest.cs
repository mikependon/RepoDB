using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class MinAllTest
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

        #region MinAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionMinAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable, int?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable, int?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region MinAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionMinAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllAsyncTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync<IdentityTable, int?>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllAsyncWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync<IdentityTable, int?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region MinAll(TableName)

        [TestMethod]
        public void TestSqlConnectionMinViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region MinAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionMinAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        #endregion
    }
}
