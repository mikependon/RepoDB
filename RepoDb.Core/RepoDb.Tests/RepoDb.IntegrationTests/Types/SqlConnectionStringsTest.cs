using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Strings
{
    [TestClass]
    public class SqlConnectionStringsTest
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

        #region <TEntity>

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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
            }
        }

        #endregion

        #region (TableName)

        [TestMethod]
        public void TestSqlConnectionStringsCrudViaTableName()
        {
            // Setup
            var text = Helper.GetUnicodeString();
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnChar = text,
                ColumnNChar = text,
                ColumnNText = text,
                ColumnNVarChar = text,
                ColumnText = text,
                ColumnVarChar = text
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(ClassMappedNameCache.Get<StringsClass>(), entity);

                // Act Query
                var data = connection.Query(ClassMappedNameCache.Get<StringsClass>(), new { SessionId = (Guid)id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnChar, data.ColumnChar.Trim());
                Assert.AreEqual(entity.ColumnNChar, data.ColumnNChar.Trim());
                Assert.AreEqual(entity.ColumnNText, data.ColumnNText);
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnText, data.ColumnText);
                Assert.AreEqual(entity.ColumnVarChar, data.ColumnVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsNullCrudViaTableName()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnChar = (string)null,
                ColumnNChar = (string)null,
                ColumnNText = (string)null,
                ColumnNVarChar = (string)null,
                ColumnText = (string)null,
                ColumnVarChar = (string)null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(ClassMappedNameCache.Get<StringsClass>(), entity);

                // Act Query
                var data = connection.Query(ClassMappedNameCache.Get<StringsClass>(), new { SessionId = (Guid)id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnChar);
                Assert.IsNull(data.ColumnNChar);
                Assert.IsNull(data.ColumnNText);
                Assert.IsNull(data.ColumnNVarChar);
                Assert.IsNull(data.ColumnText);
                Assert.IsNull(data.ColumnVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsCrudViaAsyncViaTableName()
        {
            // Setup
            var text = Helper.GetUnicodeString();
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnChar = text,
                ColumnNChar = text,
                ColumnNText = text,
                ColumnNVarChar = text,
                ColumnText = text,
                ColumnVarChar = text
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(ClassMappedNameCache.Get<StringsClass>(), entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<StringsClass>(), new { SessionId = (Guid)id });
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnChar, data.ColumnChar.Trim());
                Assert.AreEqual(entity.ColumnNChar, data.ColumnNChar.Trim());
                Assert.AreEqual(entity.ColumnNText, data.ColumnNText);
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
                Assert.AreEqual(entity.ColumnText, data.ColumnText);
                Assert.AreEqual(entity.ColumnVarChar, data.ColumnVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionStringsNullCrudViaAsyncViaTableName()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnChar = (string)null,
                ColumnNChar = (string)null,
                ColumnNText = (string)null,
                ColumnNVarChar = (string)null,
                ColumnText = (string)null,
                ColumnVarChar = (string)null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(ClassMappedNameCache.Get<StringsClass>(), entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<StringsClass>(), new { SessionId = (Guid)id });
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnChar);
                Assert.IsNull(data.ColumnNChar);
                Assert.IsNull(data.ColumnNText);
                Assert.IsNull(data.ColumnNVarChar);
                Assert.IsNull(data.ColumnText);
                Assert.IsNull(data.ColumnVarChar);
            }
        }

        #endregion
    }
}
