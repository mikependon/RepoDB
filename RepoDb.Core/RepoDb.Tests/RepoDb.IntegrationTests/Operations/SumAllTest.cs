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
    public class SumAllTest
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

        #region SumAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionSumAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable, int?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable, int?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultDecimal()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable, decimal?>(e => e.ColumnDecimal);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllWithHintsTypedResultDecimal()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable, double?>(e => e.ColumnFloat);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllWithHintsTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<IdentityTable, double?>(e => e.ColumnFloat,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region SumAllAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<IdentityTable, int?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<IdentityTable, int?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncTypedResultDecimal()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<IdentityTable, decimal?>(e => e.ColumnDecimal);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncWithHintsTypedResultDecimal()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<IdentityTable, double?>(e => e.ColumnFloat);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncWithHintsTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region SumAll(TableName)

        [TestMethod]
        public void TestSqlConnectionSumViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultDecimalViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultDecimalViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultDoubleViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAllTypedResultDoubleViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region SumAllAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllTypedResultAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllTypedResultAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllTypedResultDecimalAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllTypedResultDecimalAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllTypedResultDoubleAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"));

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAllTypedResultDoubleAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        #endregion
    }
}
