using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.Types.Spatials
{
    [TestClass]
    public class SqlConnectionSpatialsTest
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

        //[TestMethod]
        //public void TestSqlConnectionSpatialsCrud()
        //{
        //    // Setup
        //    var entity = new SpatialsClass
        //    {
        //        SessionId = Guid.NewGuid(),
        //        ColumnGeography = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
        //        ColumnGeometry = "LINESTRING (-122.36 47.656, -122.343 47.656)"
        //    };
        //
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Act Insert
        //        var id = connection.Insert(entity);
        //
        //        // Act Query
        //        var data = connection.Query<SpatialsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();
        //
        //        // Assert
        //        Assert.IsNotNull(data);
        //        Assert.AreEqual(entity.ColumnGeography.ToString(), data.ColumnGeography?.ToString());
        //        Assert.AreEqual(entity.ColumnGeometry.ToString(), data.ColumnGeometry?.ToString());
        //    }
        //}

        [TestMethod]
        public void TestSqlConnectionSpatialsNullCrud()
        {
            // Setup
            var entity = new SpatialsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeography = null,
                ColumnGeometry = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SpatialsClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeography);
                Assert.IsNull(data.ColumnGeometry);
            }
        }

        //[TestMethod]
        //public void TestSqlConnectionSpatialsMappedCrud()
        //{
        //    // Setup
        //    var entity = new SpatialsMapClass
        //    {
        //        SessionId = Guid.NewGuid(),
        //        ColumnGeographyMapped = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
        //        ColumnGeometryMapped = "LINESTRING (-122.36 47.656, -122.343 47.656)"
        //    };
        //
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Act Insert
        //        var id = connection.Insert(entity);
        //
        //        // Act Query
        //        var data = connection.Query<SpatialsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();
        //
        //        // Assert
        //        Assert.IsNotNull(data);
        //        Assert.AreEqual(entity.ColumnGeographyMapped.ToString(), data.ColumnGeographyMapped?.ToString());
        //        Assert.AreEqual(entity.ColumnGeometryMapped.ToString(), data.ColumnGeometryMapped?.ToString());
        //    }
        //}

        [TestMethod]
        public void TestSqlConnectionSpatialsMappedNullCrud()
        {
            // Setup
            var entity = new SpatialsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeographyMapped = null,
                ColumnGeometryMapped = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SpatialsMapClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeographyMapped);
                Assert.IsNull(data.ColumnGeometryMapped);
            }
        }

        //[TestMethod]
        //public async Task TestSqlConnectionSpatialsCrudAsync()
        //{
        //    // Setup
        //    var entity = new SpatialsClass
        //    {
        //        SessionId = Guid.NewGuid(),
        //        ColumnGeography = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
        //        ColumnGeometry = "LINESTRING (-122.36 47.656, -122.343 47.656)"
        //    };
        //
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Act Insert
        //        var insertResult = connection.InsertAsync(entity);
        //        var id = insertResult;
        //
        //        // Act Query
        //        var queryResult = connection.QueryAsync<SpatialsClass>(e => e.SessionId == (Guid)id);
        //        var data = queryResult.FirstOrDefault();
        //
        //        // Assert
        //        Assert.IsNotNull(data);
        //        Assert.AreEqual(entity.ColumnGeography.ToString(), data.ColumnGeography?.ToString());
        //        Assert.AreEqual(entity.ColumnGeometry.ToString(), data.ColumnGeometry?.ToString());
        //    }
        //}

        [TestMethod]
        public async Task TestSqlConnectionSpatialsNullCrudAsync()
        {
            // Setup
            var entity = new SpatialsClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeography = null,
                ColumnGeometry = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = await insertResult;

                // Act Query
                var queryResult = await connection.QueryAsync<SpatialsClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeography);
                Assert.IsNull(data.ColumnGeometry);
            }
        }

        //[TestMethod]
        //public async Task TestSqlConnectionSpatialsMappedCrudAsync()
        //{
        //    // Setup
        //    var entity = new SpatialsMapClass
        //    {
        //        SessionId = Guid.NewGuid(),
        //        ColumnGeographyMapped = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
        //        ColumnGeometryMapped = "LINESTRING (-122.36 47.656, -122.343 47.656)"
        //    };
        //
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Act Insert
        //        var insertResult = connection.InsertAsync(entity);
        //        var id = insertResult;
        //
        //        // Act Query
        //        var queryResult = connection.QueryAsync<SpatialsMapClass>(e => e.SessionId == (Guid)id);
        //        var data = queryResult.FirstOrDefault();
        //
        //        // Assert
        //        Assert.IsNotNull(data);
        //        Assert.AreEqual(entity.ColumnGeographyMapped.ToString(), data.ColumnGeographyMapped?.ToString());
        //        Assert.AreEqual(entity.ColumnGeometryMapped.ToString(), data.ColumnGeometryMapped?.ToString());
        //    }
        //}

        [TestMethod]
        public async Task TestSqlConnectionSpatialsMappedNullCrudAsync()
        {
            // Setup
            var entity = new SpatialsMapClass
            {
                SessionId = Guid.NewGuid(),
                ColumnGeographyMapped = null,
                ColumnGeometryMapped = null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(entity);
                var id = await insertResult;

                // Act Query
                var queryResult = await connection.QueryAsync<SpatialsMapClass>(e => e.SessionId == (Guid)id);
                var data = queryResult.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeographyMapped);
                Assert.IsNull(data.ColumnGeometryMapped);
            }
        }

        #endregion

        #region (TableName)

        //[TestMethod]
        //public void TestSqlConnectionSpatialsCrudViaTableName()
        //{
        //    // Setup
        //    var entity = new
        //    {
        //        SessionId = Guid.NewGuid(),
        //        ColumnGeography = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
        //        ColumnGeometry = "LINESTRING (-122.36 47.656, -122.343 47.656)"
        //    };
        //
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Act Insert
        //        var id = connection.Insert(ClassMappedNameCache.Get<SpatialsClass>(), entity);
        //
        //        // Act Query
        //        var data = connection.Query(ClassMappedNameCache.Get<SpatialsClass>(), new { SessionId = (Guid)id }).FirstOrDefault();
        //
        //        // Assert
        //        Assert.IsNotNull(data);
        //        Assert.AreEqual(entity.ColumnGeography, data.ColumnGeography?.ToString());
        //        Assert.AreEqual(entity.ColumnGeometry, data.ColumnGeometry?.ToString());
        //    }
        //}

        [TestMethod]
        public void TestSqlConnectionSpatialsNullCrudViaTableName()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnGeography = (object)null,
                ColumnGeometry = (object)null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var id = connection.Insert(ClassMappedNameCache.Get<SpatialsClass>(), entity);

                // Act Query
                var data = connection.Query(ClassMappedNameCache.Get<SpatialsClass>(), new { SessionId = (Guid)id }).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeography);
                Assert.IsNull(data.ColumnGeometry);
            }
        }

        //[TestMethod]
        //public async Task TestSqlConnectionSpatialsCrudViaAsyncViaTableName()
        //{
        //    // Setup
        //    var entity = new
        //    {
        //        SessionId = Guid.NewGuid(),
        //        ColumnGeography = "POLYGON ((0 0, 50 0, 50 50, 0 50, 0 0))",
        //        ColumnGeometry = "LINESTRING (-122.36 47.656, -122.343 47.656)"
        //    };
        //
        //    using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
        //    {
        //        // Act Insert
        //        var insertResult = connection.InsertAsync(ClassMappedNameCache.Get<SpatialsClass>(), entity);
        //        var id = insertResult;
        //
        //        // Act Query
        //        var queryResult = connection.QueryAsync(ClassMappedNameCache.Get<SpatialsClass>(), new { SessionId = (Guid)id });
        //        var data = queryResult.FirstOrDefault();
        //
        //        // Assert
        //        Assert.IsNotNull(data);
        //        Assert.AreEqual(entity.ColumnGeography, data.ColumnGeography?.ToString());
        //        Assert.AreEqual(entity.ColumnGeometry, data.ColumnGeometry?.ToString());
        //    }
        //}

        [TestMethod]
        public async Task TestSqlConnectionSpatialsNullCrudViaAsyncViaTableName()
        {
            // Setup
            var entity = new
            {
                SessionId = Guid.NewGuid(),
                ColumnGeography = (object)null,
                ColumnGeometry = (object)null
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act Insert
                var insertResult = connection.InsertAsync(ClassMappedNameCache.Get<SpatialsClass>(), entity);
                var id = await insertResult;

                // Act Query
                var queryResult = await connection.QueryAsync(ClassMappedNameCache.Get<SpatialsClass>(), new { SessionId = (Guid)id });
                var data = queryResult.FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Assert.IsNull(data.ColumnGeography);
                Assert.IsNull(data.ColumnGeometry);
            }
        }

        #endregion
    }
}
