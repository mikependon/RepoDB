using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RepoDb.IntegrationTests.Types.Bytes
{
    [TestClass]
    public class DbRepositoryBytesTest
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
                connection.DeleteAll<BytesClass>();
            }
        }

        [TestMethod]
        public void TestDbRepositoryBytesCrud()
        {
            // Setup
            var entity = new BytesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BytesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deletedRows = repository.Delete<BytesClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BytesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBytesNullCrud()
        {
            // Setup
            var entity = new BytesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BytesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);

                // Act Delete
                var deletedRows = repository.Delete<BytesClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BytesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBytesMappedCrud()
        {
            // Setup
            var entity = new BytesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BytesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deletedRows = repository.Delete<BytesMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BytesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBytesMappedNullCrud()
        {
            // Setup
            var entity = new BytesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BytesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);

                // Act Delete
                var deletedRows = repository.Delete<BytesMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BytesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBytesCrudAsync()
        {
            // Setup
            var entity = new BytesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BytesClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BytesClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BytesClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBytesNullCrudAsync()
        {
            // Setup
            var entity = new BytesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BytesClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BytesClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BytesClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBytesMappedCrudAsync()
        {
            // Setup
            var entity = new BytesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BytesMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BytesMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BytesMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBytesMappedNullCrudAsync()
        {
            // Setup
            var entity = new BytesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BytesMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BytesMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BytesMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
