using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RepoDb.IntegrationTests.Types.Bytes
{
    [TestClass]
    public class BaseRepositoryBytesTest
    {
        private class BytesClassRepository : BaseRepository<BytesClass, SqlConnection>
        {
            public BytesClassRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

        private class BytesMapClassRepository : BaseRepository<BytesMapClass, SqlConnection>
        {
            public BytesMapClassRepository(string connectionString) : base(connectionString, (int?)0) { }
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
                connection.DeleteAll<BytesClass>();
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBytesCrud()
        {
            // Setup
            var text = Helper.GetAssemblyDescription();
            var bytes = Encoding.UTF8.GetBytes(text);
            var entity = new BytesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = bytes,
                ColumnImage = bytes,
                ColumnVarBinary = bytes,
                ColumnTinyInt = 128
            };

            using (var repository = new BytesClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray()));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnImage));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnVarBinary));
                Assert.AreEqual(entity.ColumnTinyInt, data.ColumnTinyInt);

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
        public void TestBaseRepositoryBytesNullCrud()
        {
            // Setup
            var entity = new BytesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null,
                ColumnImage = null,
                ColumnTinyInt = null,
                ColumnVarBinary = null
            };

            using (var repository = new BytesClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);
                Assert.IsNull(data.ColumnImage);
                Assert.IsNull(data.ColumnTinyInt);
                Assert.IsNull(data.ColumnVarBinary);

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
        public void TestBaseRepositoryBytesMappedCrud()
        {
            // Setup
            var text = Helper.GetAssemblyDescription();
            var bytes = Encoding.UTF8.GetBytes(text);
            var entity = new BytesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = bytes,
                ColumnImageMapped = bytes,
                ColumnVarBinaryMapped = bytes,
                ColumnTinyIntMapped = 128
            };

            using (var repository = new BytesMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray()));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnImageMapped));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnVarBinaryMapped));
                Assert.AreEqual(entity.ColumnTinyIntMapped, data.ColumnTinyIntMapped);

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
        public void TestBaseRepositoryBytesMappedNullCrud()
        {
            // Setup
            var entity = new BytesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null,
                ColumnImageMapped = null,
                ColumnTinyIntMapped = null,
                ColumnVarBinaryMapped = null
            };

            using (var repository = new BytesMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);
                Assert.IsNull(data.ColumnImageMapped);
                Assert.IsNull(data.ColumnTinyIntMapped);
                Assert.IsNull(data.ColumnVarBinaryMapped);

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
        public void TestBaseRepositoryBytesCrudAsync()
        {
            // Setup
            var text = Helper.GetAssemblyDescription();
            var bytes = Encoding.UTF8.GetBytes(text);
            var entity = new BytesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = bytes,
                ColumnImage = bytes,
                ColumnVarBinary = bytes,
                ColumnTinyInt = 128
            };

            using (var repository = new BytesClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray()));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnImage));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnVarBinary));
                Assert.AreEqual(entity.ColumnTinyInt, data.ColumnTinyInt);

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
        public void TestBaseRepositoryBytesNullCrudAsync()
        {
            // Setup
            var entity = new BytesClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null,
                ColumnImage = null,
                ColumnTinyInt = null,
                ColumnVarBinary = null
            };

            using (var repository = new BytesClassRepository(Database.ConnectionStringForRepoDb))
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
                Assert.IsNull(data.ColumnImage);
                Assert.IsNull(data.ColumnTinyInt);
                Assert.IsNull(data.ColumnVarBinary);

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
        public void TestBaseRepositoryBytesMappedCrudAsync()
        {
            // Setup
            var text = Helper.GetAssemblyDescription();
            var bytes = Encoding.UTF8.GetBytes(text);
            var entity = new BytesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = bytes,
                ColumnImageMapped = bytes,
                ColumnVarBinaryMapped = bytes,
                ColumnTinyIntMapped = 128
            };

            using (var repository = new BytesMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray()));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnImageMapped));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnVarBinaryMapped));
                Assert.AreEqual(entity.ColumnTinyIntMapped, data.ColumnTinyIntMapped);

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
        public void TestBaseRepositoryBytesMappedNullCrudAsync()
        {
            // Setup
            var entity = new BytesMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null,
                ColumnImageMapped = null,
                ColumnTinyIntMapped = null,
                ColumnVarBinaryMapped = null
            };

            using (var repository = new BytesMapClassRepository(Database.ConnectionStringForRepoDb))
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
                Assert.IsNull(data.ColumnImageMapped);
                Assert.IsNull(data.ColumnTinyIntMapped);
                Assert.IsNull(data.ColumnVarBinaryMapped);

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
