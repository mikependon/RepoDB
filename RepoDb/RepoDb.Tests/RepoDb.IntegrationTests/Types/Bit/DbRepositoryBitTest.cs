using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Bit
{
    [TestClass]
    public class DbRepositoryBitTest
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
                connection.DeleteAll<BitClass>();
                connection.DeleteAll<BitMapClass>();
            }
        }

        [TestMethod]
        public void TestDbRepositoryBitCrud()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBit);

                // Act Delete
                var deletedRows = repository.Delete<BitClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBitNullCrud()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBit);

                // Act Delete
                var deletedRows = repository.Delete<BitClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBitMappedCrud()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BitMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBitMapped);

                // Act Delete
                var deletedRows = repository.Delete<BitMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BitMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBitMappedNullCrud()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<BitMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBitMapped);

                // Act Delete
                var deletedRows = repository.Delete<BitMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<BitMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBitCrudAsync()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BitClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBit);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BitClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BitClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBitNullCrudAsync()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BitClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBit);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BitClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BitClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBitMappedCrudAsync()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = true
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBitMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryBitMappedNullCrudAsync()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBitMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
