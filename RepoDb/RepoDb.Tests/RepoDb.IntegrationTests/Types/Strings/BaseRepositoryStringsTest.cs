using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Strings
{
    [TestClass]
    public class BaseRepositoryStringsTest
    {
        private class StringsClassRepository : BaseRepository<StringsClass, SqlConnection>
        {
            public StringsClassRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

        private class StringsMapClassRepository : BaseRepository<StringsMapClass, SqlConnection>
        {
            public StringsMapClassRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

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
        public void TestBaseRepositoryStringsCrud()
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

            using (var repository = new StringsClassRepository(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnChar, data.ColumnChar.Trim());
                Assert.AreEqual(entity.ColumnNChar, data.ColumnNChar.Trim());
                Assert.AreEqual(entity.ColumnNText, data.ColumnNText);
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnText, data.ColumnText);
                Assert.AreEqual(entity.ColumnVarChar, data.ColumnVarChar);

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
        public void TestBaseRepositoryStringsNullCrud()
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

            using (var repository = new StringsClassRepository(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnChar);
                Assert.IsNull(data.ColumnNChar);
                Assert.IsNull(data.ColumnNText);
                Assert.IsNull(data.ColumnNVarChar);
                Assert.IsNull(data.ColumnText);
                Assert.IsNull(data.ColumnVarChar);

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
        public void TestBaseRepositoryStringsMappedCrud()
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

            using (var repository = new StringsMapClassRepository(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnCharMapped, data.ColumnCharMapped.Trim());
                Assert.AreEqual(entity.ColumnNCharMapped, data.ColumnNCharMapped.Trim());
                Assert.AreEqual(entity.ColumnNTextMapped, data.ColumnNTextMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
                Assert.AreEqual(entity.ColumnTextMapped, data.ColumnTextMapped);
                Assert.AreEqual(entity.ColumnVarCharMapped, data.ColumnVarCharMapped);

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
        public void TestBaseRepositoryStringsMappedNullCrud()
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

            using (var repository = new StringsMapClassRepository(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnCharMapped);
                Assert.IsNull(data.ColumnNCharMapped);
                Assert.IsNull(data.ColumnNTextMapped);
                Assert.IsNull(data.ColumnNVarCharMapped);
                Assert.IsNull(data.ColumnTextMapped);
                Assert.IsNull(data.ColumnVarCharMapped);

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
        public void TestBaseRepositoryStringsCrudAsync()
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

            using (var repository = new StringsClassRepository(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnChar, data.ColumnChar.Trim());
                Assert.AreEqual(entity.ColumnNChar, data.ColumnNChar.Trim());
                Assert.AreEqual(entity.ColumnNText, data.ColumnNText);
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnText, data.ColumnText);
                Assert.AreEqual(entity.ColumnVarChar, data.ColumnVarChar);

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
        public void TestBaseRepositoryStringsNullCrudAsync()
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

            using (var repository = new StringsClassRepository(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnChar);
                Assert.IsNull(data.ColumnNChar);
                Assert.IsNull(data.ColumnNText);
                Assert.IsNull(data.ColumnNVarChar);
                Assert.IsNull(data.ColumnText);
                Assert.IsNull(data.ColumnVarChar);

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
        public void TestBaseRepositoryStringsMappedCrudAsync()
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

            using (var repository = new StringsMapClassRepository(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnCharMapped, data.ColumnCharMapped.Trim());
                Assert.AreEqual(entity.ColumnNCharMapped, data.ColumnNCharMapped.Trim());
                Assert.AreEqual(entity.ColumnNTextMapped, data.ColumnNTextMapped);
                Assert.AreEqual(entity.ColumnNVarCharMapped, data.ColumnNVarCharMapped);
                Assert.AreEqual(entity.ColumnTextMapped, data.ColumnTextMapped);
                Assert.AreEqual(entity.ColumnVarCharMapped, data.ColumnVarCharMapped);

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
        public void TestBaseRepositoryStringsMappedNullCrudAsync()
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

            using (var repository = new StringsMapClassRepository(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnCharMapped);
                Assert.IsNull(data.ColumnNCharMapped);
                Assert.IsNull(data.ColumnNTextMapped);
                Assert.IsNull(data.ColumnNVarCharMapped);
                Assert.IsNull(data.ColumnTextMapped);
                Assert.IsNull(data.ColumnVarCharMapped);

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
