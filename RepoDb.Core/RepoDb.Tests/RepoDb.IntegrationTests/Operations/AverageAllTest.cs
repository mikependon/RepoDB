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

        #region AverageAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionAverageAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable, double?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable, double?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResultDecimal()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable, decimal?>(e => e.ColumnDecimal);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllWithHintsTypedResultDecimal()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable, double?>(e => e.ColumnFloat);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllWithHintsTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<IdentityTable, double?>(e => e.ColumnFloat,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region AverageAllAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            await using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<IdentityTable>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            await using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<IdentityTable>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<IdentityTable, double?>(e => e.ColumnInt);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncWithHintsTypedResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<IdentityTable, double?>(e => e.ColumnInt,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncTypedResultDecimal()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<IdentityTable, decimal?>(e => e.ColumnDecimal);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncWithHintsTypedResultDecimal()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<IdentityTable, double?>(e => e.ColumnFloat);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncWithHintsTypedResultDouble()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region AverageAll(TableName)

        [TestMethod]
        public void TestSqlConnectionAverageViaAllTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResultViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResultViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResultDecimalViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResultDecimalViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResultDoubleViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageAllTypedResultDoubleViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.AverageAll<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region AverageAllAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllTypedResultAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllTypedResultAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllTypedResultDecimalAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllTypedResultDecimalAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllTypedResultDoubleAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"));

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAllTypedResultDoubleAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAllAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        #endregion
    }
}
