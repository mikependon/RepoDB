using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class SkipQueryTest
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
        public void TestSqlServerConnectionSkipQueryFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    6,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    6,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryFirstBatchAscendingWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    skip: 0,
                    rowsPerBatch: 3,
                    orderBy: OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    where: (object)null,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    6,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    6,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryAsyncFirstBatchAscendingWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    skip: 0,
                    rowsPerBatch: 3,
                    orderBy: OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    where: (object)null,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryViaTableNameFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryViaTableNameFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryViaTableNameThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    6,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryViaTableNameThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    6,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionSkipQueryViaTableNameFirstBatchAscendingWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    where: (object)null,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryViaTableNameAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryViaTableNameAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryViaTableNameAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    6,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryViaTableNameAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    6,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionSkipQueryViaTableNameAsyncFirstBatchAscendingWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    where: (object)null,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Helper.AssertMembersEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertMembersEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        #endregion

        #endregion
    }
}
