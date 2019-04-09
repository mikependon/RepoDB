using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Others
{
    [TestClass]
    public class BaseRepositoryOthersTest
    {
        private class OthersClassRepository : BaseRepository<OthersClass, SqlConnection>
        {
            public OthersClassRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

        private class OthersMapClassRepository : BaseRepository<OthersMapClass, SqlConnection>
        {
            public OthersMapClassRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

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
        public void TestBaseRepositoryOthersCrud()
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

            using (var repository = new OthersClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnHierarchyId.ToString(), data.ColumnHierarchyId?.ToString());
                Assert.AreEqual(entity.ColumnSqlVariant, data.ColumnSqlVariant);
                Assert.AreEqual(entity.ColumnUniqueIdentifier, data.ColumnUniqueIdentifier);
                Assert.AreEqual(entity.ColumnXml, data.ColumnXml);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryOthersNullCrud()
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

            using (var repository = new OthersClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnHierarchyId);
                Assert.IsNull(data.ColumnSqlVariant);
                Assert.IsNull(data.ColumnUniqueIdentifier);
                Assert.IsNull(data.ColumnXml);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryOthersMappedCrud()
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

            using (var repository = new OthersMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnHierarchyIdMapped.ToString(), data.ColumnHierarchyIdMapped?.ToString());
                Assert.AreEqual(entity.ColumnSqlVariantMapped, data.ColumnSqlVariantMapped);
                Assert.AreEqual(entity.ColumnUniqueIdentifierMapped, data.ColumnUniqueIdentifierMapped);
                Assert.AreEqual(entity.ColumnXmlMapped, data.ColumnXmlMapped);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryOthersMappedNullCrud()
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

            using (var repository = new OthersMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnHierarchyIdMapped);
                Assert.IsNull(data.ColumnSqlVariantMapped);
                Assert.IsNull(data.ColumnUniqueIdentifierMapped);
                Assert.IsNull(data.ColumnXmlMapped);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryOthersCrudAsync()
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

            using (var repository = new OthersClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnHierarchyId.ToString(), data.ColumnHierarchyId?.ToString());
                Assert.AreEqual(entity.ColumnSqlVariant, data.ColumnSqlVariant);
                Assert.AreEqual(entity.ColumnUniqueIdentifier, data.ColumnUniqueIdentifier);
                Assert.AreEqual(entity.ColumnXml, data.ColumnXml);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryOthersNullCrudAsync()
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

            using (var repository = new OthersClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnHierarchyId);
                Assert.IsNull(data.ColumnSqlVariant);
                Assert.IsNull(data.ColumnUniqueIdentifier);
                Assert.IsNull(data.ColumnXml);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryOthersMappedCrudAsync()
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

            using (var repository = new OthersMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnHierarchyIdMapped.ToString(), data.ColumnHierarchyIdMapped?.ToString());
                Assert.AreEqual(entity.ColumnSqlVariantMapped, data.ColumnSqlVariantMapped);
                Assert.AreEqual(entity.ColumnUniqueIdentifierMapped, data.ColumnUniqueIdentifierMapped);
                Assert.AreEqual(entity.ColumnXmlMapped, data.ColumnXmlMapped);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryOthersMappedNullCrudAsync()
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

            using (var repository = new OthersMapClassRepository(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result;

                // Act Query
                var queryResult = repository.QueryAsync(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnHierarchyIdMapped);
                Assert.IsNull(data.ColumnSqlVariantMapped);
                Assert.IsNull(data.ColumnUniqueIdentifierMapped);
                Assert.IsNull(data.ColumnXmlMapped);
            }
        }
    }
}
