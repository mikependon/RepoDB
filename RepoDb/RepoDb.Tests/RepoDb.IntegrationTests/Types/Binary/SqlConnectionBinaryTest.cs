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
    public class SqlConnectionBinaryTest
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
        public void TestSqlConnectionBinaryCrud()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BinaryClass>(e => e.SessionId == (Guid)id).FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deletedRows = connection.Delete<BinaryClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BinaryClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBinaryNullCrud()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BinaryClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);

                // Act Delete
                var deletedRows = connection.Delete<BinaryClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BinaryClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBinaryMappedCrud()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BinaryMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deletedRows = connection.Delete<BinaryMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BinaryMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBinaryMappedNullCrud()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BinaryMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);

                // Act Delete
                var deletedRows = connection.Delete<BinaryMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<BinaryMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBinaryCrudAsync()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBinaryNullCrudAsync()
        {
            // Setup
            var entity = new BinaryClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinary = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BinaryClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBinaryMappedCrudAsync()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = Encoding.UTF8.GetBytes("ABCDE")
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();
                var result = Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray());

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual("ABCDE", result);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBinaryMappedNullCrudAsync()
        {
            // Setup
            var entity = new BinaryMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBinaryMapped = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<BinaryMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
