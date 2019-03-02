using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.BigInt
{
    [TestClass]
    public class DbRepositoryBigIntTest
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
        public void TestDbRepositoryBigIntCrud()
        {
            // Setup
            var entity = new BigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = Int64.MaxValue
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(Int64.MaxValue, data.ColumnBigInt);

                // Act Delete
                var deletedRows = repository.Delete<BigIntClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBigIntNullCrud()
        {
            // Setup
            var entity = new BigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigInt);

                // Act Delete
                var deletedRows = repository.Delete<BigIntClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBigIntMappedCrud()
        {
            // Setup
            var entity = new BigIntMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = Int64.MaxValue
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BigIntMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(Int64.MaxValue, data.ColumnBigIntMapped);

                // Act Delete
                var deletedRows = repository.Delete<BigIntMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BigIntMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBigIntMappedNullCrud()
        {
            // Setup
            var entity = new BigIntMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BigIntMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigIntMapped);

                // Act Delete
                var deletedRows = repository.Delete<BigIntMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BigIntMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBigIntCrudAsync()
        {
            // Setup
            var entity = new BigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = Int64.MaxValue
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(Int64.MaxValue, data.ColumnBigInt);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBigIntNullCrudAsync()
        {
            // Setup
            var entity = new BigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigInt);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BigIntClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBigIntMappedCrudAsync()
        {
            // Setup
            var entity = new BigIntMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = Int64.MaxValue
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(Int64.MaxValue, data.ColumnBigIntMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBigIntMappedNullCrudAsync()
        {
            // Setup
            var entity = new BigIntMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigIntMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBigIntMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BigIntMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
