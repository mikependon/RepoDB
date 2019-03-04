using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Others
{
    [TestClass]
    public class DbRepositoryOthersTest
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
                connection.DeleteAll<OthersClass>();
            }
        }

        [TestMethod]
        public void TestDbRepositoryOthersCrud()
        {
            // Setup
            var entity = new OthersClass
            {
                SessionId = Guid.NewGuid(),
                ColumnHierarchyId = "/",
                ColumnSqlVariant = "This is variant!",
                ColumnUniqueIdentifier = Guid.NewGuid(),
                ColumnXml = "<xml><person><id>1</id><name>Michael</name></person><person><id>2</id><name>RepoDb</name></person></xml>"
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<OthersClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnHierarchyId.ToString(), data.ColumnHierarchyId?.ToString());
                Assert.AreEqual(entity.ColumnSqlVariant, data.ColumnSqlVariant);
                Assert.AreEqual(entity.ColumnUniqueIdentifier, data.ColumnUniqueIdentifier);
                Assert.AreEqual(entity.ColumnXml, data.ColumnXml);

                // Act Delete
                var deletedRows = repository.Delete<OthersClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<OthersClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryOthersNullCrud()
        {
            // Setup
            var entity = new OthersClass
            {
                SessionId = Guid.NewGuid(),
                ColumnHierarchyId = null,
                ColumnSqlVariant = null,
                ColumnUniqueIdentifier = null,
                ColumnXml = null
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<OthersClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnHierarchyId);
                Assert.IsNull(data.ColumnSqlVariant);
                Assert.IsNull(data.ColumnUniqueIdentifier);
                Assert.IsNull(data.ColumnXml);

                // Act Delete
                var deletedRows = repository.Delete<OthersClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<OthersClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryOthersMappedCrud()
        {
            // Setup
            var entity = new OthersMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnHierarchyIdMapped = "/",
                ColumnSqlVariantMapped = "This is variant!",
                ColumnUniqueIdentifierMapped = Guid.NewGuid(),
                ColumnXmlMapped = "<xml><person><id>1</id><name>Michael</name></person><person><id>2</id><name>RepoDb</name></person></xml>"
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<OthersMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnHierarchyIdMapped.ToString(), data.ColumnHierarchyIdMapped?.ToString());
                Assert.AreEqual(entity.ColumnSqlVariantMapped, data.ColumnSqlVariantMapped);
                Assert.AreEqual(entity.ColumnUniqueIdentifierMapped, data.ColumnUniqueIdentifierMapped);
                Assert.AreEqual(entity.ColumnXmlMapped, data.ColumnXmlMapped);

                // Act Delete
                var deletedRows = repository.Delete<OthersMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<OthersMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryOthersMappedNullCrud()
        {
            // Setup
            var entity = new OthersMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnHierarchyIdMapped = null,
                ColumnSqlVariantMapped = null,
                ColumnUniqueIdentifierMapped = null,
                ColumnXmlMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<OthersMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnHierarchyIdMapped);
                Assert.IsNull(data.ColumnSqlVariantMapped);
                Assert.IsNull(data.ColumnUniqueIdentifierMapped);
                Assert.IsNull(data.ColumnXmlMapped);

                // Act Delete
                var deletedRows = repository.Delete<OthersMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<OthersMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryOthersCrudAsync()
        {
            // Setup
            var entity = new OthersClass
            {
                SessionId = Guid.NewGuid(),
                ColumnHierarchyId = "/",
                ColumnSqlVariant = "This is variant!",
                ColumnUniqueIdentifier = Guid.NewGuid(),
                ColumnXml = "<xml><person><id>1</id><name>Michael</name></person><person><id>2</id><name>RepoDb</name></person></xml>"
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<OthersClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnHierarchyId.ToString(), data.ColumnHierarchyId?.ToString());
                Assert.AreEqual(entity.ColumnSqlVariant, data.ColumnSqlVariant);
                Assert.AreEqual(entity.ColumnUniqueIdentifier, data.ColumnUniqueIdentifier);
                Assert.AreEqual(entity.ColumnXml, data.ColumnXml);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<OthersClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<OthersClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryOthersNullCrudAsync()
        {
            // Setup
            var entity = new OthersClass
            {
                SessionId = Guid.NewGuid(),
                ColumnHierarchyId = null,
                ColumnSqlVariant = null,
                ColumnUniqueIdentifier = null,
                ColumnXml = null
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<OthersClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnHierarchyId);
                Assert.IsNull(data.ColumnSqlVariant);
                Assert.IsNull(data.ColumnUniqueIdentifier);
                Assert.IsNull(data.ColumnXml);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<OthersClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<OthersClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryOthersMappedCrudAsync()
        {
            // Setup
            var entity = new OthersMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnHierarchyIdMapped = "/",
                ColumnSqlVariantMapped = "This is variant!",
                ColumnUniqueIdentifierMapped = Guid.NewGuid(),
                ColumnXmlMapped = "<xml><person><id>1</id><name>Michael</name></person><person><id>2</id><name>RepoDb</name></person></xml>"
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<OthersMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnHierarchyIdMapped.ToString(), data.ColumnHierarchyIdMapped?.ToString());
                Assert.AreEqual(entity.ColumnSqlVariantMapped, data.ColumnSqlVariantMapped);
                Assert.AreEqual(entity.ColumnUniqueIdentifierMapped, data.ColumnUniqueIdentifierMapped);
                Assert.AreEqual(entity.ColumnXmlMapped, data.ColumnXmlMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<OthersMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<OthersMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositoryOthersMappedNullCrudAsync()
        {
            // Setup
            var entity = new OthersMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnHierarchyIdMapped = null,
                ColumnSqlVariantMapped = null,
                ColumnUniqueIdentifierMapped = null,
                ColumnXmlMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<OthersMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnHierarchyIdMapped);
                Assert.IsNull(data.ColumnSqlVariantMapped);
                Assert.IsNull(data.ColumnUniqueIdentifierMapped);
                Assert.IsNull(data.ColumnXmlMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<OthersMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<OthersMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
