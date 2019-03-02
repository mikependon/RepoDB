using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RepoDb.IntegrationTests.Types.Binary
{
    [TestClass]
    public class DbRepositoryBinaryTest
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
                connection.DeleteAll<BinaryClass>();
                connection.DeleteAll<BinaryMapClass>();
            }
        }

        [TestMethod]
        public void TestDbRepositoryBinaryCrud()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BinaryClass>(e => e.SessionId == (Guid)id).FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deletedRows = repository.Delete<BinaryClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BinaryClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBinaryNullCrud()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BinaryClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);

                // Act Delete
                var deletedRows = repository.Delete<BinaryClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BinaryClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBinaryMappedCrud()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BinaryMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deletedRows = repository.Delete<BinaryMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BinaryMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBinaryMappedNullCrud()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BinaryMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);

                // Act Delete
                var deletedRows = repository.Delete<BinaryMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BinaryMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBinaryCrudAsync()
        {
            // Setup
            var entity = new BinaryClass
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
                var queryResult = repository.QueryAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBinaryNullCrudAsync()
        {
            // Setup
            var entity = new BinaryClass
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
                var queryResult = repository.QueryAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBinaryMappedCrudAsync()
        {
            // Setup
            var entity = new BinaryMapClass
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
                var queryResult = repository.QueryAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBinaryMappedNullCrudAsync()
        {
            // Setup
            var entity = new BinaryMapClass
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
                var queryResult = repository.QueryAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
