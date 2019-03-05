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
    public class SqlConnectionBytesTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        [TestMethod]
        public void TestSqlConnectionBytesCrud()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BytesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray()));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnImage));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnVarBinary));
                Assert.AreEqual(entity.ColumnTinyInt, data.ColumnTinyInt);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBytesNullCrud()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BytesClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);
                Assert.IsNull(data.ColumnImage);
                Assert.IsNull(data.ColumnTinyInt);
                Assert.IsNull(data.ColumnVarBinary);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBytesMappedCrud()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BytesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray()));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnImageMapped));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnVarBinaryMapped));
                Assert.AreEqual(entity.ColumnTinyIntMapped, data.ColumnTinyIntMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBytesMappedNullCrud()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BytesMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);
                Assert.IsNull(data.ColumnImageMapped);
                Assert.IsNull(data.ColumnTinyIntMapped);
                Assert.IsNull(data.ColumnVarBinaryMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBytesCrudAsync()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BytesClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnBinary.Take(entity.ColumnBinary.Length).ToArray()));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnImage));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnVarBinary));
                Assert.AreEqual(entity.ColumnTinyInt, data.ColumnTinyInt);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBytesNullCrudAsync()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BytesClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinary);
                Assert.IsNull(data.ColumnImage);
                Assert.IsNull(data.ColumnTinyInt);
                Assert.IsNull(data.ColumnVarBinary);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBytesMappedCrudAsync()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BytesMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnBinaryMapped.Take(entity.ColumnBinaryMapped.Length).ToArray()));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnImageMapped));
                Assert.AreEqual(text, Encoding.UTF8.GetString(data.ColumnVarBinaryMapped));
                Assert.AreEqual(entity.ColumnTinyIntMapped, data.ColumnTinyIntMapped);
            }
        }

        [TestMethod]
        public void TestSqlConnectionBytesMappedNullCrudAsync()
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<BytesMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnBinaryMapped);
                Assert.IsNull(data.ColumnImageMapped);
                Assert.IsNull(data.ColumnTinyIntMapped);
                Assert.IsNull(data.ColumnVarBinaryMapped);
            }
        }
    }
}
