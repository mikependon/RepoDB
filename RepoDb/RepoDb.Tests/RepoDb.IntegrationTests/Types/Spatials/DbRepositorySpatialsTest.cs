using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests.Types.Spatials
{
    [TestClass]
    public class DbRepositorySpatialsTest
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
                connection.DeleteAll<SpatialsClass>();
            }
        }

        [TestMethod]
        public void TestDbRepositorySpatialsCrud()
        {
            // Setup
            var entity = new SpatialsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeography = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
                ColumnGeometry = "LINESTRING (-122.36 47.656, -122.343 47.656)"
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<SpatialsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnGeography.ToString(), data.ColumnGeography?.ToString());
                Assert.AreEqual(entity.ColumnGeometry.ToString(), data.ColumnGeometry?.ToString());

                // Act Delete
                var deletedRows = repository.Delete<SpatialsClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<SpatialsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositorySpatialsNullCrud()
        {
            // Setup
            var entity = new SpatialsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeography = null,
                ColumnGeometry = null
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<SpatialsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeography);
                Assert.IsNull(data.ColumnGeometry);

                // Act Delete
                var deletedRows = repository.Delete<SpatialsClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<SpatialsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositorySpatialsMappedCrud()
        {
            // Setup
            var entity = new SpatialsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeographyMapped = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
                ColumnGeometryMapped = "LINESTRING (-122.36 47.656, -122.343 47.656)"
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<SpatialsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnGeographyMapped.ToString(), data.ColumnGeographyMapped?.ToString());
                Assert.AreEqual(entity.ColumnGeometryMapped.ToString(), data.ColumnGeometryMapped?.ToString());

                // Act Delete
                var deletedRows = repository.Delete<SpatialsMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<SpatialsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositorySpatialsMappedNullCrud()
        {
            // Setup
            var entity = new SpatialsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeographyMapped = null,
                ColumnGeometryMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = repository.Insert(entity);

                // Act Query
                var data = repository.Query<SpatialsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeographyMapped);
                Assert.IsNull(data.ColumnGeometryMapped);

                // Act Delete
                var deletedRows = repository.Delete<SpatialsMapClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = repository.Query<SpatialsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositorySpatialsCrudAsync()
        {
            // Setup
            var entity = new SpatialsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeography = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
                ColumnGeometry = "LINESTRING (-122.36 47.656, -122.343 47.656)"
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<SpatialsClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnGeography.ToString(), data.ColumnGeography?.ToString());
                Assert.AreEqual(entity.ColumnGeometry.ToString(), data.ColumnGeometry?.ToString());

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<SpatialsClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<SpatialsClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositorySpatialsNullCrudAsync()
        {
            // Setup
            var entity = new SpatialsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeography = null,
                ColumnGeometry = null
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<SpatialsClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeography);
                Assert.IsNull(data.ColumnGeometry);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<SpatialsClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<SpatialsClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositorySpatialsMappedCrudAsync()
        {
            // Setup
            var entity = new SpatialsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeographyMapped = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
                ColumnGeometryMapped = "LINESTRING (-122.36 47.656, -122.343 47.656)"
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<SpatialsMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.AreEqual(entity.ColumnGeographyMapped.ToString(), data.ColumnGeographyMapped?.ToString());
                Assert.AreEqual(entity.ColumnGeometryMapped.ToString(), data.ColumnGeometryMapped?.ToString());

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<SpatialsMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<SpatialsMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }

        [TestMethod]
        public void TestDbRepositorySpatialsMappedNullCrudAsync()
        {
            // Setup
            var entity = new SpatialsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeographyMapped = null,
                ColumnGeometryMapped = null
            };

            using (var repository = new DbRepository<SqlConnection>(Startup.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = repository.InsertAsync(entity);
                var id = insertResult.Result.Extract();

                // Act Query
                var queryResult = repository.QueryAsync<SpatialsMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeographyMapped);
                Assert.IsNull(data.ColumnGeometryMapped);

                // Act Delete
                var deleteAsyncResult = repository.DeleteAsync<SpatialsMapClass>(e => e.SessionId == (Guid)id);
                var count = deleteAsyncResult.Result.Extract();

                // Act Query
                queryResult = repository.QueryAsync<SpatialsMapClass>(e => e.SessionId == (Guid)id);
                data = queryResult.Result.Extract().FirstOrDefault();

                // Assert
                Assert.AreEqual(1, count);
                Assert.IsNull(data);
            }
        }
    }
}
