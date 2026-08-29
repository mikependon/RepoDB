using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultDateTime()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable, DateTime?>(e => e.ColumnDateTime);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllWithHintsTypedResultDateTime()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable, double?>(e => e.ColumnFloat);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllWithHintsTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<IdentityTable, double?>(e => e.ColumnFloat,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region MinAllAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<IdentityTable, int?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<IdentityTable, int?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncTypedResultDateTime()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncWithHintsTypedResultDateTime()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<IdentityTable, double?>(e => e.ColumnFloat);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncWithHintsTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region MinAll(TableName)

        [TestMethod]
        public void TestSqlConnectionMinViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultDateTimeViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultDateTimeViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultDoubleViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinAllTypedResultDoubleViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MinAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region MinAllAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllTypedResultAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllTypedResultAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllTypedResultDateTimeAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllTypedResultDateTimeAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllTypedResultDoubleAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"));

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAllTypedResultDoubleAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        #endregion
    }
}
