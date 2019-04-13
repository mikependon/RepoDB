using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb.IntegrationTests.Caches
{
    [TestClass]
    public class BaseRepositoryCachesTest
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

        #region SubClass

        public class SimpleTableRepository : BaseRepository<SimpleTable, SqlConnection>
        {
            public SimpleTableRepository(ICache cache, int cacheItemExpiration)
                : base(Database.ConnectionStringForRepoDb,
                    0,
                    ConnectionPersistency.PerCall,
                    cache,
                    cacheItemExpiration,
                    null,
                    null)
            { }
        }

        #endregion

        #region Helper

        private SimpleTable GetSimpleTable()
        {
            var random = new Random();
            return new SimpleTable
            {
                ColumnBit = true,
                ColumnDateTime = DateTime.UtcNow,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        #endregion

        #region Sync

        [TestMethod]
        public void TestSqlConnectionQueryCacheWithoutExpression()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query(orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaDynamics()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query((object)null, /* whereOrPrimaryKey */
                    (IEnumerable<OrderField>)null, /* orderBy */
                    (string)null, /* hints */
                    cacheKey, /* cacheKey */
                    (IDbTransaction)null);

                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryField()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query(where: (QueryField)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryFields()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query(where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaExpression()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query(where: (Expression<Func<SimpleTable, bool>>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryGroup()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheWithoutExpression()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync(orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaDynamics()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync((object)null, /* whereOrPrimaryKey */
                    (IEnumerable<OrderField>)null, /* orderBy */
                    (string)null, /* hints */
                    cacheKey, /* cacheKey */
                    (IDbTransaction)null).Result;

                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryField()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync(where: (QueryField)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryFields()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync(where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaExpression()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync(where: (Expression<Func<SimpleTable, bool>>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryGroup()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetSimpleTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new SimpleTableRepository(cache, cacheItemExpiration))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    transaction: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion
    }
}
