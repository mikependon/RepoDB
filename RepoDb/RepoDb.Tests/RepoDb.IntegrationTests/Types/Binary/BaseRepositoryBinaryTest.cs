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
    public class BaseRepositoryBinaryTest
    {
        private class BinaryClassRepository : BaseRepository<BinaryClass, SqlConnection>
        {
            public BinaryClassRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

        private class BinaryMapClassRepository : BaseRepository<BinaryMapClass, SqlConnection>
        {
            public BinaryMapClassRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

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
        public void TestBaseRepositoryBinaryCrud()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new BinaryClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deletedRows = repository.Delete(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBinaryNullCrud()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null
            };

            using (var repository = new BinaryClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);

                // Act Delete
                var deletedRows = repository.Delete(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBinaryMappedCrud()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new BinaryMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deletedRows = repository.Delete(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBinaryMappedNullCrud()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null
            };

            using (var repository = new BinaryMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(null);

                // Act Delete
                var deletedRows = repository.Delete(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBinaryCrudAsync()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new BinaryClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBinaryNullCrudAsync()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null
            };

            using (var repository = new BinaryClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBinaryMappedCrudAsync()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var repository = new BinaryMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBinaryMappedNullCrudAsync()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null
            };

            using (var repository = new BinaryMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
