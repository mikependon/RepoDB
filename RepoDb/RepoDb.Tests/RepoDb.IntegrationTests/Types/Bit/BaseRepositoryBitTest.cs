using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Bit
{
    [TestClass]
    public class BaseRepositoryBitTest
    {
        private class BitClassRepository : BaseRepository<BitClass, SqlConnection>
        {
            public BitClassRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

        private class BitMapClassRepository : BaseRepository<BitMapClass, SqlConnection>
        {
            public BitMapClassRepository(string connectionString) : base(connectionString, (int?)0) { }
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
                connection.DeleteAll<BitClass>();
                connection.DeleteAll<BitMapClass>();
            }
        }

        [TestMethod]
        public void TestBaseRepositoryBitCrud()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = true
            };

            using (var repository = new BitClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBit);

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
        public void TestBaseRepositoryBitNullCrud()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null
            };

            using (var repository = new BitClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBit);

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
        public void TestBaseRepositoryBitMappedCrud()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = true
            };

            using (var repository = new BitMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBitMapped);

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
        public void TestBaseRepositoryBitMappedNullCrud()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = null
            };

            using (var repository = new BitMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBitMapped);

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
        public void TestBaseRepositoryBitCrudAsync()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = true
            };

            using (var repository = new BitClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBit);

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
        public void TestBaseRepositoryBitNullCrudAsync()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null
            };

            using (var repository = new BitClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBit);

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
        public void TestBaseRepositoryBitMappedCrudAsync()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = true
            };

            using (var repository = new BitMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBitMapped);

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
        public void TestBaseRepositoryBitMappedNullCrudAsync()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = null
            };

            using (var repository = new BitMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBitMapped);

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
