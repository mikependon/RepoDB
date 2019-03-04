using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Strings
{
    [TestClass]
    public class SqlConnectionStringsTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Startup.Init();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                connection.DeleteAll<StringsClass>();
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsCrud()
        {
            // Setup
            var text = Helper.GetUnicodeString();
            var entity = new StringsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnChar = text,
                ColumnNChar = text,
                ColumnNText = text,
                ColumnNVarChar = text,
                ColumnText = text,
                ColumnVarChar = text
            };

            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnChar, data.ColumnChar.Trim());
                Assert.AreEqual(entity.ColumnNChar, data.ColumnNChar.Trim());
                Assert.AreEqual(entity.ColumnNText, data.ColumnNText);
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnText, data.ColumnText);
                Assert.AreEqual(entity.ColumnVarChar, data.ColumnVarChar);

                // Act Delete
                var deletedRows = connection.Delete<StringsClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<StringsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsNullCrud()
        {
            // Setup
            var entity = new StringsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnChar = null,
                ColumnNChar = null,
                ColumnNText = null,
                ColumnNVarChar = null,
                ColumnText = null,
                ColumnVarChar = null
            };

            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnChar);
                Assert.IsNull(data.ColumnNChar);
                Assert.IsNull(data.ColumnNText);
                Assert.IsNull(data.ColumnNVarChar);
                Assert.IsNull(data.ColumnText);
                Assert.IsNull(data.ColumnVarChar);

                // Act Delete
                var deletedRows = connection.Delete<StringsClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<StringsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsMappedCrud()
        {
            // Setup
            var text = Helper.GetUnicodeString();
            var entity = new StringsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnCharMapped = text,
                ColumnNCharMapped = text,
                ColumnNTextMapped = text,
                ColumnNVarCharMapped = text,
                ColumnTextMapped = text,
                ColumnVarCharMapped = text
            };

            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnCharMapped, data.ColumnCharMapped.Trim());
                Assert.AreEqual(entity.ColumnNCharMapped, data.ColumnNCharMapped.Trim());
                Assert.AreEqual(entity.ColumnNTextMapped, data.ColumnNTextMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
                Assert.AreEqual(entity.ColumnTextMapped, data.ColumnTextMapped);
                Assert.AreEqual(entity.ColumnVarCharMapped, data.ColumnVarCharMapped);

                // Act Delete
                var deletedRows = connection.Delete<StringsMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<StringsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsMappedNullCrud()
        {
            // Setup
            var entity = new StringsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnCharMapped = null,
                ColumnNCharMapped = null,
                ColumnNTextMapped = null,
                ColumnNVarCharMapped = null,
                ColumnTextMapped = null,
                ColumnVarCharMapped = null
            };

            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnCharMapped);
                Assert.IsNull(data.ColumnNCharMapped);
                Assert.IsNull(data.ColumnNTextMapped);
                Assert.IsNull(data.ColumnNVarCharMapped);
                Assert.IsNull(data.ColumnTextMapped);
                Assert.IsNull(data.ColumnVarCharMapped);

                // Act Delete
                var deletedRows = connection.Delete<StringsMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<StringsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsCrudAsync()
        {
            // Setup
            var text = Helper.GetUnicodeString();
            var entity = new StringsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnChar = text,
                ColumnNChar = text,
                ColumnNText = text,
                ColumnNVarChar = text,
                ColumnText = text,
                ColumnVarChar = text
            };

            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<StringsClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnChar, data.ColumnChar.Trim());
                Assert.AreEqual(entity.ColumnNChar, data.ColumnNChar.Trim());
                Assert.AreEqual(entity.ColumnNText, data.ColumnNText);
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnText, data.ColumnText);
                Assert.AreEqual(entity.ColumnVarChar, data.ColumnVarChar);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<StringsClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<StringsClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsNullCrudAsync()
        {
            // Setup
            var entity = new StringsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnChar = null,
                ColumnNChar = null,
                ColumnNText = null,
                ColumnNVarChar = null,
                ColumnText = null,
                ColumnVarChar = null
            };

            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<StringsClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnChar);
                Assert.IsNull(data.ColumnNChar);
                Assert.IsNull(data.ColumnNText);
                Assert.IsNull(data.ColumnNVarChar);
                Assert.IsNull(data.ColumnText);
                Assert.IsNull(data.ColumnVarChar);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<StringsClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<StringsClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsMappedCrudAsync()
        {
            // Setup
            var text = Helper.GetUnicodeString();
            var entity = new StringsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnCharMapped = text,
                ColumnNCharMapped = text,
                ColumnNTextMapped = text,
                ColumnNVarCharMapped = text,
                ColumnTextMapped = text,
                ColumnVarCharMapped = text
            };

            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<StringsMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnCharMapped, data.ColumnCharMapped.Trim());
                Assert.AreEqual(entity.ColumnNCharMapped, data.ColumnNCharMapped.Trim());
                Assert.AreEqual(entity.ColumnNTextMapped, data.ColumnNTextMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
                Assert.AreEqual(entity.ColumnTextMapped, data.ColumnTextMapped);
                Assert.AreEqual(entity.ColumnVarCharMapped, data.ColumnVarCharMapped);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<StringsMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<StringsMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsMappedNullCrudAsync()
        {
            // Setup
            var entity = new StringsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnCharMapped = null,
                ColumnNCharMapped = null,
                ColumnNTextMapped = null,
                ColumnNVarCharMapped = null,
                ColumnTextMapped = null,
                ColumnVarCharMapped = null
            };

            using (var connection = new SqlConnection(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync<StringsMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnCharMapped);
                Assert.IsNull(data.ColumnNCharMapped);
                Assert.IsNull(data.ColumnNTextMapped);
                Assert.IsNull(data.ColumnNVarCharMapped);
                Assert.IsNull(data.ColumnTextMapped);
                Assert.IsNull(data.ColumnVarCharMapped);

                // Act Delete
                var deleteAsyncResult = connection.DeleteAsync<StringsMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result;

                // Act Query
                queryResult = connection.QueryAsync<StringsMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
