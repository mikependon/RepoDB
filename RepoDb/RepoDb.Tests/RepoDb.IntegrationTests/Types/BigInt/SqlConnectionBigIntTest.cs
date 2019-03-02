using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.BigInt
{
    [TestClass]
    public class SqlConnectionBigIntTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Init();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.DeleteAll<BigIntClass>();
                connection.DeleteAll<BigIntMapClass>();
            }
        }

        [TestMethod]
        public void TestSqlConnectionBigIntCrud()
        {
            // Setup
            var entity = new BigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = Int64.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(Int64.MaxValue, data.ColumnBigInt);

                // Act Delete
                var deletedRows = connection.Delete<BigIntClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBigIntNullCrud()
        {
            // Setup
            var entity = new BigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(null);

                // Act Delete
                var deletedRows = connection.Delete<BigIntClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBigIntMappedCrud()
        {
            // Setup
            var entity = new BigIntMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = Int64.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(Int64.MaxValue, data.ColumnBigIntMapped);

                // Act Delete
                var deletedRows = connection.Delete<BigIntMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BigIntMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBigIntMappedNullCrud()
        {
            // Setup
            var entity = new BigIntMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(null);

                // Act Delete
                var deletedRows = connection.Delete<BigIntMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BigIntMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBigIntCrudAsync()
        {
            // Setup
            var entity = new BigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = Int64.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(Int64.MaxValue, data.ColumnBigInt);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBigIntNullCrudAsync()
        {
            // Setup
            var entity = new BigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigInt);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBigIntMappedCrudAsync()
        {
            // Setup
            var entity = new BigIntMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = Int64.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(Int64.MaxValue, data.ColumnBigIntMapped);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBigIntMappedNullCrudAsync()
        {
            // Setup
            var entity = new BigIntMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigIntMapped);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
