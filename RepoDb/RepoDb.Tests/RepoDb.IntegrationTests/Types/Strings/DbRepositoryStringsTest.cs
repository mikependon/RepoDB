using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Strings
{
    [TestClass]
    public class DbRepositoryStringsTest
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
        public void TestDbRepositoryStringsCrud()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<StringsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

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
        public void TestDbRepositoryStringsNullCrud()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<StringsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

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
        public void TestDbRepositoryStringsMappedCrud()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<StringsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

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
        public void TestDbRepositoryStringsMappedNullCrud()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<StringsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

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
        public void TestDbRepositoryStringsCrudAsync()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync<StringsClass>(e => e.SessionId == (Guid)id);
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
        public void TestDbRepositoryStringsNullCrudAsync()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync<StringsClass>(e => e.SessionId == (Guid)id);
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
        public void TestDbRepositoryStringsMappedCrudAsync()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync<StringsMapClass>(e => e.SessionId == (Guid)id);
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
        public void TestDbRepositoryStringsMappedNullCrudAsync()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync<StringsMapClass>(e => e.SessionId == (Guid)id);
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
        public void TestDbRepositoryStringsCrudViaTableName()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(ClassMappedNameCache.Get<StringsClass>(DbSettingMapper.Get(typeof(SqlConnection))), entity);

                // Act Query
                var data = repository.Query(ClassMappedNameCache.Get<StringsClass>(DbSettingMapper.Get(typeof(SqlConnection))), new { SessionId = (Guid)id }).FirstOrDefault();

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
        public void TestDbRepositoryStringsNullCrudViaTableName()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(ClassMappedNameCache.Get<StringsClass>(DbSettingMapper.Get(typeof(SqlConnection))), entity);

                // Act Query
                var data = repository.Query(ClassMappedNameCache.Get<StringsClass>(DbSettingMapper.Get(typeof(SqlConnection))), new { SessionId = (Guid)id }).FirstOrDefault();

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
        public void TestDbRepositoryStringsCrudViaTableNameAsync()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(ClassMappedNameCache.Get<StringsClass>(DbSettingMapper.Get(typeof(SqlConnection))), entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync(ClassMappedNameCache.Get<StringsClass>(DbSettingMapper.Get(typeof(SqlConnection))), new { SessionId = (Guid)id });
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
        public void TestDbRepositoryStringsNullCrudViaTableNameAsync()
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

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(ClassMappedNameCache.Get<StringsClass>(DbSettingMapper.Get(typeof(SqlConnection))), entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync(ClassMappedNameCache.Get<StringsClass>(DbSettingMapper.Get(typeof(SqlConnection))), new { SessionId = (Guid)id });
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
