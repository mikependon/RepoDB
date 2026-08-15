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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable, double?>(e => e.ColumnFloat);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllWithHintsTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable, double?>(e => e.ColumnFloat,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultDateTime()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable, DateTime?>(e => e.ColumnDateTime);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllWithHintsTypedResultDateTime()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnDateTime), result);
            }
        }

        #endregion

        #region MaxAllAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<IdentityTable, int?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<IdentityTable, int?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<IdentityTable, double?>(e => e.ColumnFloat);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncWithHintsTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncTypedResultDateTime()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncWithHintsTypedResultDateTime()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnDateTime), result);
            }
        }

        #endregion

        #region MaxAll(TableName)

        [TestMethod]
        public void TestSqlConnectionMaxViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultDoubleViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultDoubleViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultDateTimeViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAllTypedResultDateTimeViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAll<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnDateTime), result);
            }
        }

        #endregion

        #region MaxAllAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllTypedResultAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllTypedResultAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllTypedResultDoubleAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllTypedResultDoubleAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllTypedResultDateTimeAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"));

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMaxAllTypedResultDateTimeAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MaxAllAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnDateTime), result);
            }
        }

        #endregion
    }
}
