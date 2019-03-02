using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Bit
{
    [TestClass]
    public class SqlConnectionBitTest
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
        public void TestSqlConnectionBitCrud()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBit);

                // Act Delete
                var deletedRows = connection.Delete<BitClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBitNullCrud()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBit);

                // Act Delete
                var deletedRows = connection.Delete<BitClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBitMappedCrud()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BitMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBitMapped);

                // Act Delete
                var deletedRows = connection.Delete<BitMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BitMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBitMappedNullCrud()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BitMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBitMapped);

                // Act Delete
                var deletedRows = connection.Delete<BitMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BitMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBitCrudAsync()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BitClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBit);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BitClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BitClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBitNullCrudAsync()
        {
            // Setup
            var entity = new BitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BitClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBit);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BitClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BitClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBitMappedCrudAsync()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(true, data.ColumnBitMapped);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBitMappedNullCrudAsync()
        {
            // Setup
            var entity = new BitMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBitMapped = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBitMapped);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BitMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
