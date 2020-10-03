using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class MaxAllTest
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

        #region MaxAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionMaxAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable, int?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable, int?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region MaxAllAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionMaxAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync<IdentityTable>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllAsyncTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync<IdentityTable, int?>(e => e.ColumnInt).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllAsyncWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync<IdentityTable, int?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region MaxAll(TableName)

        [TestMethod]
        public void TestSqlConnectionMaxViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region MaxAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionMaxAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt")).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        #endregion
    }
}
